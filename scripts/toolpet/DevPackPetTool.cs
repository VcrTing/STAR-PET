using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用精灵入背包工具
/// 提供精灵数据实例的创建与背包同步功能
/// 负责加载 define/datapet/ 下的静态数据，若文件不存在则创建默认初始数据
/// </summary>
public static class DevPackPetTool
{
	/// <summary>
	/// 加载精灵静态数据，若文件不存在则创建默认 Resource
	/// </summary>
	/// <param name="pet">精灵图鉴编号</param>
	/// <param name="petType">精灵系别</param>
	/// <returns>精灵资源数据（加载或创建的默认数据）</returns>
	public static Resource LoadOrCreatePetData(EnumPet pet, EnumPetType petType)
	{
		int petId = (int)pet;
		string typeFolder = PetTypeDesign.GetDataFolderPath(petType);
		string path = $"res://define/datapet/{typeFolder}/pet_{petId}.gd";

		// 文件不存在，创建默认初始 Resource
		if (!ResourceLoader.Exists(path))
		{
			GD.PrintErr($"[DevPackPetTool] 精灵数据文件不存在，使用默认数据: {path}");
			var fallback = new Resource();
			fallback.Set("pet_id", petId);
			fallback.Set("pet_name", "???");
			return fallback;
		}

		// 加载 GDScript 并实例化为 Resource
		var gdScript = GD.Load<GDScript>(path);
		if (gdScript == null)
		{
			GD.PrintErr($"[DevPackPetTool] 无法加载精灵脚本: {path}");
			var fallback = new Resource();
			fallback.Set("pet_id", petId);
			fallback.Set("pet_name", "???");
			return fallback;
		}

		var instance = gdScript.New().AsGodotObject() as Resource;
		if (instance == null)
		{
			GD.PrintErr($"[DevPackPetTool] 无法实例化精灵数据: {path}");
			var fallback = new Resource();
			fallback.Set("pet_id", petId);
			fallback.Set("pet_name", "???");
			return fallback;
		}

		return instance;
	}

	/// <summary>
	/// 通过 InstancePackPetManager 查找或创建 InsPackPetData
	/// 若 UUID 已存在则直接返回，否则新建数据实例并添加到背包
	/// </summary>
	public static InsPackPetData SyncPackData(string instanceUuid, EnumPet pet, EnumPetType petType, Resource petData)
	{
		var packData = InstancePackPetManager.Instance.GetPetByUuid(instanceUuid);
		if (packData != null)
			return packData;
		return CreateDefaultPackData(instanceUuid, pet, petType, petData);
	}

	private static InsPackPetData CreateDefaultPackData(string instanceUuid, EnumPet pet, EnumPetType petType, Resource petData)
	{
		int petId = (int)pet;
		var packData = new InsPackPetData
		{
			PetUuid = instanceUuid,
			PetId = petId.ToString(),
			PetTypes = new() { petType },
			PetName = petData != null ? petData.Get("pet_name").AsString() : "???",
			Nickname = petData != null ? petData.Get("pet_name").AsString() : "???",
			Level = 1, Exp = 0, Hp = 100, MaxHp = 100,
			Nature = EnumPetNature.Serious, Gender = EnumPetGender.Male,
			IsLocked = false, IsSpecial = false, Intimacy = 0,
			ObtainedDate = DateTime.Now.ToString("yyyy-MM-dd"),
			ObtainedMethod = "开发测试", ObtainedLocation = "DevTool"
		};
		if (packData.CarriedSkills == null || packData.CarriedSkills.Count == 0)
			packData.CarriedSkills = new List<string> { "0_2_1", "0_1_1", "0_1_2", "0_1_3" };
		InstancePackPetManager.Instance.AddPet(packData);
		return packData;
	}

	public static InsPackPetData LoadAndSync(EnumPet pet, EnumPetType petType, out Resource petData)
	{
		string instanceUuid = Guid.NewGuid().ToString();
		petData = LoadOrCreatePetData(pet, petType);
		return SyncPackData(instanceUuid, pet, petType, petData);
	}
}