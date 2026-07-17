using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用精灵入背包生成工具
/// 负责 InsPackPetData 的创建与客制化
/// 提供零号精灵初始化、生蛋生成、捕捉生成三种入口
/// 开发者可在此处干预生成数据的各项参数
/// </summary>
public static class DevPackPetGeneraTool
{
	// ==================== 公开入口方法 ====================

	/// <summary>
	/// 生成固定特殊精灵（由 stone 客制化配置驱动）
	/// 自动加载 datapet/{typeFolder}/stone/stone_pet_{petId}.gd 配置
	/// 用于初始精灵、活动赠送、NPC给予等固定生成的特殊精灵
	/// </summary>
	/// <param name="pet">精灵图鉴编号</param>
	/// <param name="petType">精灵主系别</param>
	public static InsPackPetData InitSpecialStonePet(EnumPet pet, EnumPetType petType)
	{
		string uuid = Guid.NewGuid().ToString();

		int petId = (int)pet;
		string typeFolder = PetTypeDesign.GetDataFolderPath(petType);

		// 1. 加载精灵基础静态数据
		Resource petData = DevPackPetTool.LoadOrCreatePetData(pet, petType);

		// 2. 加载 stone 客制化配置文件
		string stonePath = $"res://datapet/{typeFolder}/stone/stone_pet_{petId}.gd";
		Resource stoneData = null;
		if (ResourceLoader.Exists(stonePath))
		{
			var stoneScript = GD.Load<GDScript>(stonePath);
			if (stoneScript != null)
				stoneData = stoneScript.New().AsGodotObject() as Resource;
		}
		GD.Print($"stonePath: {stonePath}");

		// 3. 从基础静态数据中提取信息
		string petName = petData?.Get("pet_name").AsString() ?? "???";
		float femaleRatio = (float)(petData?.Get("female_ratio").AsDouble() ?? 50.0);

		// 4. 读取 base_stats 并转换为 Iv
		var baseStatsDict = petData?.Get("base_stats").AsGodotDictionary();
		var ivDict = PetBaseStatsDesign.BaseStatsToIvDict(baseStatsDict);

		// 5. 读取 stone 配置或使用默认值
		int initialLevel = stoneData?.Get("initial_level").AsInt32() ?? 5;
		int initialNature = stoneData?.Get("initial_nature").AsInt32() ?? (int)EnumPetNature.Timid;
		int initialIntimacy = stoneData?.Get("initial_intimacy").AsInt32() ?? 30;
		int defaultBig = stoneData?.Get("default_big").AsInt32() ?? (int)EnumPetBig.Normal;
		int petFlyType = petData?.Get("pet_fly_type").AsInt32() ?? (int)EnumPetFly.Walk;
		bool isLocked = stoneData?.Get("is_locked").AsBool() ?? true;
		bool isSpecial = stoneData?.Get("is_special").AsBool() ?? true;
		int talentType = stoneData?.Get("talent_type").AsInt32() ?? (int)EnumPetTalent.Normal;
		string obtainedMethod = stoneData?.Get("obtained_method").AsString() ?? "初始精灵";
		string obtainedLocation = stoneData?.Get("obtained_location").AsString() ?? "启程之森";

		// 6. 生成天赋值字典（存入原始点数，战斗时由 CalculateFinalStats3 按等级缩放）
		var talentFixedStatsArray = stoneData?.Get("talent_fixed_stats").AsGodotArray();
		// GD.Print($"talentType: {talentType}, talent_fixed_stats: {talentFixedStatsArray}");
		var talentPointsDict = new Dictionary<EnumPetBaseStats, int>();
		foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
		{
			talentPointsDict[stat] = 0;
		}
		if (talentFixedStatsArray != null && talentFixedStatsArray.Count > 0)
		{
			int points = PetTalentDesign.RollTalentValue(talentType);
			foreach (var item in talentFixedStatsArray)
			{
				int statId = (int)item;
				talentPointsDict[(EnumPetBaseStats)statId] = points;
			}
		}
		else
		{
			// 未指定则全属性使用 RollTalentValue
			foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
			{
				talentPointsDict[stat] = PetTalentDesign.RollTalentValue(talentType);
			}
		}
		var evDict = talentPointsDict;
		// GD.Print(string.Join(", ", evDict));

		// 7. 根据 female_ratio 决定性别（0=全雄性，>0=按概率随机）
		EnumPetGender gender = femaleRatio > 0f && RandomTool.Range(0, 100) < femaleRatio
			? EnumPetGender.Female
			: EnumPetGender.Male;

		// 8. 构建背包数据
		var packData = CreateBasePackData(uuid, pet, petType, petData);

		// ---- stone 客制化配置驱动（由 stone_pet_{petId}.gd 定义） ----
		packData.PetName = petName;
		packData.Nickname = petName;
		packData.Level = initialLevel;
		packData.Nature = (EnumPetNature)initialNature;
		packData.Gender = gender;
		packData.IsLocked = isLocked;
		packData.IsSpecial = isSpecial;
		packData.PetFly = (EnumPetFly)petFlyType;
		packData.PetBig = (EnumPetBig)defaultBig;
		packData.PetAbility = DevAbilityConf.GetAbility(pet);
		packData.Intimacy = initialIntimacy;
		packData.Iv = ivDict;
		packData.Talent = evDict;
		packData.ObtainedMethod = obtainedMethod;
		packData.ObtainedLocation = obtainedLocation;
		packData.ObtainedDate = DateTime.Now.ToString("yyyy-MM-dd");

		// 填充系别数组（优先读取 pet_0.gd 的 pet_types）
		FillPetTypes(packData, petData, petType);

		// 计算最终个体值 FinalStats
		var growth = DevPetIvTool.GetGrowth2(packData.Level, packData.Intimacy);
		packData.FinalStats = DevPetIvTool.Update(packData, packData.Level, growth);

		return packData;
	}

