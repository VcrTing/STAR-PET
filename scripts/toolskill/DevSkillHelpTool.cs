using Godot;
using System;

/// <summary>
/// 技能辅助判断工具
/// 提供各类技能类型的判断方法
/// </summary>
public static class DevSkillHelpTool
{
	/// <summary>
	/// 切换宠物技能的 SkillId
	/// </summary>
	private const string SwitchPetSkillId = "0_4_1";

	/// <summary>
	/// 判断一个战斗技能是否为切换宠物技能
	/// </summary>
	/// <param name="fightSkill">战斗技能实例</param>
	/// <returns>true=是切换宠物技能</returns>
	public static bool IsSwitchPetSkill(InsFightSkill fightSkill)
	{
		if (fightSkill?.Skill == null)
			return false;

		return fightSkill.Skill.SkillId == SwitchPetSkillId;
	}
}