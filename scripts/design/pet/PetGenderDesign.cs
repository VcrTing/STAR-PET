using Godot;
using System;

/// <summary>
/// 精灵性别设计
/// </summary>
public static class PetGenderDesign
{
	// const int 常量已提取到 EnumPetGender 枚举，见 scripts/design/pet/enum/EnumPetGender.cs
	public const int Male = (int)EnumPetGender.Male;
	public const int Female = (int)EnumPetGender.Female;

	/// <summary>
	/// 获取性别中文名称
	/// </summary>
	public static string GetName(int gender)
	{
		return gender switch
		{
			(int)EnumPetGender.Male => "雄",
			(int)EnumPetGender.Female => "雌",
			_ => "无性别"
		};
	}

	/// <summary>
	/// 获取性别图标路径，如 res://IMG/uigame/sex/ui_sex1.png
	/// </summary>
	public static string GetIconPath(int gender)
	{
		return $"res://IMG/uigame/sex/ui_sex{gender}.png";
	}

	/// <summary>
	/// 加载性别图标纹理
	/// </summary>
	public static Texture2D GetIconTexture(int gender)
	{
		return GD.Load<Texture2D>(GetIconPath(gender));
	}
}