using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵天赋配置
/// 天赋值范围 0-10
/// 天赋类型：零天赋(固定0)、普通天赋(0-3)、一般般天赋(4-6)、好天赋(7-9)、极品天赋(10)
/// </summary>
public static class PetTalentDesign
{
	// const int 常量已提取到 EnumPetTalent 枚举，见 scripts/design/pet/enum/EnumPetTalent.cs
	public const int Zero = (int)EnumPetTalent.Zero;             // 零天赋
	public const int Normal = (int)EnumPetTalent.Normal;         // 普通天赋
	public const int NormalPlus = (int)EnumPetTalent.NormalPlus; // 一般般天赋
	public const int Good = (int)EnumPetTalent.Good;             // 好天赋
	public const int Excellent = (int)EnumPetTalent.Excellent;   // 极品天赋

	/// <summary>
	/// 根据天赋值获取天赋类型
	/// </summary>
	public static int GetTalentType(int value)
	{
		if (value >= 10) return Excellent;
		if (value >= 7) return Good;
		if (value >= 4) return NormalPlus;
		if (value > 0) return Normal;
		return Zero;
	}

	/// <summary>
	/// 获取天赋类型中文名称
	/// </summary>
	public static string GetTalentTypeName(int talentType)
	{
		return talentType switch
		{
			(int)EnumPetTalent.Zero => "零天赋",
			(int)EnumPetTalent.Normal => "普通天赋",
			(int)EnumPetTalent.NormalPlus => "一般般天赋",
			(int)EnumPetTalent.Good => "好天赋",
			(int)EnumPetTalent.Excellent => "极品天赋",
			_ => $"未知({talentType})"
		};
	}

	/// <summary>
	/// 获取天赋值对应的类型名称
	/// </summary>
	public static string GetTalentNameByValue(int value)
	{
		return GetTalentTypeName(GetTalentType(value));
	}

	/// <summary>
	/// 根据天赋类型随机生成天赋值
	/// </summary>
	/// <param name="talentType">天赋类型（0=零，1=普通，2=一般般，3=好，4=极品）</param>
	/// <returns>天赋值：零 0，普通 0-3，一般般 4-6，好 7-9，极品 10</returns>
	public static int RollTalentValue(int talentType)
	{
		return talentType switch
		{
			(int)EnumPetTalent.Zero => 0,
			(int)EnumPetTalent.NormalPlus => RandomTool.Range(4, 6),
			(int)EnumPetTalent.Good => RandomTool.Range(7, 9),
			(int)EnumPetTalent.Excellent => 10,
			_ => RandomTool.Range(0, 3) // Normal 或未知类型
		};
	}

	/// <summary>
	/// 生成全属性天赋值字典（所有 EnumPetBaseStats 都赋予相同的天赋类型）
	/// 用于初始精灵或特殊精灵的全极品天赋生成
	/// </summary>
	/// <param name="talentType">天赋类型（默认 Excellent=4，全极品）</param>
	/// <returns>全属性天赋值字典，stat -> talent</returns>
	public static Dictionary<EnumPetBaseStats, int> GenerateAllTalentDict(int talentType)
	{
		var dict = new Dictionary<EnumPetBaseStats, int>();
		foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
		{
			dict[stat] = RollTalentValue(talentType);
		}
		return dict;
	}

