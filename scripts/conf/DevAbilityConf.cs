using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用精灵特性配置
/// 维护宠物编号 → 特性枚举的映射
/// </summary>
public static class DevAbilityConf
{
	/// <summary>
	/// 宠物编号 -> 特性映射
	/// 默认 0 号精灵特性为 None（无特性）
	/// </summary>
	private static readonly Dictionary<int, EnumPetAbility> PetAbilityMap = new()
	{
		{ 0, EnumPetAbility.None },
	};

	/// <summary>
	/// 获取指定宠物编号的特性
	/// </summary>
	public static EnumPetAbility GetAbility(int petId)
	{
		if (PetAbilityMap.TryGetValue(petId, out var ability))
			return ability;
		return EnumPetAbility.None;
	}
}