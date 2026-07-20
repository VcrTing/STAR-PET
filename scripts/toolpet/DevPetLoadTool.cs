using Godot;
using System;

/// <summary>
/// 开发用精灵加载工具
/// 从 res://scenepet/{系别}/pet_{编号}.tscn 实例化精灵场景
/// 若专属场景不存在则回退到 res://scenepet/origin/pet_origin.tscn
/// </summary>
public static class DevPetLoadTool
{

	public static String GetPetTexTurePath(InsFightPetData FightPet)
	{
		int petId = int.Parse(FightPet.PetId);
		var petType = FightPet.PetTypes.Count > 0 ? FightPet.PetTypes[0] : EnumPetType.Gold;
		string typeFolder = PetTypeDesign.GetDataFolderPath(petType);
		string scenePath = $"res://scenepet/{typeFolder}/pet_{petId}.tscn";
		if (!ResourceLoader.Exists(scenePath))
			scenePath = "res://scenepet/Gold/pet_0.tscn";
		return scenePath;
	}
	public static PetFightWrapper SpawnPetFightWrapper(InsFightPetData FightPetData, Node parent, Vector2 position, bool isMy)
	{
		if (FightPetData == null) return null;

		var scene = GD.Load<PackedScene>("res://scenepet/__wrapper/pet_fight_wrapper.tscn");

		PetFightWrapper PetWrapper = scene.Instantiate<PetFightWrapper>();
		parent.AddChild(PetWrapper);
		PetWrapper.Init(position, false, FightPetData);
		
		if (FightPetData?.FinalStats != null)
			GD.Print($"  {FightPetData?.PetName}，Level={FightPetData?.Level}，FinalStats: {string.Join(", ", FightPetData.FinalStats)}");

		return PetWrapper;
	}
}