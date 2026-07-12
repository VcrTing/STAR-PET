using Godot;
using System;

/// <summary>
/// 技能类型配置
/// </summary>
public static class SkillTypeDesign
{
	// const int 常量已提取到 EnumSkillType 枚举，见 scripts/design/skill/enum/EnumSkillType.cs
	public const int ATTACK = (int)EnumSkillType.ATTACK;   // 攻击
	public const int DEFENSE = (int)EnumSkillType.DEFENSE; // 防御
	public const int STATUS = (int)EnumSkillType.STATUS;   // 状态

	/// <summary>
	/// 获取技能类型中文名称
	/// </summary>
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

	/// <summary>
	/// 获取技能类型对应的 dataskill/ 子文件夹名（attack / defense / status）
	/// </summary>
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
	/// 根据精灵系别 + 技能类型，返回技能所在文件夹路径
	/// 路径格式：res://dataskill/{petTypeFolder}/{skillTypeFolder}/
	/// 与 res://dataskill/ 下文件夹名称联动
	/// </summary>
	/// <param name="petType">精灵系别（0=普通系，1-11对应 EnumPetType）</param>
	/// <param name="skillType">技能类型（1=攻击, 2=防御, 3=状态）</param>
	/// <returns>如 res://dataskill/Dragon/attack/</returns>
	public static string GetSkillFolderPath(int petType, int skillType)
	{
		string petTypeFolder = petType switch
		{
			(int)EnumPetType.Normal => "Normal",   // 普通系
			(int)EnumPetType.Water => "Water",
			(int)EnumPetType.Fire => "Fire",
			(int)EnumPetType.Grass => "Grass",
			(int)EnumPetType.Earth => "Earth",
			(int)EnumPetType.Gold => "Gold",     // 对应 dataskill/ 下的文件夹，需与 EnumPetType.Gold 同步
			(int)EnumPetType.Ice => "Ice",
			(int)EnumPetType.Electric => "Electric",
			(int)EnumPetType.Fairy => "Fairy",
			(int)EnumPetType.Dragon => "Dragon",
			(int)EnumPetType.Ghost => "Ghost",
			(int)EnumPetType.Fighting => "Fighting",
			_ => "Normal"
		};

		string skillTypeFolder = GetSkillTypeFolder(skillType);

		return $"res://dataskill/{petTypeFolder}/{skillTypeFolder}/";
	}
}