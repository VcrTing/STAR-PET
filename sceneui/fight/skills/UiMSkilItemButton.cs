using Godot;
using System;

public partial class UiMSkilItemButton : TextureButton
{
	/// <summary>
	/// 关联的战斗技能数据
	/// </summary>
	public InsFightSkill FightSkill { get; private set; }

	private Label _labelSkillName;
	private Label _labelSkillDamage;
	private Label _labelSkillCost;

	public override void _Ready()
	{
		Pressed += OnClick;

		// 鼠标悬停：淡蓝色边框发光效果
		var borderBox = new StyleBoxFlat();
		borderBox.BgColor = Colors.Transparent;
		borderBox.BorderWidthLeft = 2;
		borderBox.BorderWidthRight = 2;
		borderBox.BorderWidthTop = 2;
		borderBox.BorderWidthBottom = 2;
		borderBox.CornerRadiusTopLeft = 6;
		borderBox.CornerRadiusTopRight = 6;
		borderBox.CornerRadiusBottomLeft = 6;
		borderBox.CornerRadiusBottomRight = 6;

		// 正常状态：透明边框
		var normalBox = (StyleBoxFlat)borderBox.Duplicate();
		normalBox.BorderColor = Colors.Transparent;
		AddThemeStyleboxOverride("normal", normalBox);

		// 悬停状态：淡蓝色发光边框
		var hoverBox = (StyleBoxFlat)borderBox.Duplicate();
		hoverBox.BorderColor = new Color(0.5f, 0.8f, 1.0f);
		AddThemeStyleboxOverride("hover", hoverBox);

		// 额外：按下状态稍微亮一点
		var pressedBox = (StyleBoxFlat)borderBox.Duplicate();
		pressedBox.BorderColor = new Color(0.3f, 0.6f, 0.9f);
		AddThemeStyleboxOverride("pressed", pressedBox);
	}

	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 点击处理：判断玩家能否行动，若能则选择技能
	/// </summary>
	private void OnClick()
	{
		if (FightSkill?.Skill == null)
			return;

		if (FightCenterManger.Instance.CanPlayerAct())
		{
			FightCenterManger.Instance.PlayerSelectSkill(FightSkill);
		}
	}

	/// <summary>
	/// 根据战斗技能数据刷新 UI 显示
	/// </summary>
	/// <param name="fightSkill">战斗技能数据</param>
	public void Refresh(InsFightSkill fightSkill)
	{
		if (fightSkill?.Skill == null)
			return;

		FightSkill = fightSkill;

		var skill = fightSkill.Skill;

		// 懒加载：只在首次查询，后续复用缓存
		_labelSkillName ??= FindChild("LabelSkillName", true, false) as Label;
		_labelSkillDamage ??= FindChild("LabelSkillDamage", true, false) as Label;
		_labelSkillCost ??= FindChild("LabelSkillCost", true, false) as Label;

		if (_labelSkillName != null)
			_labelSkillName.Text = skill.SkillName;

		if (_labelSkillDamage != null)
			_labelSkillDamage.Text = fightSkill.DisplayAttackValue.ToString();

		if (_labelSkillCost != null)
			_labelSkillCost.Text = fightSkill.ActualPpCost.ToString();
	}
}
