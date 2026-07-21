using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 战斗伤害计算工具
/// 提供技能伤害计算方法，支持物理、魔法、固伤三种伤害类型，
/// 系别克制关系计算委托给 PetTypeDesign
/// </summary>
public static class FightDamageTool
{
	private static readonly Random _random = new();

	/// <summary>
	/// 计算技能伤害值
	/// 支持三种伤害类型：
	///   AttackType=0（固伤）→ 直接返回技能威力
	///   AttackType=2（物攻）→ 基于 ATK vs DEF 计算
	///   AttackType=3（魔攻）→ 基于 MATK vs MDEF 计算
	/// </summary>
	/// <param name="skill">战斗技能实例（包含技能基础数据及修正后的实际威力）</param>
	/// <param name="attacker">攻击方宠物战斗数据</param>
	/// <param name="defender">防守方宠物战斗数据</param>
	/// <returns>计算后的最终伤害值（整数，向下取整，最低为 0）</returns>
	public static int CalcDamage(InsFightSkill skill, InsFightPetData attacker, InsFightPetData defender)
	{
		if (skill?.Skill == null || attacker == null || defender == null)
			return 0;

		InsSkill skillData = skill.Skill;
		int power = Math.Max(skill.ActualAttackValue, 1); // 威力至少为1

		// ─── 固伤：直接返回威力 ───
		if (skillData.AttackType == 0)
			return power;

		// ─── 物攻 / 魔攻 ───
		// 攻防属性选择
		EnumPetBaseStats atkStat = skillData.AttackType == 2 ? EnumPetBaseStats.ATK : EnumPetBaseStats.MATK;
		EnumPetBaseStats defStat = skillData.AttackType == 2 ? EnumPetBaseStats.DEF : EnumPetBaseStats.MDEF;

		int atkVal = StatOrDefault(attacker.FinalStats, atkStat, 30);
		int defVal = StatOrDefault(defender.FinalStats, defStat, 30);

		// 防止除零
		if (atkVal <= 0) atkVal = 1;
		if (defVal <= 0) defVal = 1;

		int level = Math.Max(attacker.Level, 1);

		// 1. 先算基础伤害（只取整一次）
		float baseCalc = ((2.0f * level / 5.0f + 2.0f) * power * ((float)atkVal / defVal)) / 50.0f + 2.0f;
		int baseDamage = Math.Max((int)baseCalc, 1); 

		// 2. 获取系别克制倍率（如果免疫直接返回0）
		float typeMod = GetTypeEffectiveness(skillData.PetType, defender.PetTypes);
		if (typeMod <= 0.0f) return 0;

		// 3. ★核心修正：合并所有系数为一个总倍率（顺序在此毫无影响！）
		float totalModifier = 1.0f;
		totalModifier *= typeMod;                    // 系别克制
		if (skill.IsSameType(attacker)) 
			totalModifier *= 1.5f;                   // 本系加成（STAB）
		// 以后你如果要加"会心一击"、"天气"、"特性"等，都乘在这里

		// 4. 统一应用总倍率，并取整
		int damageAfterMod = (int)(baseDamage * totalModifier);

		// 5. 最后乘随机数（0.85~1.00）并取整
		float randomFactor = 0.85f + (float)_random.NextDouble() * 0.15f;
		int finalDamage = (int)(damageAfterMod * randomFactor);

		GD.Print($"基础={baseDamage}, 总倍率={totalModifier:F2}, 随机={randomFactor:F2}，最终={finalDamage}");
		return Math.Max(finalDamage, 0);
	}

	/// <summary>
	/// 获取攻击方系别对防守方系别的克制系数
	/// 防守方可能有多个系别（双属性），取乘积
	/// 委托给 PetTypeDesign.GetDamageMultipliers 计算
	/// </summary>
	/// <param name="atkType">攻击方系别</param>
	/// <param name="defTypes">防守方系别列表</param>
	/// <returns>克制系数乘积（0.0 表示免疫）</returns>
	public static float GetTypeEffectiveness(int atkType, List<EnumPetType> defTypes)
	{
		return PetTypeDesign.GetDamageMultipliers(atkType, defTypes);
	}

	/// <summary>
	/// 从属性字典安全读取数值，不存在返回默认值
	/// </summary>
	private static int StatOrDefault(Dictionary<EnumPetBaseStats, int> dict, EnumPetBaseStats key, int def)
	{
		if (dict != null && dict.TryGetValue(key, out int val))
			return val;
		return def;
	}
}