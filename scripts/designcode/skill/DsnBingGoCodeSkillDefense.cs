using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 防御技能应对成功的特殊代码执行类
/// 处理防御技能（SkillType=2）在成功应对攻击时的伤害减伤计算
/// </summary>
public static class DsnBingGoCodeSkillDefense
{
	/// <summary>
	/// 计算防御技能应对后的最终伤害
	/// 根据技能的 damage_reduction_rate 对原始伤害进行减伤
	/// </summary>
	/// <param name="defenseSkill">防御方使用的技能（必须为防御技能）</param>
	/// <param name="rawDamage">攻击方原本造成的原始伤害</param>
	/// <returns>经过防御减伤后的最终伤害</returns>
	public static int CalcFinalDamage(InsFightSkill defenseSkill, int rawDamage)
	{
		if (defenseSkill?.Skill == null)
			return rawDamage;

		// 从技能静态数据获取减伤率（0-100），默认0
		int reductionRate = defenseSkill.Skill.DamageReductionRate;

		// 减伤率范围限制 0-100
		reductionRate = Math.Clamp(reductionRate, 0, 100);

		// 无减伤直接返回原始伤害
		if (reductionRate <= 0)
			return rawDamage;

		// 完全减伤（100%）返回0
		if (reductionRate >= 100)
			return 0;

		// 计算减伤后的伤害
		// 例如 reductionRate=70 → 受到 30% 伤害
		float remaining = (100.0f - reductionRate) / 100.0f;
		int finalDamage = (int)(rawDamage * remaining);

		return Math.Max(finalDamage, 0);
	}
}
