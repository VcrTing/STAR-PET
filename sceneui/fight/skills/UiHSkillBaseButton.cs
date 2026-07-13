using Godot;
using System;

/// <summary>
/// 基础技能按钮（固定技能，不切换）
/// 默认加载 0_3_1 聚能技能
/// </summary>
public partial class UiHSkillBaseButton : TextureButton
{
	/// <summary>
	/// 关联的战斗技能数据
	/// </summary>
	public InsFightSkill FightSkill { get; private set; }

	public override void _Ready()
	{
		// 默认加载聚能技能（0_3_1）
		var skillIds = new[] { "0_3_1" };
		var skills = DevSkillLoadTool.LoadSkills(skillIds);
		if (skills.Count > 0)
		{
			FightSkill = InsFightSkill.FromInsSkill(skills[0]);
		}

		// 点击打印技能信息
		Pressed += () =>
		{
			if (FightSkill?.Skill != null)
			{
				var s = FightSkill.Skill;
				GD.Print($"[UiHSkillBaseButton] 点击技能: {s.SkillId} {s.SkillName}, 威力={FightSkill.ActualAttackValue}(显示={FightSkill.DisplayAttackValue}), 能耗={FightSkill.ActualPpCost}, 冻结={FightSkill.IsFrozen}, 冷却={FightSkill.CooldownTurns}");
			}
			else
			{
				GD.PrintErr("[UiHSkillBaseButton] 点击技能但 FightSkill 为空");
			}
		};
	}

	public override void _Process(double delta)
	{
	}
}