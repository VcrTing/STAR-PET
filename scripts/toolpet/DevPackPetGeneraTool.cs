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
	/// 自动加载 define/datapet/{typeFolder}/stone/stone_pet_{petId}.gd 配置
	/// </summary>
	public static InsPackPetData InitSpecialStonePet(EnumPet pet, EnumPetType petType)
	{
		string uuid = Guid.NewGuid().ToString();
		int petId = (int)pet;
		string typeFolder = PetTypeDesign.GetDataFolderPath(petType);

		Resource petData = DevPackPetTool.LoadOrCreatePetData(pet, petType);
		string stonePath = $"res://define/datapet/{typeFolder}/stone/stone_pet_{petId}.gd";
		Resource stoneData = null;
		if (ResourceLoader.Exists(stonePath))
		{
			var stoneScript = GD.Load<GDScript>(stonePath);
			if (stoneScript != null)
				stoneData = stoneScript.New().AsGodotObject() as Resource;
		}

		string petName = petData?.Get("pet_name").AsString() ?? "???";
		float femaleRatio = (float)(petData?.Get("female_ratio").AsDouble() ?? 50.0);

		var baseStatsDict = petData?.Get("base_stats").AsGodotDictionary();
		var ivDict = PetBaseStatsDesign.BaseStatsToIvDict(baseStatsDict);

		int initialLevel = stoneData?.Get("initial_level").AsInt32() ?? 5;
		int initialNature = stoneData?.Get("initial_nature").AsInt32() ?? (int)EnumPetNature.Timid;
		int initialIntimacy = stoneData?.Get("initial_intimacy").AsInt32() ?? 30;
		int defaultBig = stoneData?.Get("default_big").AsInt32() ?? (int)EnumPetBig.Normal;

		// 从 petData 读取 pet_fly_type 数组
		var petFlyTypeArray = petData?.Get("pet_fly_type").AsGodotArray();
		var petFlyTypes = new List<int>();
		if (petFlyTypeArray != null && petFlyTypeArray.Count > 0)
		{
			foreach (var item in petFlyTypeArray)
				petFlyTypes.Add((int)item);
		}
		else
		{
			int singleFlyType = petData?.Get("pet_fly_type").AsInt32() ?? (int)EnumPetFly.Walk;
			petFlyTypes.Add(singleFlyType);
		}

		bool isLocked = stoneData?.Get("is_locked").AsBool() ?? true;
		bool isSpecial = stoneData?.Get("is_special").AsBool() ?? true;
		int talentType = stoneData?.Get("talent_type").AsInt32() ?? (int)EnumPetTalent.Normal;
		string obtainedMethod = stoneData?.Get("obtained_method").AsString() ?? "初始精灵";
		string obtainedLocation = stoneData?.Get("obtained_location").AsString() ?? "启程之森";

		var talentFixedStatsArray = stoneData?.Get("talent_fixed_stats").AsGodotArray();
		var talentPointsDict = new Dictionary<EnumPetBaseStats, int>();
		foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
			talentPointsDict[stat] = 0;
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
			foreach (EnumPetBaseStats stat in Enum.GetValues(typeof(EnumPetBaseStats)))
				talentPointsDict[stat] = PetTalentDesign.RollTalentValue(talentType);
		}
		var evDict = talentPointsDict;

		EnumPetGender gender = femaleRatio > 0f && RandomTool.Range(0, 100) < femaleRatio
			? EnumPetGender.Female : EnumPetGender.Male;

		var packData = CreateBasePackData(uuid, pet, petType, petData);
		packData.PetName = petName; packData.Nickname = petName;
		packData.Level = initialLevel; packData.Nature = (EnumPetNature)initialNature;
		packData.Gender = gender; packData.IsLocked = isLocked; packData.IsSpecial = isSpecial;
		packData.PetFly = (petFlyTypes.Count > 0)
			? petFlyTypes.ConvertAll(f => (EnumPetFly)f)
			: new List<EnumPetFly> { EnumPetFly.Walk };
		packData.PetBig = (EnumPetBig)defaultBig;
		packData.PetAbility = DevAbilityConf.GetAbility(pet);
		packData.Intimacy = initialIntimacy; packData.Iv = ivDict; packData.Talent = evDict;
		packData.ObtainedMethod = obtainedMethod; packData.ObtainedLocation = obtainedLocation;
		packData.ObtainedDate = DateTime.Now.ToString("yyyy-MM-dd");

		FillPetTypes(packData, petData, petType);
		FillLearnedSkills(packData, petData);

		var growth = DevPetIvTool.GetGrowth2(packData.Level, packData.Intimacy);
		packData.FinalStats = DevPetIvTool.Update(packData, packData.Level, growth);
		return packData;
	}

	public static InsPackPetData InitEggPet(EnumPet pet, EnumPetType petType, string fatherUuid, string motherUuid)
	{
		string uuid = Guid.NewGuid().ToString();
		Resource petData = DevPackPetTool.LoadOrCreatePetData(pet, petType);
		var packData = CreateBasePackData(uuid, pet, petType, petData);
		packData.PetName = petData?.Get("pet_name").AsString() ?? "???";
		packData.Level = 1;
		packData.Nature = (EnumPetNature)RandomTool.Range(1, 25);
		packData.Gender = RandomTool.Range(0, 1) == 0 ? EnumPetGender.Male : EnumPetGender.Female;
		packData.Intimacy = 50; packData.HatchCounter = 0;
		packData.ObtainedMethod = "孵化"; packData.ObtainedLocation = "培育屋";
		FillPetTypes(packData, petData, petType);
		return packData;
	}

	public static InsPackPetData InitCapturePet(EnumPet pet, EnumPetType petType, int captureLevel, string captureLocation, int ballType)
	{
		string uuid = Guid.NewGuid().ToString();
		Resource petData = DevPackPetTool.LoadOrCreatePetData(pet, petType);
		var packData = CreateBasePackData(uuid, pet, petType, petData);
		packData.PetName = petData?.Get("pet_name").AsString() ?? "???";
		packData.Level = captureLevel; packData.BallType = ballType;
		packData.Nature = (EnumPetNature)RandomTool.Range(1, 25);
		packData.Gender = RandomTool.Range(0, 1) == 0 ? EnumPetGender.Male : EnumPetGender.Female;
		packData.Intimacy = 10;
		packData.ObtainedMethod = "捕捉"; packData.ObtainedLocation = captureLocation;
		FillPetTypes(packData, petData, petType);
		return packData;
	}

	// ==================== 共性方法 ====================

	private static void FillLearnedSkills(InsPackPetData packData, Resource petData)
	{
		if (petData == null) return;
		var learnableArray = petData.Get("learnable_skills").AsGodotArray();
		if (learnableArray == null || learnableArray.Count == 0) return;

		List<string> learnedSkills = new();
		foreach (var item in learnableArray)
		{
			var entry = item.AsGodotArray();
			if (entry == null || entry.Count < 2) continue;
			string skillId = entry[0].AsString();
			int learnLevel = entry[1].AsInt32();
			if (string.IsNullOrEmpty(skillId)) continue;
			if (packData.Level >= learnLevel)
				learnedSkills.Add(skillId);
		}
		packData.Skills = learnedSkills;
		int carryCount = Math.Min(learnedSkills.Count, 4);
		packData.CarriedSkills = learnedSkills.GetRange(0, carryCount);
	}

	private static void FillPetTypes(InsPackPetData packData, Resource petData, EnumPetType petType)
	{
		var petTypesArray = petData?.Get("pet_types").AsGodotArray();
		if (petTypesArray != null && petTypesArray.Count > 0)
		{
			packData.PetTypes.Clear();
			foreach (var item in petTypesArray)
				packData.PetTypes.Add((EnumPetType)(int)item);
		}
		else
			packData.PetTypes = new List<EnumPetType> { petType };
	}

	private static InsPackPetData CreateBasePackData(string instanceUuid, EnumPet pet, EnumPetType petType, Resource petData)
	{
		return new InsPackPetData
		{
			PetUuid = instanceUuid, PetId = ((int)pet).ToString(),
			PetTypes = new() { petType },
			Nickname = petData != null ? petData.Get("pet_name").AsString() : "???",
			Level = 1, Exp = 0, Hp = 100, MaxHp = 100,
			Nature = EnumPetNature.Serious, Gender = EnumPetGender.Male,
			BallType = 0, IsShiny = false, HatchCounter = 0, Intimacy = 0,
			IsLocked = false, IsSpecial = false,
			ObtainedDate = DateTime.Now.ToString("yyyy-MM-dd"),
			ObtainedMethod = "未知", ObtainedLocation = "未知",
			Iv = new(), Talent = new(), Skills = new(), Medals = new(), FinalStats = new()
		};
	}
}