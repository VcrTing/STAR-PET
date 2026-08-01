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
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}