using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用精灵个体值计算工具
/// 负责计算最终个体值（FinalStats）
/// 通过 CalcScheme 变量控制使用哪种计算方案
/// </summary>
public static class DevPetIvTool
{
	// ==================== 计算方案枚举 ====================

	/// <summary>
	/// 计算方案
	/// 1 = 简化版：基础Iv + 等级修正 + 天赋修正
	/// 2 = 完整版：包含种族值、个体值、等级、成长值的洛克王国风格公式
	/// </summary>
	public static int CalcScheme { get; set; } = 1;

	// ==================== 公开方法 ====================

	/// <summary>
	/// 根据当前 CalcScheme 计算最终属性值
	/// </summary>
	public static Dictionary<EnumPetBaseStats, int> Calculate(
		Dictionary<EnumPetBaseStats, int> raceStats,
		Dictionary<EnumPetBaseStats, int> iv,
		Dictionary<EnumPetBaseStats, int> talent,
		int level,
		EnumPetNature nature)
	{
		return CalcScheme switch
		{
			2 => CalculateFinalStats2(raceStats, iv, talent, level, nature),
			_ => CalculateFinalStats1(iv, talent, level, nature),
		};
	}

	/// <summary>
	/// 便捷方法：直接传入 InsPackPetData 根据当前 CalcScheme 计算并更新 FinalStats
	/// </summary>
	public static void Update(InsPackPetData petData)
	{
		petData.FinalStats = Calculate(petData.Iv, petData.Iv, petData.Talent, petData.Level, petData.Nature);
	}

	// ==================== 私有方法 ====================

	/// <summary>
	/// 计算方案1 — 简化版
	/// 公式：最终值 = (基础Iv + 等级×系数 + floor(等级/10)×天赋) × 性格修正
	/// HP: (基础Iv + 等级修正) × 2 + 天赋修正 再 × 性格修正
	/// </summary>
	private static Dictionary<EnumPetBaseStats, int> CalculateFinalStats1(
		Dictionary<EnumPetBaseStats, int> iv,
		Dictionary<EnumPetBaseStats, int> talent,
		int level,
		EnumPetNature nature)
	{
		var finalStats = new Dictionary<EnumPetBaseStats, int>();

		foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
		{
			int baseIv = iv.GetValueOrDefault(stat, 0);
			int levelBonus = (int)(level * 0.5f);
			int talentMultiplier = level / 10;
			int statTalent = talent.GetValueOrDefault(stat, 0);
			int talentBonus = talentMultiplier * statTalent;

			int subtotal;
			if (stat == EnumPetBaseStats.HP)
			{
				subtotal = (baseIv + levelBonus) * 2 + talentBonus;
			}
			else
			{
				subtotal = baseIv + levelBonus + talentBonus;
			}

			float natureMultiplier = GetNatureMultiplier(stat, nature);
			int finalValue = (int)(subtotal * natureMultiplier);
			finalValue = Math.Max(0, finalValue);

			finalStats[stat] = finalValue;
		}

		return finalStats;
	}

	/// <summary>
	/// 计算方案2 — 完整版（洛克王国世界风格）
	/// 其他属性 = [(种族值 + 个体值/2) / 2 × (1 + 等级/50) + 10] × 性格修正 + 成长值
	/// 生命值   = [(等级/25 + 1) × (种族值 + 个体值/2) / 2 + 等级 + 10] × 性格修正 + 成长值
	/// </summary>
	private static Dictionary<EnumPetBaseStats, int> CalculateFinalStats2(
		Dictionary<EnumPetBaseStats, int> raceStats,
		Dictionary<EnumPetBaseStats, int> iv,
		Dictionary<EnumPetBaseStats, int> talent,
		int level,
		EnumPetNature nature)
	{
		var fullStats = new Dictionary<EnumPetBaseStats, int>();

		foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
		{
			int baseRace = raceStats.GetValueOrDefault(stat, 0);
			int statIv = iv.GetValueOrDefault(stat, 0);
			int statTalent = talent.GetValueOrDefault(stat, 0);

			float result;

			if (stat == EnumPetBaseStats.HP)
			{
				float factor = level / 25f + 1f;
				float basePart = (baseRace + statIv / 2f) / 2f;
				result = (factor * basePart + level + 10) * GetNatureMultiplier(stat, nature) + statTalent;
			}
			else
			{
				float basePart = (baseRace + statIv / 2f) / 2f;
				float levelFactor = 1f + level / 50f;
				result = (basePart * levelFactor + 10) * GetNatureMultiplier(stat, nature) + statTalent;
			}

			int finalValue = (int)result;
			finalValue = Math.Max(0, finalValue);

			fullStats[stat] = finalValue;
		}

		return fullStats;
	}

	/// <summary>
	/// 获取性格修正系数
	/// </summary>
	private static float GetNatureMultiplier(EnumPetBaseStats stat, EnumPetNature nature)
	{
		int natureUp = PetNatureDesign.GetUpStat((int)nature);
		int natureDown = PetNatureDesign.GetDownStat((int)nature);

		if ((int)stat == natureUp) return 1.1f;
		if ((int)stat == natureDown) return 0.9f;
		return 1.0f;
	}
}