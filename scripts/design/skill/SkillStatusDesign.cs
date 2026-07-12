using Godot;
using System;

/// <summary>
/// 技能异常状态配置
/// </summary>
public static class SkillStatusDesign
{
	// const int 常量已提取到 EnumSkillStatus 枚举，见 scripts/design/skill/enum/EnumSkillStatus.cs
	public const int NONE = (int)EnumSkillStatus.NONE;       // 无
	public const int POISON = (int)EnumSkillStatus.POISON;   // 中毒
	public const int BLEED = (int)EnumSkillStatus.BLEED;     // 流血
	public const int FREEZE = (int)EnumSkillStatus.FREEZE;   // 冰冻
	public const int BURN = (int)EnumSkillStatus.BURN;       // 灼烧
	public const int SLEEP = (int)EnumSkillStatus.SLEEP;     // 睡眠

	/// <summary>
	/// 获取异常状态中文名称
	/// </summary>
	public static string GetName(int status)
	{
		return status switch
		{
			(int)EnumSkillStatus.NONE => "无",
			(int)EnumSkillStatus.POISON => "中毒",
			(int)EnumSkillStatus.BLEED => "流血",
			(int)EnumSkillStatus.FREEZE => "冰冻",
			(int)EnumSkillStatus.BURN => "灼烧",
			(int)EnumSkillStatus.SLEEP => "睡眠",
			_ => $"未知({status})"
		};
	}
}