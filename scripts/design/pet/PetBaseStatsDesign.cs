using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵种族值类别配置
/// </summary>
public static class PetBaseStatsDesign
{
	// const int 常量已提取到 EnumPetBaseStats 枚举，见 scripts/design/pet/enum/EnumPetBaseStats.cs
	public const int HP = (int)EnumPetBaseStats.HP;     // 生命
	public const int ATK = (int)EnumPetBaseStats.ATK;   // 物攻
	public const int MATK = (int)EnumPetBaseStats.MATK; // 魔攻
	public const int DEF = (int)EnumPetBaseStats.DEF;   // 物防
	public const int MDEF = (int)EnumPetBaseStats.MDEF; // 魔防
	public const int SPD = (int)EnumPetBaseStats.SPD;   // 速度

	/// <summary>
	/// 从 pet_xxxx.gd 的 base_stats 字典数据转换为 Iv 字典
	/// base_stats 格式：{ 1: 90, 2: 100, ... }，key 为 EnumPetBaseStats 的 int 值
	/// </summary>
	/// <param name="baseStatsDict">从 Resource.Get("base_stats") 获取的 Godot Dictionary</param>
	/// <returns>stat -> value 的 Iv 字典</returns>
	public static Dictionary<EnumPetBaseStats, int> BaseStatsToIvDict(Godot.Collections.Dictionary baseStatsDict)
	{
		var ivDict = new Dictionary<EnumPetBaseStats, int>();
		if (baseStatsDict == null)
			return ivDict;

		foreach (var key in baseStatsDict.Keys)
		{
			int statId = int.Parse(key.AsString());
			int statValue = (int)baseStatsDict[key];
			EnumPetBaseStats statEnum = (EnumPetBaseStats)statId;
			ivDict[statEnum] = statValue;
		}
		return ivDict;
	}

	/// <summary>
	/// 获取种族值中文名称
	/// </summary>
	public static string GetName(int stat)
	{
		return stat switch
		{
			(int)EnumPetBaseStats.HP => "生命",
			(int)EnumPetBaseStats.ATK => "物攻",
			(int)EnumPetBaseStats.MATK => "魔攻",
			(int)EnumPetBaseStats.DEF => "物防",
			(int)EnumPetBaseStats.MDEF => "魔防",
			(int)EnumPetBaseStats.SPD => "速度",
			_ => $"未知({stat})"
		};
	}
}