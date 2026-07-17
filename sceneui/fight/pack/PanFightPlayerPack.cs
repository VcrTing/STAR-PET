using Godot;
using System;

public partial class PanFightPlayerPack : PanelContainer
{
	public static PanFightPlayerPack Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		Visible = false;
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Open()
	{
		Visible = true;

		var pets = PlayerLandMyStandPlayer.Instance?.FightPets;
		if (pets != null && pets.Count > 0)
		{
			ScrollPetsContent.Instance?.RefreshPetItems(pets.ToArray());
		}
	}

	public void Close()
	{
		Visible = false;
	}
}
