using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵特性配置
/// 特性映射存储于 DevAbilityConf，此处仅提供特性中文名称等查询方法
/// </summary>
public static class PetAbilityDesign
{
	/// <summary>
	/// 特性中文名称映射
	/// </summary>
	private static readonly Dictionary<int, string> _abilityNameMap = new()
	{
		{ (int)EnumPetAbility.None, "无特性" },
		{ (int)EnumPetAbility.ZeroPet, "零号精灵" },
	};

	/// <summary>
	/// 获取指定宠物编号的特性（委托 DevAbilityConf）
	/// </summary>
	public static EnumPetAbility GetAbility(int petId)
	{
		return DevAbilityConf.GetAbility(petId);
	}

	/// <summary>
	/// 获取特性的中文名称
	/// </summary>
	public static string GetAbilityName(EnumPetAbility ability)
	{
		return GetAbilityName((int)ability);
	}

	/// <summary>
	/// 获取特性的中文名称
	/// </summary>
	public static string GetAbilityName(int abilityId)
	{
		if (_abilityNameMap.TryGetValue(abilityId, out var name))
			return name;
		return $"Unknown({abilityId})";
	}

	/// <summary>
	/// 获取所有特性 ID 列表
	/// </summary>
	public static List<int> GetAllAbilityIds()
	{
		return new List<int>(_abilityNameMap.Keys);
	}
}
