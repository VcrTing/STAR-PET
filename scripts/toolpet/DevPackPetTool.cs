using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用精灵入背包工具
/// 提供精灵数据实例的创建与背包同步功能
/// 负责加载 datapet/ 下的静态数据，若文件不存在则创建默认初始数据
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
		string path = $"res://datapet/{typeFolder}/pet_{petId}.gd";

		if (ResourceLoader.Exists(path))
		{
			var data = ResourceLoader.Load(path);
			// GD.Print($"[DevPackPetTool] 已加载精灵数据: {path}");
			return data;
		}

		// 文件不存在，创建默认初始 Resource
		GD.PrintErr($"[DevPackPetTool] 精灵数据文件不存在，使用默认数据: {path}");
		var fallback = new Resource();
		fallback.Set("pet_id", petId);
		fallback.Set("pet_name", "???");
		return fallback;
	}

	/// <summary>
	/// 通过 InstancePackPetManager 查找或创建 InsPackPetData
	/// 若 UUID 已存在则直接返回，否则新建数据实例并添加到背包
	/// </summary>
	/// <param name="instanceUuid">精灵实例 UUID</param>
	/// <param name="pet">精灵图鉴编号</param>
	/// <param name="petType">精灵系别</param>
	/// <param name="petData">已加载的精灵静态数据（可为 null）</param>
	/// <returns>背包中的精灵数据实例</returns>
	public static InsPackPetData SyncPackData(string instanceUuid, EnumPet pet, EnumPetType petType, Resource petData)
	{
		// 尝试按 UUID 查找已有数据
		var packData = InstancePackPetManager.Instance.GetPetByUuid(instanceUuid);

		if (packData != null)
		{
			// GD.Print($"[DevPackPetTool] 已找到背包数据: {instanceUuid}");
			return packData;
		}

		// 未找到则创建基础数据实例并添加到背包
		// 开发者可在此处改为调用 DevPackPetGeneraTool 的三个专用方法之一：
		//   DevPackPetGeneraTool.InitSpecialStonePet(pet, petType)
		//   DevPackPetGeneraTool.InitEggPet(pet, petType, fatherUuid, motherUuid)
		//   DevPackPetGeneraTool.InitCapturePet(pet, petType, level, location, ballType)
		int petId = (int)pet;

		packData = new InsPackPetData
		{
			PetUuid = instanceUuid,
			PetId = petId.ToString(),
			PetTypes = new() { petType },
			PetName = petData != null ? petData.Get("pet_name").AsString() : "???",
			Nickname = petData != null ? petData.Get("pet_name").AsString() : "???",
			Level = 1,
			Exp = 0,
			Hp = 100,
			MaxHp = 100,
			Nature = EnumPetNature.Serious,
			Gender = EnumPetGender.Male,
			IsLocked = false,
			IsSpecial = false,
			Intimacy = 0,
			ObtainedDate = DateTime.Now.ToString("yyyy-MM-dd"),
			ObtainedMethod = "开发测试",
			ObtainedLocation = "DevTool"
		};

		// CarriedSkills 为空时插入默认携带技能
		if (packData.CarriedSkills == null || packData.CarriedSkills.Count == 0)
		{
			packData.CarriedSkills = new List<string> { "0_2_1", "0_1_1", "0_1_2", "0_1_3" };
		}

		InstancePackPetManager.Instance.AddPet(packData);
		// GD.Print($"[DevPackPetTool] 已创建新背包数据: {instanceUuid}");

		return packData;
	}

	/// <summary>
	/// 一站式方法：加载精灵静态数据（或创建默认数据），并同步到背包管理器
	/// UUID 内部自动生成
	/// </summary>
	/// <param name="pet">精灵图鉴编号</param>
	/// <param name="petType">精灵系别</param>
	/// <param name="petData">输出参数：加载或创建的精灵静态数据</param>
	/// <returns>背包中的精灵数据实例</returns>
	public static InsPackPetData LoadAndSync(EnumPet pet, EnumPetType petType, out Resource petData)
	{
		string instanceUuid = Guid.NewGuid().ToString();
		petData = LoadOrCreatePetData(pet, petType);
		return SyncPackData(instanceUuid, pet, petType, petData);
	}
}