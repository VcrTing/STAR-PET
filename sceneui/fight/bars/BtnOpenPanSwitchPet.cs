using Godot;
using System;

public partial class BtnOpenPanSwitchPet : TextureButton
{
	private Label _statusLabel;

	public override void _Ready()
	{
		_statusLabel = (Label) GodotTool.FindChildByName(this, "Label");
		Pressed += OnPressed;
	}

	public override void _Process(double delta)
	{
		if (_statusLabel == null) return;
		bool canSwitch = PlayerLandMyStandPlayer.Instance?.CanSwitchPet() ?? false;
		_statusLabel.Text = canSwitch ? "切换精灵" : "不可切换";
	}

	private void OnPressed()
	{
		if (!(PlayerLandMyStandPlayer.Instance?.CanSwitchPet() ?? false))
		{
			GD.Print("[BtnOpenPanSwitchPet] 当前不可切换宠物");
			return;
		}
		PanFightPlayerPack.Instance?.Open();
	}
}