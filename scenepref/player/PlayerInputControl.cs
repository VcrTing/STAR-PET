using Godot;
using System;

public partial class PlayerInputControl : Node2D
{

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Movement keys - checked every frame for continuous movement
		if (Input.IsKeyPressed(Key.W))
		{
			PlayerMove.Instance.OnMoveUp();
		}
		if (Input.IsKeyPressed(Key.S))
		{
			PlayerMove.Instance.OnMoveDown();
		}
		if (Input.IsKeyPressed(Key.A))
		{
			PlayerMove.Instance.OnMoveLeft();
		}
		if (Input.IsKeyPressed(Key.D))
		{
			PlayerMove.Instance.OnMoveRight();
		}
		if (Input.IsKeyPressed(Key.Shift))
		{
			PlayerMove.Instance.OnSprint();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && !keyEvent.Echo)
		{
			switch (keyEvent.Keycode)
			{
				case Key.Space:
					if (keyEvent.Pressed) PlayerMove.Instance.OnJump();
					break;
				case Key.A:
					if (!keyEvent.Pressed) PlayerMove.Instance.OnMoveLeftRelease();
					break;
				case Key.D:
					if (!keyEvent.Pressed) PlayerMove.Instance.OnMoveRightRelease();
					break;
				case Key.C:
					if (keyEvent.Pressed) OnThrowPetBall();
					break;
			}
		}
	}

	private void OnThrowPetBall()
	{
		GD.Print("PlayerInputControl 按下了 C");
		// 朝玩家当前朝向扔出精灵球
		int dir = PlayerMove.Instance.GetDirection();
		Vector2 direction = new Vector2(dir, 0);
		if (direction == Vector2.Zero)
		{
			direction = Vector2.Right;
		}
		PlayerUxPetBall.Instance.ThrowPetBall(direction);
	}
}