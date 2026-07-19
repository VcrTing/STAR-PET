using Godot;
using System;

/// <summary>
/// 技能类型配置
/// </summary>
public static class SkillTypeDesign
{
	public const int ATTACK = (int)EnumSkillType.ATTACK;
	public const int DEFENSE = (int)EnumSkillType.DEFENSE;
	public const int STATUS = (int)EnumSkillType.STATUS;

	public static string GetName(int type)
	{
		return type switch
		{
			(int)EnumSkillType.ATTACK => "攻击",
			(int)EnumSkillType.DEFENSE => "防御",
			(int)EnumSkillType.STATUS => "状态",
			_ => $"未知({type})"
		};
	}

	public static string GetSkillTypeFolder(int skillType)
	{
		return skillType switch
		{
			(int)EnumSkillType.ATTACK => "attack",
			(int)EnumSkillType.DEFENSE => "defense",
			(int)EnumSkillType.STATUS => "status",
			_ => "attack"
		};
	}

	/// <summary>
	/// 路径格式：res://define/dataskill/{petTypeFolder}/{skillTypeFolder}/
	/// </summary>
	public static string GetSkillFolderPath(int petType, int skillType)
	{
		string petTypeFolder = petType switch
		{
			(int)EnumPetType.Normal => "Normal",
			(int)EnumPetType.Water => "Water",
			(int)EnumPetType.Fire => "Fire",
			(int)EnumPetType.Grass => "Grass",
			(int)EnumPetType.Earth => "Earth",
			(int)EnumPetType.Gold => "Gold",
			(int)EnumPetType.Ice => "Ice",
			(int)EnumPetType.Electric => "Electric",
			(int)EnumPetType.Fairy => "Fairy",
			(int)EnumPetType.Dragon => "Dragon",
			(int)EnumPetType.Ghost => "Ghost",
			(int)EnumPetType.Fighting => "Fighting",
			_ => "Normal"
		};
		string skillTypeFolder = GetSkillTypeFolder(skillType);
		return $"res://define/dataskill/{petTypeFolder}/{skillTypeFolder}/";
	}
}