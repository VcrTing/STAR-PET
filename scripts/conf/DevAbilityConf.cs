using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用精灵特性配置
/// 维护宠物 → 特性枚举的映射
/// </summary>
public static class DevAbilityConf
{
	/// <summary>
	/// 精灵 -> 特性映射
	/// 默认 0 号精灵特性为 None（无特性）
	/// </summary>
	private static readonly Dictionary<EnumPet, EnumPetAbility> PetAbilityMap = new()
	{
		{ EnumPet.Zero, EnumPetAbility.None },
		{ EnumPet.One, EnumPetAbility.None },
	};

	/// <summary>
	/// 获取指定精灵的特性
	/// </summary>
	public static EnumPetAbility GetAbility(EnumPet pet)
	{
		if (PetAbilityMap.TryGetValue(pet, out var ability))
			return ability;
		return EnumPetAbility.None;
	}

	/// <summary>
	/// 获取指定宠物编号的特性（兼容 int 调用）
	/// </summary>
	public static EnumPetAbility GetAbility(int petId)
	{
		return GetAbility((EnumPet)petId);
	}
}