	/// <summary>
	/// 随机生成宠物的天赋值字典（3 个随机属性获得随机天赋值，其余为 0）
	/// 用于野生精灵捕捉时随机生成天赋分布
	/// </summary>
	/// <returns>天赋值字典，key=随机 3 个 EnumPetBaseStats，value=随机天赋值（0-10）</returns>
	public static Dictionary<EnumPetBaseStats, int> GenerateRandomTalentDict()
	{
		var dict = new Dictionary<EnumPetBaseStats, int>();
		var allStats = new List<EnumPetBaseStats>((EnumPetBaseStats[])Enum.GetValues(typeof(EnumPetBaseStats)));

		// 从 6 个属性中随机选出 3 个
		var selectedStats = new List<EnumPetBaseStats>();
		var pool = new List<EnumPetBaseStats>(allStats);
		for (int i = 0; i < 3 && pool.Count > 0; i++)
		{
			int index = RandomTool.Range(0, pool.Count - 1);
			selectedStats.Add(pool[index]);
			pool.RemoveAt(index);
		}

		// 为选中的 3 个属性随机分配天赋类型（偏向普通/一般般，小概率好/极品）
		foreach (var stat in selectedStats)
		{
			int talentType = RollRandomTalentType();
			dict[stat] = RollTalentValue(talentType);
		}

		return dict;
	}

	/// <summary>
	/// 传入指定天赋类型 + 随机生成个体项
	/// 从 6 个属性中随机选出 3 个，全部赋予指定的天赋类型
	/// </summary>
	/// <param name="talentType">指定的天赋类型</param>
	/// <returns>天赋值字典，3 个随机属性使用指定天赋类型</returns>
	public static Dictionary<EnumPetBaseStats, int> GenerateTalentDictByType(int talentType)
	{
		var dict = new Dictionary<EnumPetBaseStats, int>();
		var allStats = new List<EnumPetBaseStats>((EnumPetBaseStats[])Enum.GetValues(typeof(EnumPetBaseStats)));

		// 从 6 个属性中随机选出 3 个
		var pool = new List<EnumPetBaseStats>(allStats);
		for (int i = 0; i < 3 && pool.Count > 0; i++)
		{
			int index = RandomTool.Range(0, pool.Count - 1);
			dict[pool[index]] = RollTalentValue(talentType);
			pool.RemoveAt(index);
		}

		return dict;
	}

	/// <summary>
	/// 传入指定个体项 + 随机生成天赋
	/// 指定的属性使用随机天赋类型，其余属性天赋值为 0
	/// </summary>
	/// <param name="targetStats">指定的个体项列表（如 {HP, ATK, SPD}）</param>
	/// <returns>天赋值字典，指定属性随机天赋值</returns>
	public static Dictionary<EnumPetBaseStats, int> GenerateTalentDictByStats(List<EnumPetBaseStats> targetStats)
	{
		var dict = new Dictionary<EnumPetBaseStats, int>();
		foreach (var stat in targetStats)
		{
			int talentType = RollRandomTalentType();
			dict[stat] = RollTalentValue(talentType);
		}
		return dict;
	}

	/// <summary>
	/// 传入指定天赋 + 指定个体项
	/// 指定的属性全部使用指定的天赋类型
	/// </summary>
	/// <param name="targetStats">指定的个体项列表</param>
	/// <param name="talentType">指定的天赋类型</param>
	/// <returns>天赋值字典，指定属性使用指定天赋值</returns>
	public static Dictionary<EnumPetBaseStats, int> GenerateTalentDictByStatsAndType(List<EnumPetBaseStats> targetStats, int talentType)
	{
		var dict = new Dictionary<EnumPetBaseStats, int>();
		foreach (var stat in targetStats)
		{
			dict[stat] = RollTalentValue(talentType);
		}
		return dict;
	}

	/// <summary>
	/// 随机生成天赋类型
	/// 概率分布：Normal 25%, NormalPlus 30%, Good 25%, Excellent 20%
	/// 不包含 Zero，Zero 为手动指定
	/// </summary>
	private static int RollRandomTalentType()
	{
		int roll = RandomTool.Range(0, 99);
		if (roll < 25) return Normal;          // 0-24:  普通 (25%)
		if (roll < 55) return NormalPlus;      // 25-54: 一般般 (30%)
		if (roll < 80) return Good;            // 55-79: 好 (25%)
		return Excellent;                       // 80-99: 极品 (20%)
	}
}