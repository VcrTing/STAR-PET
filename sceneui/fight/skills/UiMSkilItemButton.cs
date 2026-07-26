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
	}

	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 设置按钮可用状态
	/// true=禁用，false=启用
	/// </summary>
	/// <param name="disabled">是否禁用</param>
	private void SetButtonDisabled(bool disabled)
	{
		Disabled = disabled;
	}

	/// <summary>
	/// 点击处理：先同步技能栏数据，再判断玩家能否行动且 PP 充足，若满足则选择技能
	/// </summary>
	private void OnClick()
	{
		if (FightSkill?.Skill == null)
			return;

		// 点击前同步技能栏数据
		var myPet = FightLandMyStandPet.Instance?.FightPetData;
		if (myPet?.FightSkills != null && myPet.FightSkills.Count > 0)
		{
			UiHBoxSkillsManager.Instance?.UpdateSkills(myPet.FightSkills);
		}

		if (FightCenterManger.Instance.CanPlayerAct())
		{
			if (myPet != null && myPet.Pp >= FightSkill.ActualPpCost)
			{
				FightCenterManger.Instance.PlayerSelectSkill(FightSkill);
			}
		}
	}

	/// <summary>
	/// 根据战斗技能数据刷新 UI 显示
	/// 同时检查当前精灵 PP 是否足够，若不足则禁用按钮
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

		AsyncDisabled();
	}

	void AsyncDisabled()
	{
		// 检查当前精灵 PP 是否足够使用该技能
		var myPet = FightLandMyStandPet.Instance?.FightPetData;
		if (myPet != null)
		{
			bool insufficientPp = myPet.Pp < FightSkill.ActualPpCost;
			SetButtonDisabled(insufficientPp);
		}
	}
}
