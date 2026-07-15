using Godot;
using System;

public partial class FightCanvasLayer : CanvasLayer
{
	public static FightCanvasLayer Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Node LoadScene(string scenePath)
	{
		var scene = GD.Load<PackedScene>(scenePath);
		if (scene == null)
		{
			GD.PrintErr($"  ⚠ 场景加载失败: {scenePath}");
			return null;
		}
		var instance = scene.Instantiate<Node>();
		AddChild(instance);
		return instance;
	}
}
