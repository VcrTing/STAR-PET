using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 技能UI管理器（单例）
/// 管理底部技能栏的显示与切换
/// </summary>
public partial class UiHBoxSkillsManager : HBoxContainer
{
	private static UiHBoxSkillsManager _instance;
	public static UiHBoxSkillsManager Instance => _instance;

	public override void _EnterTree()
	{
		if (_instance != null)
		{
			QueueFree();
			return;
		}
		_instance = this;
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 切换显示的技能列表
	/// 先清除旧的技能项，再根据传入的 InsFightSkill 数组生成新的技能按钮并刷新UI数据
	/// </summary>
	/// <param name="fightSkills">战斗技能数组</param>
	public void SwitchSkills(List<InsFightSkill> fightSkills)
	{
		// 1. 清除旧的技能项
		foreach (Node child in GetChildren())
		{
			child.QueueFree();
		}

		if (fightSkills == null || fightSkills.Count == 0)
			return;

		// 2. 加载技能场景模板
		var scene = GD.Load<PackedScene>("res://sceneui/fight/skills/ui_m_skill_item_wrapper.tscn");

		// 3. 为每个技能生成实例、添加到容器并刷新UI
		foreach (var fightSkill in fightSkills)
		{
			var skillItem = scene.Instantiate<MarginContainer>();
			AddChild(skillItem);

			// 查找其中的 UiMSkilItemButton 并刷新技能数据
			var btn = skillItem.FindChild("UiMSkilItemButton", true, false) as UiMSkilItemButton;
			if (btn != null)
			{
				btn.Refresh(fightSkill);
			}
			else
			{
				GD.PrintErr($"[UiHBoxSkillsManager] 未找到 UiMSkilItemButton");
			}
		}
	}

	public override void _ExitTree()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}