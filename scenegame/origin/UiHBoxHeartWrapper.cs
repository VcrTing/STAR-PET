using Godot;
using System;

/// <summary>
/// 血量UI管理器（单例）
/// 实时同步当前上场精灵的 HP / MaxHp 数值显示
/// </summary>
public partial class UiHBoxHeartWrapper : VBoxContainer
{
	private static UiHBoxHeartWrapper _instance;
	public static UiHBoxHeartWrapper Instance => _instance;

	// 子节点引用（在场景树中通过 _Ready 自动查找）
	private Label _hpLabel;
	private TextureProgressBar _hpBar;
	private Label _hpText;

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
		// 查找子节点：期望场景中包含名为 "HpBar" 的 TextureProgressBar 和 "HpLabel" 的 Label
		_hpBar = FindChild("HpBar", true, false) as TextureProgressBar;
		_hpText = FindChild("HpLabel", true, false) as Label;
		if (_hpText == null)
		{
			// 如果没有 HpLabel，尝试找通用标签
			_hpLabel = FindChild("Label", true, false) as Label;
		}
	}

	public override void _Process(double delta)
	{
		// 每帧从 FightLandMyStandPet 同步当前宠物的血量
		var fightPet = FightLandMyStandPet.Instance?.FightPetData;
		if (fightPet == null)
			return;

		int hp = fightPet.Hp;
		int maxHp = fightPet.MaxHp;
		if (maxHp <= 0)
			maxHp = 1; // 防止除零

		// 更新血条（TextureProgressBar）
		if (_hpBar != null)
		{
			_hpBar.MaxValue = maxHp;
			_hpBar.Value = hp;
		}

		// 更新文本显示 "HP / MaxHP"
		if (_hpText != null)
		{
			_hpText.Text = $"{hp} / {maxHp}";
		}
		else if (_hpLabel != null)
		{
			_hpLabel.Text = $"{hp} / {maxHp}";
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