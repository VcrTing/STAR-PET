using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵性格设计，完全照搬宝可梦 25 种性格
/// 每种种性格影响两项种族值：一项 +10%，一项 -10%（或五种种无影响）
/// 种族值索引使用 PetBaseStatsDesign
/// </summary>
public static class PetNatureDesign
{
	// const int 常量已提取到 EnumPetNature 枚举，见 scripts/design/pet/enum/EnumPetNature.cs
	public const int Hardy = (int)EnumPetNature.Hardy;
	public const int Lonely = (int)EnumPetNature.Lonely;
	public const int Brave = (int)EnumPetNature.Brave;
	public const int Adamant = (int)EnumPetNature.Adamant;
	public const int Naughty = (int)EnumPetNature.Naughty;
	public const int Bold = (int)EnumPetNature.Bold;
	public const int Docile = (int)EnumPetNature.Docile;
	public const int Relaxed = (int)EnumPetNature.Relaxed;
	public const int Impish = (int)EnumPetNature.Impish;
	public const int Lax = (int)EnumPetNature.Lax;
	public const int Timid = (int)EnumPetNature.Timid;
	public const int Hasty = (int)EnumPetNature.Hasty;
	public const int Serious = (int)EnumPetNature.Serious;
	public const int Jolly = (int)EnumPetNature.Jolly;
	public const int Naive = (int)EnumPetNature.Naive;
	public const int Modest = (int)EnumPetNature.Modest;
	public const int Mild = (int)EnumPetNature.Mild;
	public const int Quiet = (int)EnumPetNature.Quiet;
	public const int Rash = (int)EnumPetNature.Rash;
	public const int Calm = (int)EnumPetNature.Calm;
	public const int Gentle = (int)EnumPetNature.Gentle;
	public const int Careful = (int)EnumPetNature.Careful;
	public const int Quirky = (int)EnumPetNature.Quirky;
	public const int Sassy = (int)EnumPetNature.Sassy;
	public const int Bashful = (int)EnumPetNature.Bashful;

	// 性格数据：提升项、降低项（无影响 = -1）
	private static readonly Dictionary<int, (int up, int down)> _natureData = new()
	{
		{ (int)EnumPetNature.Hardy,   ( -1, -1 ) },
		{ (int)EnumPetNature.Lonely,  ( (int)EnumPetBaseStats.ATK, (int)EnumPetBaseStats.DEF ) },
		{ (int)EnumPetNature.Brave,   ( (int)EnumPetBaseStats.ATK, (int)EnumPetBaseStats.SPD ) },
		{ (int)EnumPetNature.Adamant, ( (int)EnumPetBaseStats.ATK, (int)EnumPetBaseStats.MATK ) },
		{ (int)EnumPetNature.Naughty, ( (int)EnumPetBaseStats.ATK, (int)EnumPetBaseStats.MDEF ) },
		{ (int)EnumPetNature.Bold,    ( (int)EnumPetBaseStats.DEF, (int)EnumPetBaseStats.ATK ) },
		{ (int)EnumPetNature.Docile,  ( -1, -1 ) },
		{ (int)EnumPetNature.Relaxed, ( (int)EnumPetBaseStats.DEF, (int)EnumPetBaseStats.SPD ) },
		{ (int)EnumPetNature.Impish,  ( (int)EnumPetBaseStats.DEF, (int)EnumPetBaseStats.MATK ) },
		{ (int)EnumPetNature.Lax,     ( (int)EnumPetBaseStats.DEF, (int)EnumPetBaseStats.MDEF ) },
		{ (int)EnumPetNature.Timid,   ( (int)EnumPetBaseStats.SPD, (int)EnumPetBaseStats.ATK ) },
		{ (int)EnumPetNature.Hasty,   ( (int)EnumPetBaseStats.SPD, (int)EnumPetBaseStats.DEF ) },
		{ (int)EnumPetNature.Serious, ( -1, -1 ) },
		{ (int)EnumPetNature.Jolly,   ( (int)EnumPetBaseStats.SPD, (int)EnumPetBaseStats.MATK ) },
		{ (int)EnumPetNature.Naive,   ( (int)EnumPetBaseStats.SPD, (int)EnumPetBaseStats.MDEF ) },
		{ (int)EnumPetNature.Modest,  ( (int)EnumPetBaseStats.MATK, (int)EnumPetBaseStats.ATK ) },
		{ (int)EnumPetNature.Mild,    ( (int)EnumPetBaseStats.MATK, (int)EnumPetBaseStats.DEF ) },
		{ (int)EnumPetNature.Quiet,   ( (int)EnumPetBaseStats.MATK, (int)EnumPetBaseStats.SPD ) },
		{ (int)EnumPetNature.Rash,    ( (int)EnumPetBaseStats.MATK, (int)EnumPetBaseStats.MDEF ) },
		{ (int)EnumPetNature.Calm,    ( (int)EnumPetBaseStats.MDEF, (int)EnumPetBaseStats.ATK ) },
		{ (int)EnumPetNature.Gentle,  ( (int)EnumPetBaseStats.MDEF, (int)EnumPetBaseStats.DEF ) },
		{ (int)EnumPetNature.Careful, ( (int)EnumPetBaseStats.MDEF, (int)EnumPetBaseStats.MATK ) },
		{ (int)EnumPetNature.Quirky,  ( -1, -1 ) },
		{ (int)EnumPetNature.Sassy,   ( (int)EnumPetBaseStats.MDEF, (int)EnumPetBaseStats.SPD ) },
		{ (int)EnumPetNature.Bashful, ( -1, -1 ) },
	};

