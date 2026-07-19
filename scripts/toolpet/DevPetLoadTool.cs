using Godot;
using System;

/// <summary>
/// 开发用精灵加载工具
/// 从 res://scenepet/{系别}/pet_{编号}.tscn 实例化精灵场景
/// 若专属场景不存在则回退到 res://scenepet/origin/pet_origin.tscn
/// </summary>
public static class DevPetLoadTool
{
	private static string GetScenePath(EnumPet pet, EnumPetType petType)
	{
		int petId = (int)pet;
		string typeFolder = PetTypeDesign.GetDataFolderPath(petType);
		string specificPath = $"res://scenepet/{typeFolder}/pet_{petId}.tscn";
		if (ResourceLoader.Exists(specificPath))
			return specificPath;
		return "res://scenepet/origin/pet_origin.tscn";
	}

	public static DevPetWrapper SpawnDevPet(EnumPet pet, EnumPetType petType, Node parent, Vector2 position, bool flipX = false)
	{
		string scenePath = GetScenePath(pet, petType);
		var scene = GD.Load<PackedScene>(scenePath);
		var devPet = scene.Instantiate<DevPetWrapper>();
		devPet.Init(pet, petType);
		parent.AddChild(devPet);
		devPet.Position = position;
		if (flipX) devPet.FlipChildrenX();
		return devPet;
	}

	public static DevPetWrapper SpawnDevPetFromFightData(InsFightPetData fightPetData, Node parent, Vector2 position, bool flipX = false)
	{
		if (fightPetData == null) return null;
		int petId = int.Parse(fightPetData.PetId);
		var pet = (EnumPet)petId;
		var petType = fightPetData.PetTypes.Count > 0 ? fightPetData.PetTypes[0] : EnumPetType.Gold;

		string scenePath = GetScenePath(pet, petType);
		var scene = GD.Load<PackedScene>(scenePath);
		var devPet = scene.Instantiate<DevPetWrapper>();
		devPet.Pet = pet;
		devPet.PetType = petType;
		devPet.InstanceUuid = fightPetData.PetUuid;

		parent.AddChild(devPet);
		devPet.Position = position;
		if (flipX) devPet.FlipChildrenX();
		return devPet;
	}

	public static PetFightWrapper SpawnPetFightWrapper(InsFightPetData fightPetData, Node parent, Vector2 position, bool isMy)
	{
		if (fightPetData == null) return null;

		var scene = GD.Load<PackedScene>("res://scenepet/__wrapper/pet_fight_wrapper.tscn");
		var wrapper = scene.Instantiate<PetFightWrapper>();
		wrapper.Name = $"PetFight_{fightPetData.PetName}";
		wrapper.Init(position, isMy, fightPetData);
		parent.AddChild(wrapper);
		return wrapper;
	}
}