using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵对人类攻击等级设计（野外遭遇行为）
/// 提供各等级的中文名称查询
/// </summary>
public static class PetHostileLevelDesign
{
	/// <summary>
	/// 攻击等级中文名称映射
	/// </summary>
	private static readonly Dictionary<int, string> _hostileLevelData = new()
	{
		{ (int)EnumPetHostileLevel.Flee, "逃避" },
		{ (int)EnumPetHostileLevel.None, "不攻击" },
		{ (int)EnumPetHostileLevel.Passive, "不主动攻击" },
		{ (int)EnumPetHostileLevel.Aggressive, "近处攻击" },
	};

	/// <summary>
	/// 获取攻击等级的中文名称
	/// </summary>
	public static string GetHostileLevelName(int levelId)
	{
		if (_hostileLevelData.TryGetValue(levelId, out var name))
			return name;
		return $"Unknown({levelId})";
	}

	/// <summary>
	/// 获取攻击等级的中文名称
	/// </summary>
	public static string GetHostileLevelName(EnumPetHostileLevel level)
	{
		return GetHostileLevelName((int)level);
	}

	/// <summary>
	/// 获取所有攻击等级 ID 列表
	/// </summary>
	public static List<int> GetAllHostileLevelIds()
	{
		return new List<int>(_hostileLevelData.Keys);
	}
}