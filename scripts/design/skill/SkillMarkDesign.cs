using Godot;
using System;

/// <summary>
/// 技能印记配置
/// 对标洛克王国世界，精灵下场印记不消失
/// 印记类型：黄金、中毒、增益、减益
/// </summary>
public static class SkillMarkDesign
{
	// const int 常量已提取到 EnumSkillMark 枚举，见 scripts/design/skill/enum/EnumSkillMark.cs
	public const int NONE = (int)EnumSkillMark.NONE;       // 无印记
	public const int GOLDEN = (int)EnumSkillMark.GOLDEN;   // 黄金印记
	public const int POISON = (int)EnumSkillMark.POISON;   // 中毒印记
	public const int BUFF = (int)EnumSkillMark.BUFF;       // 增益印记
	public const int DEBUFF = (int)EnumSkillMark.DEBUFF;   // 减益印记

	/// <summary>
	/// 获取印记中文名称
	/// </summary>
	public static string GetName(int mark)
	{
		return mark switch
		{
			(int)EnumSkillMark.NONE => "无",
			(int)EnumSkillMark.GOLDEN => "黄金印记",
			(int)EnumSkillMark.POISON => "中毒印记",
			(int)EnumSkillMark.BUFF => "增益印记",
			(int)EnumSkillMark.DEBUFF => "减益印记",
			_ => $"未知({mark})"
		};
	}

	/// <summary>
	/// 是否为负面印记
	/// </summary>
	public static bool IsNegative(int mark)
	{
		return mark switch
		{
			(int)EnumSkillMark.POISON => true,
			(int)EnumSkillMark.DEBUFF => true,
			_ => false
		};
	}

	/// <summary>
	/// 是否为正面印记
	/// </summary>
	public static bool IsPositive(int mark)
	{
		return mark switch
		{
			(int)EnumSkillMark.GOLDEN => true,
			(int)EnumSkillMark.BUFF => true,
			_ => false
		};
	}
}