	/// <summary>
	/// 生蛋生成精灵
	/// </summary>
	public static InsPackPetData InitEggPet(EnumPet pet, EnumPetType petType, string fatherUuid, string motherUuid)
	{
		string uuid = Guid.NewGuid().ToString();

		Resource petData = DevPackPetTool.LoadOrCreatePetData(pet, petType);
		var packData = CreateBasePackData(uuid, pet, petType, petData);

		// ---- 生蛋专属客制化 ----
		packData.PetName = petData?.Get("pet_name").AsString() ?? "???";
		packData.Level = 1;
		packData.Nature = (EnumPetNature)RandomTool.Range(1, 25);
		packData.Gender = RandomTool.Range(0, 1) == 0 ? EnumPetGender.Male : EnumPetGender.Female;
		packData.Intimacy = 50;
		packData.HatchCounter = 0;
		packData.ObtainedMethod = "孵化";
		packData.ObtainedLocation = "培育屋";

		// 填充系别数组
		FillPetTypes(packData, petData, petType);

		return packData;
	}

	/// <summary>
	/// 捕捉精灵进背包时生成精灵
	/// </summary>
	public static InsPackPetData InitCapturePet(EnumPet pet, EnumPetType petType, int captureLevel, string captureLocation, int ballType)
	{
		string uuid = Guid.NewGuid().ToString();

		Resource petData = DevPackPetTool.LoadOrCreatePetData(pet, petType);
		var packData = CreateBasePackData(uuid, pet, petType, petData);

		// ---- 捕捉专属客制化 ----
		packData.PetName = petData?.Get("pet_name").AsString() ?? "???";
		packData.Level = captureLevel;
		packData.Nature = (EnumPetNature)RandomTool.Range(1, 25);
		packData.Gender = RandomTool.Range(0, 1) == 0 ? EnumPetGender.Male : EnumPetGender.Female;
		packData.BallType = ballType;
		packData.Intimacy = 10;
		packData.ObtainedMethod = "捕捉";
		packData.ObtainedLocation = captureLocation;

		// 填充系别数组
		FillPetTypes(packData, petData, petType);

		return packData;
	}

	// ==================== 共性方法 ====================

	/// <summary>
	/// 从 petData 读取 pet_types 数组填充到 InsPackPetData.PetTypes
	/// 若 pet_types 不存在或为空，则使用 petType 作为唯一系别
	/// </summary>
	private static void FillPetTypes(InsPackPetData packData, Resource petData, EnumPetType petType)
	{
		var petTypesArray = petData?.Get("pet_types").AsGodotArray();
		if (petTypesArray != null && petTypesArray.Count > 0)
		{
			packData.PetTypes.Clear();
			foreach (var item in petTypesArray)
			{
				int typeId = (int)item;
				packData.PetTypes.Add((EnumPetType)typeId);
			}
		}
		else
		{
			// 未定义 pet_types 则使用传入的 petType 作为唯一系别
			packData.PetTypes = new List<EnumPetType> { petType };
		}
	}

	/// <summary>
	/// 创建基础 InsPackPetData，包含所有预设默认值
	/// 三个入口方法都调用此方法创建基础数据，再各自客制化
	/// </summary>
	private static InsPackPetData CreateBasePackData(string instanceUuid, EnumPet pet, EnumPetType petType, Resource petData)
	{
		return new InsPackPetData
		{
			PetUuid = instanceUuid,
			PetId = ((int)pet).ToString(),
			PetTypes = new() { petType },
			Nickname = petData != null ? petData.Get("pet_name").AsString() : "???",
			Level = 1,
			Exp = 0,
			Hp = 100,
			MaxHp = 100,
			Nature = EnumPetNature.Serious,
			Gender = EnumPetGender.Male,
			BallType = 0,
			IsShiny = false,
			HatchCounter = 0,
			Intimacy = 0,
			IsLocked = false,
			IsSpecial = false,
			ObtainedDate = DateTime.Now.ToString("yyyy-MM-dd"),
			ObtainedMethod = "未知",
			ObtainedLocation = "未知",
			Iv = new(),
			Talent = new(),
			Skills = new(),
			Medals = new(),
			FinalStats = new()
		};
	}
}