using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵飞行/移动方式设计
/// 记录五种移动方式的英文类别文字
/// </summary>
public static class PetFlayDesign
{
	public const int PsychicFlight = (int)EnumPetFly.PsychicFlight; // 超能力飞行
	public const int WingFlight = (int)EnumPetFly.WingFlight;       // 翅膀飞行
	public const int Float = (int)EnumPetFly.Float;                 // 漂浮
	public const int Walk = (int)EnumPetFly.Walk;                   // 走路
	public const int Dive = (int)EnumPetFly.Dive;                   // 潜水
	public const int Burrow = (int)EnumPetFly.Burrow;               // 遁地

	/// <summary>
	/// 移动方式中文名称映射
	/// </summary>
	private static readonly Dictionary<int, string> _flyData = new()
	{
		{ (int)EnumPetFly.PsychicFlight, "超能力飞行" },
		{ (int)EnumPetFly.WingFlight, "翅膀飞行" },
		{ (int)EnumPetFly.Float, "漂浮" },
		{ (int)EnumPetFly.Walk, "走路" },
		{ (int)EnumPetFly.Dive, "潜水" },
		{ (int)EnumPetFly.Burrow, "遁地" },
	};

	/// <summary>
	/// 获取移动方式的中文名称
	/// </summary>
	public static string GetFlyName(int flyId)
	{
		if (_flyData.TryGetValue(flyId, out var name))
			return name;
		return $"Unknown({flyId})";
	}

	/// <summary>
	/// 获取所有移动方式 ID 列表
	/// </summary>
	public static List<int> GetAllFlyIds()
	{
		return new List<int>(_flyData.Keys);
	}
}