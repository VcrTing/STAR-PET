using Godot;
using System;

public partial class VBoxFightPlayerPetsPack : VBoxContainer
{
	public static VBoxFightPlayerPetsPack Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void LoadPlayerPets(InsFightPetData[] pets)
	{
		if (pets == null) return;

		ScrollPetsContent.Instance?.RefreshPetItems(pets);

		// 默认选中当前上场精灵，没找到就不展示
		var defaultPet = FightLandMyStandPet.Instance?.FightPetData;
		if (defaultPet != null)
			VBoxPetMsgContent.Instance?.UpdatePetData(defaultPet);
	}
}
