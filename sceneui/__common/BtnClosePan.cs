using Godot;
using System;

public partial class BtnClosePan : TextureButton
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
		switch (PanName)
		{
			case EnumPanName.FightPetPack:
				PanFightPlayerPack.Instance?.Close();
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
