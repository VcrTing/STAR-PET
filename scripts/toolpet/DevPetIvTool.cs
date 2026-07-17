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
	public static int CalcScheme { get; set; } = 3;

	// ==================== 公开方法 ====================

	/// <summary>
	/// 根据等级和亲密值获取成长值字典
	/// 生命受亲密值影响：亲密 0 时 60级 +80，亲密加成 0~20（亲密 100 时总 +100）
	/// 其他五属性 60级时各 +50，不受亲密影响
	/// 浮点向下取整
	/// </summary>
	/// <param name="level">精灵等级</param>
	/// <param name="intimacy">亲密值（0~100）</param>
	/// <returns>六维成长值字典</returns>
	public static Dictionary<EnumPetBaseStats, int> GetGrowth2(int level, int intimacy)
	{
		var growth = new Dictionary<EnumPetBaseStats, int>();
		intimacy = Math.Clamp(intimacy, 0, 100);

		foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
		{
			if (stat == EnumPetBaseStats.HP)
			{
				// 基础 60级时 +80，亲密加成 0~20（亲密100时总 +100）
				int baseHp = (int)(level * 80f / 60f);
				int bonusHp = (int)(level * 20f / 60f * intimacy / 100f);
				growth[stat] = baseHp + bonusHp;
			}
			else
			{
				// 其他五属性各：60级时 +50
				growth[stat] = (int)(level * 50f / 60f);
			}
		}

		return growth;
	}

	/// <summary>
	/// 根据当前 CalcScheme 计算最终属性值（固定使用方案3）
	/// </summary>
	public static Dictionary<EnumPetBaseStats, int> Calculate(
		Dictionary<EnumPetBaseStats, int> iv,
		Dictionary<EnumPetBaseStats, int> talent,
		int level,
		EnumPetNature nature,
		Dictionary<EnumPetBaseStats, int> growth)
	{
		return CalculateFinalStats3(iv, talent, level, nature, growth);
	}

	/// <summary>
	/// 传入 InsPackPetData + 自定义等级，返回计算结果字典（不修改原数据）
	/// 用于战斗时用不同等级重新计算 FinalStats
	/// </summary>
	public static Dictionary<EnumPetBaseStats, int> Update(InsPackPetData petData, int level, Dictionary<EnumPetBaseStats, int> growth)
	{
		return Calculate(petData.Iv, petData.Talent, level, petData.Nature, growth);
	}

	// ==================== 天赋生成 ====================

	/// <summary>
	/// 将天赋点数字典转换为按等级缩放后的最终天赋值字典
	/// 公式：最终天赋值 = 点数 × 等级 / 12
	/// 点数 = 0 的个体最终天赋值也为 0
	/// </summary>
	/// <param name="talent">天赋点数字典（stat -> 点数，如 ATK=10）</param>
	/// <param name="level">精灵等级</param>
	/// <returns>stat -> 最终天赋值的字典</returns>
	public static Dictionary<EnumPetBaseStats, int> GenerateTalentDict(
		Dictionary<EnumPetBaseStats, int> talent,
		int level)
	{
		var result = new Dictionary<EnumPetBaseStats, int>();

		foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
		{
			int points = talent.GetValueOrDefault(stat, 0);
			result[stat] = (int)(points * level / 12f);
		}

		return result;
	}

	// ==================== 核心计算 ====================

	/// <summary>
	/// 计算方案3 — 完整版公式
	/// 天赋值由原始点数按等级缩放：缩放值 = 点数 × 等级 / 12
	/// 其他属性 = [(个体值 + 缩放值/2) / 2 × (1 + 等级/50) + 10] × 性格修正 + 成长值
	/// 生命值   = [(等级/25 + 1) × (个体值 + 缩放值/2) / 2 + 等级 + 10] × 性格修正 + 成长值
	/// </summary>
	public static Dictionary<EnumPetBaseStats, int> CalculateFinalStats3(
		Dictionary<EnumPetBaseStats, int> iv,
		Dictionary<EnumPetBaseStats, int> talent,
		int level,
		EnumPetNature nature,
		Dictionary<EnumPetBaseStats, int> growth)
	{
		var fullStats = new Dictionary<EnumPetBaseStats, int>();

		foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
		{
			int statIv = iv.GetValueOrDefault(stat, 0);
			int statTalent = (int)(talent.GetValueOrDefault(stat, 0) * level / 12f);
			int statGrowth = growth.GetValueOrDefault(stat, 0);

			float result;

			if (stat == EnumPetBaseStats.HP)
			{
				float factor = level / 25f + 1f;
				float basePart = (statIv + statTalent / 2f) / 2f;
				result = (factor * basePart + level + 10) * GetNatureMultiplier(stat, nature) + statGrowth;
			}
			else
			{
				float basePart = (statIv + statTalent / 2f) / 2f;
				float levelFactor = 1f + level / 50f;
				result = (basePart * levelFactor + 10) * GetNatureMultiplier(stat, nature) + statGrowth;
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

	// ==================== 以下为暂未使用的方法（保留备选） ====================

	/* 暂未使用
	/// <summary>
	/// 根据等级获取成长值字典（无亲密加成）
	/// 每级成长：生命 60级时 +100，其他五属性 60级时各 +50
	/// 浮点向下取整
	/// </summary>
	public static Dictionary<EnumPetBaseStats, int> GetGrowth1(int level)
	{
		var growth = new Dictionary<EnumPetBaseStats, int>();

		foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
		{
			if (stat == EnumPetBaseStats.HP)
			{
				growth[stat] = (int)(level * 100f / 60f);
			}
			else
			{
				growth[stat] = (int)(level * 50f / 60f);
			}
		}

		return growth;
	}
	*/

	/* 暂未使用
	/// <summary>
	/// 便捷方法：直接传入 InsPackPetData 计算并更新 FinalStats（使用自身等级）
	/// </summary>
	public static void Update(InsPackPetData petData, Dictionary<EnumPetBaseStats, int> growth)
	{
		petData.FinalStats = Calculate(petData.Iv, petData.Talent, petData.Level, petData.Nature, growth);
	}
	*/

	/* 暂未使用
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
	*/

	/* 暂未使用
	/// <summary>
	/// 计算方案2 — 完整版（洛克王国世界风格）
	/// 其他属性 = [(个体值 + 天赋值/2) / 2 × (1 + 等级/50) + 10] × 性格修正 + 成长值
	/// 生命值   = [(等级/25 + 1) × (个体值 + 天赋值/2) / 2 + 等级 + 10] × 性格修正 + 成长值
	/// </summary>
	public static Dictionary<EnumPetBaseStats, int> CalculateFinalStats2(
		Dictionary<EnumPetBaseStats, int> iv,
		Dictionary<EnumPetBaseStats, int> talent,
		int level,
		EnumPetNature nature,
		Dictionary<EnumPetBaseStats, int> growth)
	{
		return CalculateFinalStats3(iv, talent, level, nature, growth);
	}
	*/
}