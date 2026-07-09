using Godot;
using System;

public partial class PlayerInputControl : Node2D
{
	// Signals to notify other components
	[Signal]
	public delegate void MoveUpEventHandler();
	[Signal]
	public delegate void MoveDownEventHandler();
	[Signal]
	public delegate void MoveLeftEventHandler();
	[Signal]
	public delegate void MoveRightEventHandler();
	[Signal]
	public delegate void JumpEventHandler();
	[Signal]
	public delegate void SprintEventHandler();
	[Signal]
	public delegate void InteractEventHandler();
	[Signal]
	public delegate void Skill1EventHandler();
	[Signal]
	public delegate void Skill2EventHandler();
	[Signal]
	public delegate void Skill3EventHandler();
	[Signal]
	public delegate void Skill4EventHandler();
	[Signal]
	public delegate void EscapeEventHandler();
	[Signal]
	public delegate void TabEventHandler();

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
			OnMoveUp();
		}
		if (Input.IsKeyPressed(Key.S))
		{
			OnMoveDown();
		}
		if (Input.IsKeyPressed(Key.A))
		{
			OnMoveLeft();
		}
		if (Input.IsKeyPressed(Key.D))
		{
			OnMoveRight();
		}
		if (Input.IsKeyPressed(Key.Shift))
		{
			OnSprint();
		}
	}

	public override void _Input(InputEvent @event)
	{
		// Only process key presses (not releases)
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			switch (keyEvent.Keycode)
			{
				case Key.Space:
					OnJump();
					break;
				case Key.F:
					OnInteract();
					break;
				case Key.Key1:
					OnSkill1();
					break;
				case Key.Key2:
					OnSkill2();
					break;
				case Key.Key3:
					OnSkill3();
					break;
				case Key.Key4:
					OnSkill4();
					break;
				case Key.Escape:
					OnEscape();
					break;
				case Key.Tab:
					OnTab();
					break;
			}
		}
	}

	// ---- Movement Methods (called continuously in _Process) ----
	public virtual void OnMoveUp()
	{
		EmitSignal(SignalName.MoveUp);
	}

	public virtual void OnMoveDown()
	{
		EmitSignal(SignalName.MoveDown);
	}

	public virtual void OnMoveLeft()
	{
		EmitSignal(SignalName.MoveLeft);
	}

	public virtual void OnMoveRight()
	{
		EmitSignal(SignalName.MoveRight);
	}

	public virtual void OnSprint()
	{
		EmitSignal(SignalName.Sprint);
	}

	// ---- Action Methods (called once on key press in _Input) ----
	public virtual void OnJump()
	{
		EmitSignal(SignalName.Jump);
	}

	public virtual void OnInteract()
	{
		EmitSignal(SignalName.Interact);
	}

	public virtual void OnSkill1()
	{
		EmitSignal(SignalName.Skill1);
	}

	public virtual void OnSkill2()
	{
		EmitSignal(SignalName.Skill2);
	}

	public virtual void OnSkill3()
	{
		EmitSignal(SignalName.Skill3);
	}

	public virtual void OnSkill4()
	{
		EmitSignal(SignalName.Skill4);
	}

	public virtual void OnEscape()
	{
		EmitSignal(SignalName.Escape);
	}

	public virtual void OnTab()
	{
		EmitSignal(SignalName.Tab);
	}
}