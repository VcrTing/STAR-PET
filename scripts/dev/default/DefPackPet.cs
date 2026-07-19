using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发默认精灵配置
/// 测试模式下加载的默认精灵列表
/// </summary>
public static class DefPackPet
{
	/// <summary>
	/// 测试模式下的我方默认上阵精灵列表
	/// 加载零号精灵（金系）作为默认精灵
	/// </summary>
	public static List<InsPackPetData> testModeMyPackPets()
	{
		var zero = DevPackPetGeneraTool.InitSpecialStonePet(EnumPet.Zero, EnumPetType.Gold);
		return new List<InsPackPetData> { zero };
	}

	/// <summary>
	/// 测试模式下的敌方默认上阵精灵列表
	/// 加载一号精灵（地系）作为默认精灵
	/// </summary>
	public static List<InsPackPetData> testModeYouPackPets()
	{
		var one = DevPackPetGeneraTool.InitSpecialStonePet(EnumPet.One, EnumPetType.Earth);
		return new List<InsPackPetData> { one };
	}

	/// <summary>
	/// 兼容旧调用，默认返回我方列表
	/// </summary>
	public static List<InsPackPetData> testModePackPets()
	{
		return testModeMyPackPets();
	}
}