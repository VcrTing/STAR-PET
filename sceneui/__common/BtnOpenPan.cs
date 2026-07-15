using Godot;
using System;

public partial class BtnOpenPan : TextureButton
{
	[Export]
	public EnumPanName PanName { get; set; } = EnumPanName.None;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Pressed += OnClick;
	}

	private void OnClick()
	{
		GD.Print($"  📂 打开面板: {PanName}");

		switch (PanName)
		{
			case EnumPanName.FightPetPack:
				if (PanFightPlayerPack.Instance == null)
					GD.PrintErr("  ❌ PanFightPlayerPack.Instance 为 null！请在场景中添加 PanFightPlayerPack 节点");
				else
					PanFightPlayerPack.Instance.Open();
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}