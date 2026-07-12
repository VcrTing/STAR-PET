using Godot;
using System;

public partial class PlayerMove : CharacterBody2D, IPlayerMoveImpl
{
	// Singleton
	private static PlayerMove _instance;
	public static PlayerMove Instance => _instance;

	[Export]
	// 初始水平速度（按下方向键时的起步速度）
	public float SpeedMin { get; set; } = 270f;

	[Export]
	// 水平加速度（每帧增加的速度值）
	public float SpeedAdd { get; set; } = 20f;

	[Export]
	// 最大水平速度上限
	public float SpeedMax { get; set; } = 560f;

	[Export]
	// 跳跃速度
	public float JumpVelocity { get; set; } = -520f;

	[Export]
	// 最大连跳次数（1=单跳，2=双跳，3=三跳...）
	public int MaxJumpCount { get; set; } = 2;

	private float _gravity;
	private float _direction;
	private float _currentHSpeed;
	private int _jumpCount;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Singleton setup
		if (_instance != null)
		{
			GD.PrintErr("PlayerMove singleton already exists, removing duplicate.");
			QueueFree();
			return;
		}
		_instance = this;

		_gravity = GodotTool.GetDefaultGravity();
		_direction = 0;
		_currentHSpeed = 0;
		_jumpCount = MaxJumpCount;
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
		Vector2 velocity = Velocity;

		// 落地时重置连跳次数
		if (IsOnFloor())
		{
			_jumpCount = MaxJumpCount;
		}

		// 应用重力（空中时累加）
		if (!IsOnFloor())
		{
			velocity.Y += _gravity * (float)delta;
		}

		// 水平速度加速/减速
		if (_direction != 0)
		{
			// 有方向输入：起步 → 逐渐加速到最大
			if (_currentHSpeed < SpeedMin)
			{
				_currentHSpeed = SpeedMin;
			}
			_currentHSpeed += SpeedAdd;
			if (_currentHSpeed > SpeedMax)
			{
				_currentHSpeed = SpeedMax;
			}
		}
		else
		{
			// 无方向输入：速度归零
			_currentHSpeed = 0;
		}

		velocity.X = _direction * _currentHSpeed;

		Velocity = velocity;
		MoveAndSlide();
	}

	// ---- IPlayerMoveImpl Implementation ----

	public virtual void OnMoveUp()
	{
	}

	public virtual void OnMoveDown()
	{
	}

	public virtual void OnMoveLeft()
	{
		_direction = -1;
	}

	public virtual void OnMoveRight()
	{
		_direction = 1;
	}

	public virtual void OnMoveLeftRelease()
	{
		_direction = 0;
		_currentHSpeed = 0;
	}

	public virtual void OnMoveRightRelease()
	{
		_direction = 0;
		_currentHSpeed = 0;
	}

	/// <summary>
	/// 获取当前水平朝向（-1=左，1=右，0=无方向）
	/// </summary>
	public int GetDirection()
	{
		return (int)_direction;
	}

	public virtual void OnSprint()
	{
		GD.Print("PlayerMove 按下了 Shift");
	}

	public virtual void OnJump()
	{
		if (_jumpCount <= 0) return;

		// 在 JumpVelocity 基础上随机 ±20
		float randomOffset = RandomTool.Range(-20f, 20f);
		float actualJumpVelocity = JumpVelocity + randomOffset;

		Vector2 velocity = Velocity;
		velocity.Y = actualJumpVelocity;
		Velocity = velocity;
		_jumpCount--;
		// GD.Print($"PlayerMove 按下了 Space 跳跃，速度: {actualJumpVelocity:F1}，剩余连跳次数: {_jumpCount}");
	}
}