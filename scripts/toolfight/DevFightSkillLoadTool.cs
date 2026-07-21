using Godot;
using System;

/// <summary>
/// 战斗技能加载工具
/// 提供创建战斗技能实例（InsFightSkill）的便捷方法
/// </summary>
public static class DevFightSkillLoadTool
{
	/// <summary>
	/// 加载聚能技能（状态技能，本回合不攻击，下回合提升技能威力）
	/// 读取 res://define/dataskill/Normal/status/0_3_1.gd 并包装为战斗技能实例
	/// </summary>
	/// <returns>聚能技能的 InsFightSkill 实例，加载失败返回 null</returns>
	public static InsFightSkill LoadChargeSkill()
	{
		InsSkill skill = DevSkillLoadTool.LoadChargeSkill();
		if (skill == null)
			return null;

		return InsFightSkill.FromInsSkill(skill);
	}
}