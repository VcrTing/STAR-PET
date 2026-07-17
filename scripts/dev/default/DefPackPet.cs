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
	/// 测试模式下的默认上阵精灵列表
	/// 加载零号精灵（金系）和一号精灵（地系）作为默认精灵
	/// </summary>
	public static List<InsPackPetData> testModePackPets()
	{
		var zero = DevPackPetGeneraTool.InitSpecialStonePet(EnumPet.Zero, EnumPetType.Gold);
		// var one = DevPackPetTool.LoadAndSync(EnumPet.One, EnumPetType.Earth, out _);
		return new List<InsPackPetData> { zero };
	}
}