	/// <summary>
	/// 获取性格中文名称
	/// </summary>
	public static string GetName(int nature)
	{
		return nature switch
		{
			(int)EnumPetNature.Hardy => "勤奋",
			(int)EnumPetNature.Lonely => "孤独",
			(int)EnumPetNature.Brave => "勇敢",
			(int)EnumPetNature.Adamant => "固执",
			(int)EnumPetNature.Naughty => "调皮",
			(int)EnumPetNature.Bold => "大胆",
			(int)EnumPetNature.Docile => "坦率",
			(int)EnumPetNature.Relaxed => "悠闲",
			(int)EnumPetNature.Impish => "淘气",
			(int)EnumPetNature.Lax => "乐天",
			(int)EnumPetNature.Timid => "胆小",
			(int)EnumPetNature.Hasty => "急躁",
			(int)EnumPetNature.Serious => "认真",
			(int)EnumPetNature.Jolly => "爽朗",
			(int)EnumPetNature.Naive => "天真",
			(int)EnumPetNature.Modest => "内敛",
			(int)EnumPetNature.Mild => "冷静",
			(int)EnumPetNature.Quiet => "慢吞吞",
			(int)EnumPetNature.Rash => "马虎",
			(int)EnumPetNature.Calm => "温和",
			(int)EnumPetNature.Gentle => "温顺",
			(int)EnumPetNature.Careful => "慎重",
			(int)EnumPetNature.Quirky => "浮躁",
			(int)EnumPetNature.Sassy => "自大",
			(int)EnumPetNature.Bashful => "害羞",
			_ => $"未知({nature})"
		};
	}

	/// <summary>
	/// 获取性格提升的种族值类型，无影响返回 -1
	/// </summary>
	public static int GetUpStat(int nature)
	{
		if (_natureData.TryGetValue(nature, out var data))
			return data.up;
		return -1;
	}

	/// <summary>
	/// 获取性格降低的种族值类型，无影响返回 -1
	/// </summary>
	public static int GetDownStat(int nature)
	{
		if (_natureData.TryGetValue(nature, out var data))
			return data.down;
		return -1;
	}

	/// <summary>
	/// 计算性格修正系数：提升项 ×1.1，降低项 ×0.9，其余 ×1.0
	/// </summary>
	/// <param name="nature">性格</param>
	/// <param name="statType">种族值类型（PetBaseStatsDesign）</param>
	public static float GetModifier(int nature, int statType)
	{
		if (GetUpStat(nature) == statType) return 1.1f;
		if (GetDownStat(nature) == statType) return 0.9f;
		return 1.0f;
	}

	/// <summary>
	/// 获取性格描述，如 "固执（+物攻 -魔攻）"
	/// </summary>
	public static string GetDescription(int nature)
	{
		int up = GetUpStat(nature);
		int down = GetDownStat(nature);

		if (up == -1) return $"{GetName(nature)}（无影响）";

		return $"{GetName(nature)}（+{PetBaseStatsDesign.GetName(up)} -{PetBaseStatsDesign.GetName(down)}）";
	}
}