using Godot;
using System;

public partial class PetBallBody : RigidBody2D
{
	/// <summary>
	/// 当前精灵球种类（1=普通球，2=恩赐球，3=黄金球，4=炫彩球）
	/// </summary>
	public int BallType { get; private set; } = PetBallDesign.Normal;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 初始化精灵球，朝指定方向扔出
	/// </summary>
	public void Init(Vector2 direction, int ballType)
	{
		BallType = ballType;

		// 设置初始速度
		LinearVelocity = direction.Normalized() * 500f;
		GD.Print($"PetBall 扔出，种类: {GetBallTypeName()}，方向: {direction.Normalized()}，速度: {LinearVelocity}");
	}

	/// <summary>
	/// 获取球种类名称
	/// </summary>
	public string GetBallTypeName()
	{
		return BallType switch
		{
			PetBallDesign.Normal => "普通球",
			PetBallDesign.Grace => "恩赐球",
			PetBallDesign.Gold => "黄金球",
			PetBallDesign.Prismatic => "炫彩球",
			_ => $"未知({BallType})"
		};
	}
}