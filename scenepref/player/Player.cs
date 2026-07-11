using Godot;
using System;

public partial class Player : Node2D
{
	// Singleton instance
	private static Player _instance;
	public static Player Instance => _instance;

	// Child IPlayerMoveImpl (defaults to first found)
	private IPlayerMoveImpl _moveImpl;

	public IPlayerMoveImpl MoveImpl => _moveImpl;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Singleton setup
		if (_instance != null)
		{
			GD.PrintErr("Player singleton already exists, removing duplicate.");
			QueueFree();
			return;
		}
		_instance = this;

		// Get first child IPlayerMoveImpl
		_moveImpl = GodotTool.GetFirstChildOfType<IPlayerMoveImpl>(this);
		if (_moveImpl != null)
		{
			GD.Print($"Player found IPlayerMoveImpl: {((Node)_moveImpl).Name} (default)");
		}

		if (_moveImpl == null)
		{
			GD.PrintErr("Player has no child implementing IPlayerMoveImpl");
		}
	}

	/// <summary>
	/// Move the player's position by the given offset.
	/// </summary>
	public void MoveOffset(float x, float y)
	{
		Position += new Vector2(x, y);
	}

	public override void _ExitTree()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}
