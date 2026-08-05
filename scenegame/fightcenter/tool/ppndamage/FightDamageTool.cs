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
	/// 计算技能伤害值（核心方法，传入双方个体值字典）
	/// 支持三种伤害类型：
	///   AttackType=0（固伤）→ 直接返回技能威力
	///   AttackType=2（物攻）→ 基于 ATK vs DEF 计算
	///   AttackType=3（魔攻）→ 基于 MATK vs MDEF 计算
	/// </summary>
	/// <param name="skill">战斗技能实例（包含技能基础数据及修正后的实际威力）</param>
	/// <param name="attackerStats">攻击方个体值字典</param>
	/// <param name="defenderStats">防守方个体值字典</param>
	/// <param name="level">攻击方等级</param>
	/// <param name="defPetTypes">防守方系别列表（用于属性克制计算）</param>
	/// <param name="isSameType">攻击方技能是否为攻击方本系</param>
	/// <returns>计算后的最终伤害值（整数，向下取整，最低为 0）</returns>
	public static int CalcBasicDamage(InsFightSkill skill, Dictionary<EnumPetBaseStats, int> attackerStats, Dictionary<EnumPetBaseStats, int> defenderStats, int level, List<EnumPetType> defPetTypes, bool isSameType)
	{
		if (skill?.Skill == null)
			return 0;

		InsSkill skillData = skill.Skill;
		int power = Math.Max(skill.ActualAttackValue, 1); // 威力至少为1

		// ─── 固伤：直接返回威力 ───
		if (skillData.AttackType == 0)
			return power;

		// 根据技能类型选择攻防属性
		EnumPetBaseStats atkStat = skillData.AttackType == 2 ? EnumPetBaseStats.ATK : EnumPetBaseStats.MATK;
		EnumPetBaseStats defStat = skillData.AttackType == 2 ? EnumPetBaseStats.DEF : EnumPetBaseStats.MDEF;

		int atkVal = StatOrDefault(attackerStats, atkStat, 30);
		int defVal = StatOrDefault(defenderStats, defStat, 30);

		// 防止除零
		if (atkVal <= 0) atkVal = 1;
		if (defVal <= 0) defVal = 1;

		level = Math.Max(level, 1);

		// 1. 先算基础伤害（只取整一次）
		// 宝可梦官方伤害公式（Gen III+）：
		//   Damage = ((((2 × Level / 5 + 2) × Power × A / D) / 50) + 2) × Modifiers
		//   其中 A=攻击，D=防御，每次除法/乘法后向下取整
		float baseCalc = CalcBaseDamageValue(level, power, atkVal, defVal);
		int baseDamage = Math.Max((int)baseCalc, 1); 

		// 2. 获取系别克制倍率（如果免疫直接返回0）
		float typeMod = GetTypeEffectiveness(skillData.PetType, defPetTypes);
		if (typeMod <= 0.0f) return 0;

		// 3. ★核心修正：合并所有系数为一个总倍率（顺序在此毫无影响！）
		float totalModifier = 1.0f;
		totalModifier *= typeMod;                    // 系别克制
		if (isSameType) 
			totalModifier *= 1.5f;                   // 本系加成（STAB）
		// 以后你如果要加"会心一击"、"天气"、"特性"等，都乘在这里

		// 4. 统一应用总倍率，并取整
		int damageAfterMod = (int)(baseDamage * totalModifier);

		// 5. 最后乘随机数（0.9~1.00）并取整
		float randomFactor = 0.9f + (float)_random.NextDouble() * 0.1f;
		int finalDamage = (int)(damageAfterMod * randomFactor);

		GD.Print($"			基础={baseDamage}, 总倍率={totalModifier:F2}, 随机={randomFactor:F2}，最终={finalDamage}");
		return Math.Max(finalDamage, 0);
	}

	/// <summary>
	/// 计算技能伤害值（传入宠物对象，提取数值后委托给核心方法）
	/// </summary>
	/// <param name="skill">战斗技能实例</param>
	/// <param name="attacker">攻击方宠物战斗数据</param>
	/// <param name="defender">防守方宠物战斗数据</param>
	/// <returns>计算后的最终伤害值</returns>
	public static int CalcSkillFinalDamage(InsFightSkill skill, InsFightPetData attacker, InsFightPetData defender)
	{
		if (skill?.Skill == null || attacker == null || defender == null)
			return 0;

		// 干预 六维：基于基础 FinalStats，叠加 Buff 加成值
		Dictionary<EnumPetBaseStats, int> attackStats = new(attacker.FinalStats);
		Dictionary<EnumPetBaseStats, int> defenderStats = new(defender.FinalStats);

		// 首先根据 Buff 进行干预
		Dictionary<EnumPetBaseStats, int> buffAtk = FightBuffTool.CalculateBuffStats(attacker);
		Dictionary<EnumPetBaseStats, int> buffDef = FightBuffTool.CalculateBuffStats(defender);
		FightBuffTool.MergeBuffStats(attackStats, buffAtk);
		FightBuffTool.MergeBuffStats(defenderStats, buffDef);
		/*
		GD.Print($"[FightDamageTool] 攻击方 Buff 加成: {DictToString(buffAtk)}");
		GD.Print($"[FightDamageTool] 防守方 Buff 加成: {DictToString(buffDef)}");
		GD.Print($"[FightDamageTool] 攻击方叠加后: {DictToString(attackStats)}");
		GD.Print($"[FightDamageTool] 防守方叠加后: {DictToString(defenderStats)}");
		*/
		return CalcBasicDamage(skill, attackStats, defenderStats, attacker.Level, defender.PetTypes, skill.IsSameType(attacker));
	}

	/// <summary>
	/// 计算技能伤害值（根据 side 自动获取场上精灵数据）
	/// </summary>
	/// <param name="skill">战斗技能实例</param>
	/// <param name="side">攻击方标识</param>
	/// <returns>计算后的最终伤害值</returns>
	public static int CalcSkillFinalDamage(InsFightSkill skill, EnumWho side)
	{
		InsFightPetData attacker, defender;
		if (side == EnumWho.My)
		{
			attacker = FightLandMyStandPet.Instance?.FightPetData;
			defender = FightLandYouStandPet.Instance?.FightPetData;
		}
		else
		{
			attacker = FightLandYouStandPet.Instance?.FightPetData;
			defender = FightLandMyStandPet.Instance?.FightPetData;
		}

		return CalcSkillFinalDamage(skill, attacker, defender);
	}

	/// <summary>
	/// 宝可梦基础伤害值计算（官方公式主体，不含倍率修正）
	///   Damage = (((2 × Level / 5 + 2) × Power × A / D) / 50) + 2
	/// 其中 A = 攻击方攻击/特攻，D = 防守方防御/特防
	/// 返回 float 供调用方决定取整时机
	/// </summary>
	/// <param name="level">攻击方等级</param>
	/// <param name="power">技能威力</param>
	/// <param name="atkVal">攻击方攻击/特攻数值</param>
	/// <param name="defVal">防守方防御/特防数值</param>
	/// <returns>洛克王国手游公式基础伤害浮点值</returns>
	private static float CalcBaseDamageValue(int level, int power, int atkVal, int defVal)
	{
		// float pokemonDamage = ((2.0f * level / 5.0f + 2.0f) * power * ((float)atkVal / defVal)) / 50.0f + 2.0f;

		// TODO: 自创公式
		return ((2.0f * level / 5.0f + 2.0f) * power * ((float)atkVal / defVal)) / 30f + 2.0f;
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
	/// 获取技能对应的攻击方数值（ATK 或 MATK）
	/// </summary>
	/// <param name="skill">技能实例</param>
	/// <param name="attacker">攻击方精灵数据</param>
	/// <returns>攻击数值，默认 30</returns>
	public static int GetAttackValue(InsFightSkill skill, InsFightPetData attacker)
	{
		if (skill?.Skill == null || attacker == null)
			return 30;

		EnumPetBaseStats atkStat = skill.Skill.AttackType == 2 ? EnumPetBaseStats.ATK : EnumPetBaseStats.MATK;
		return StatOrDefault(attacker.FinalStats, atkStat, 30);
	}

	/// <summary>
	/// 获取技能对应的防守方数值（DEF 或 MDEF）
	/// </summary>
	/// <param name="skill">技能实例</param>
	/// <param name="defender">防守方精灵数据</param>
	/// <returns>防御数值，默认 30</returns>
	public static int GetDefenseValue(InsFightSkill skill, InsFightPetData defender)
	{
		if (skill?.Skill == null || defender == null)
			return 30;

		EnumPetBaseStats defStat = skill.Skill.AttackType == 2 ? EnumPetBaseStats.DEF : EnumPetBaseStats.MDEF;
		return StatOrDefault(defender.FinalStats, defStat, 30);
	}

	/// <summary>
	/// 将属性字典格式化为字符串（用于日志输出）
	/// </summary>
	private static string DictToString(Dictionary<EnumPetBaseStats, int> dict)
	{
		if (dict == null || dict.Count == 0)
			return "[]";

		var parts = new System.Collections.Generic.List<string>();
		foreach (var kvp in dict)
			parts.Add($"{kvp.Key}={kvp.Value}");
		return "[" + string.Join(", ", parts) + "]";
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