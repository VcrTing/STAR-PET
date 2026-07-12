using Godot;
using System;

public partial class PlayerUxPetBall : Node2D
{
	// Singleton
	private static PlayerUxPetBall _instance;
	public static PlayerUxPetBall Instance => _instance;

	[Export]
	// 扔球最小时间间隔（秒）
	public float ThrowInterval { get; set; } = 0.3f;

	// 当前选择的精灵球种类（默认普通球）
	public int CurrentBallType { get; set; } = PetBallDesign.Normal;

	private double _lastThrowTime;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Singleton setup
		if (_instance != null)
		{
			GD.PrintErr("PlayerUxPetBall singleton already exists, removing duplicate.");
			QueueFree();
			return;
		}
		_instance = this;

		_lastThrowTime = 0;
	}

	public override void _ExitTree()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 切换当前精灵球种类
	/// </summary>
	public void SetBallType(int ballType)
	{
		CurrentBallType = ballType;
		GD.Print($"PlayerUxPetBall 切换球种类: {GetBallTypeName(ballType)}");
	}

	/// <summary>
	/// 获取球种类名称
	/// </summary>
	public string GetBallTypeName(int ballType)
	{
		return ballType switch
		{
			PetBallDesign.Normal => "普通球",
			PetBallDesign.Grace => "恩赐球",
			PetBallDesign.Gold => "黄金球",
			PetBallDesign.Prismatic => "炫彩球",
			_ => $"未知({ballType})"
		};
	}

	/// <summary>
	/// 朝指定方向扔出精灵球（带防抖，最小间隔 0.3s）
	/// </summary>
	public void ThrowPetBall(Vector2 direction)
	{
		if (direction == Vector2.Zero) return;

		// 防抖：检查时间间隔
		double now = Time.GetTicksMsec() / 1000.0;
		if (now - _lastThrowTime < ThrowInterval)
		{
			GD.Print($"PlayerUxPetBall 扔球太频繁，剩余冷却: {(ThrowInterval - (now - _lastThrowTime)):F2}s");
			return;
		}
		_lastThrowTime = now;

		// 加载精灵球场景
		PackedScene petBallScene = GD.Load<PackedScene>("res://scenepref/petball/pet_ball.tscn");
		if (petBallScene == null)
		{
			GD.PrintErr("无法加载精灵球场景: res://scenepref/petball/pet_ball.tscn");
			return;
		}

		// 实例化场景
		Node2D petBallInstance = (Node2D)petBallScene.Instantiate();

		// 设置初始位置为玩家位置
		petBallInstance.GlobalPosition = GlobalPosition;

		// 添加到场景树
		GetTree().CurrentScene.AddChild(petBallInstance);

		// 获取 PetBallBody 并调用 Init（传入当前球种类）
		PetBallBody body = petBallInstance.GetNode<PetBallBody>("PetBallBody");
		if (body != null)
		{
			body.Init(direction, CurrentBallType);
			GD.Print($"PlayerUxPetBall 扔出了{GetBallTypeName(CurrentBallType)}");
		}
		else
		{
			GD.PrintErr("PetBall 场景中未找到 PetBallBody 节点");
		}
	}
}