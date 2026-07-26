using Godot;
using System;

/// <summary>
/// 基础技能按钮（固定技能，不切换）
/// 默认加载 0_3_1 聚能技能
/// </summary>
public partial class UiHSkillBaseButton : TextureButton
{
	private static UiHSkillBaseButton _instance;
	public static UiHSkillBaseButton Instance => _instance;

	/// <summary>
	/// 关联的战斗技能数据
	/// </summary>
	public InsFightSkill FightSkill { get; private set; }

	private Label _labelPp;

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
		// 缓存 LabelPp 引用
		_labelPp = FindChild("LabelPp", true, false) as Label;

		// 默认加载聚能技能（0_3_1）
		var skillIds = new[] { "0_3_1" };
		var skills = DevSkillLoadTool.LoadSkills(skillIds);
		if (skills.Count > 0)
		{
			FightSkill = InsFightSkill.FromInsSkill(skills[0]);
		}

		// 绑定点击事件
		Pressed += OnClick;
	}

	/// <summary>
	/// 点击处理：玩家可用时选择聚能技能
	/// </summary>
	private void OnClick()
	{
		if (FightSkill?.Skill == null)
		{
			GD.PrintErr("[UiHSkillBaseButton] 点击技能但 FightSkill 为空");
			return;
		}

		if (FightCenterManger.Instance.CanPlayerAct())
		{
			FightCenterManger.Instance.PlayerSelectSkill(FightSkill);
		}
	}

	/// <summary>
	/// 更新 PP 显示
	/// 显示格式：Pp / MaxPp（最大 PP 值来自 FightGameInit.MaxPpMy）
	/// </summary>
	/// <param name="petData">当前精灵数据</param>
	public void Update(InsFightPetData petData)
	{
		if (_labelPp == null)
		{
			_labelPp = FindChild("LabelPp", true, false) as Label;
			if (_labelPp == null) return;
		}

		if (petData != null)
		{
			_labelPp.Text = $"{petData.Pp} / {FightGameInit.MaxPpMy}";
		}
	}

	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}
