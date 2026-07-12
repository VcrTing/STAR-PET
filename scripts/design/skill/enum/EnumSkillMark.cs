using Godot;

/// <summary>
/// 技能印记枚举
/// 对标洛克王国世界，精灵下场印记不消失
/// </summary>
public enum EnumSkillMark
{
	NONE = 0,     // 无印记
	GOLDEN = 1,   // 黄金印记
	POISON = 2,   // 中毒印记
	BUFF = 3,     // 增益印记
	DEBUFF = 4,   // 减益印记
}