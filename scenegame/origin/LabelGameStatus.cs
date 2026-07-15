using Godot;
using System;

public partial class LabelGameStatus : Label
{
	public static LabelGameStatus Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public static void SetText(string text)
	{
		if (Instance != null)
		{
			Instance.Text = text;
		}
	}
}
