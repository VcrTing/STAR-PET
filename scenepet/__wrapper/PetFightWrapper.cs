using Godot;
using System;

/// <summary>
/// 战斗精灵包装器（Node2D）
/// 战斗场景中每只精灵的根节点，包裹精灵的视觉表现与战斗数据
/// </summary>
public partial class PetFightWrapper : Node2D
{
	public InsFightPetData FightPet { get; private set; }
	public bool IsMy { get; private set; }
	public Node2D PetView { get; private set; }

	public void Update(InsFightPetData fightData)
	{
		FightPet = fightData;
	}

	public void Init(Vector2 position, bool isMy, InsFightPetData fightPetData)
	{
		Position = position;
		IsMy = isMy;
		FightPet = fightPetData;

		int petId = int.Parse(FightPet.PetId);
		var petType = FightPet.PetTypes.Count > 0 ? FightPet.PetTypes[0] : EnumPetType.Gold;
		string typeFolder = PetTypeDesign.GetDataFolderPath(petType);
		string scenePath = $"res://scenepet/{typeFolder}/pet_{petId}.tscn";
		if (!ResourceLoader.Exists(scenePath))
			scenePath = "res://scenepet/Gold/pet_0.tscn";

		var scene = GD.Load<PackedScene>(scenePath);
		PetView = scene.Instantiate<Node2D>();
		AddChild(PetView);
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta) { }
}