using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用战斗精灵数据转换工具
/// 将背包精灵数据（InsPackPetData）转换为战斗精灵数据（InsFightPetData）
/// </summary>
public static class DevFightPackPetTool
{
	public static List<InsFightPetData> InitPackPetsToFight(List<InsPackPetData> pets, int fightLevel, bool isPvp)
	{
		var fightPets = new List<InsFightPetData>();
		if (pets == null || pets.Count == 0) return fightPets;

		foreach (var packPet in pets)
		{
			int level = isPvp ? fightLevel : packPet.Level;
			fightPets.Add(InitFightPets(packPet, level));
		}
		return fightPets;
	}

	public static InsFightPetData InitFightPets(InsPackPetData packData, int level)
	{
		if (packData == null) return null;

		var fightPet = new InsFightPetData
		{
			PetUuid = packData.PetUuid, PetId = packData.PetId, PetName = packData.PetName,
			PetTypes = new List<EnumPetType>(packData.PetTypes), Nickname = packData.Nickname,
			Level = level, Exp = packData.Exp, Hp = packData.Hp, MaxHp = packData.MaxHp,
			Iv = new Dictionary<EnumPetBaseStats, int>(packData.Iv),
			Talent = new Dictionary<EnumPetBaseStats, int>(packData.Talent),
			Skills = new List<string>(packData.Skills),
			CarriedSkills = new List<string>(packData.CarriedSkills),
			Nature = packData.Nature, Gender = packData.Gender, BallType = packData.BallType,
			PetFly = new List<EnumPetFly>(packData.PetFly),
			PetBig = packData.PetBig, PetAbility = packData.PetAbility,
			IsShiny = packData.IsShiny, HatchCounter = packData.HatchCounter,
			Intimacy = packData.Intimacy, IsLocked = packData.IsLocked, IsSpecial = packData.IsSpecial,
			ObtainedDate = packData.ObtainedDate, ObtainedMethod = packData.ObtainedMethod,
			ObtainedLocation = packData.ObtainedLocation,
			Medals = new List<EnumPetMedal>(packData.Medals),
			FinalStats = new Dictionary<EnumPetBaseStats, int>(packData.FinalStats)
		};

		// 根据 CarriedSkills 加载战斗技能（按数组索引记录技能顺序）
		if (packData.CarriedSkills != null && packData.CarriedSkills.Count > 0)
		{
			var skillIds = packData.CarriedSkills.ToArray();
			var skills = DevSkillLoadTool.LoadSkills(skillIds);
			for (int i = 0; i < skills.Count; i++)
				fightPet.FightSkills.Add(InsFightSkill.FromInsSkill(skills[i], i));
		}

		var growth = DevPetIvTool.GetGrowth2(level, packData.Intimacy);
		var finalStats = DevPetIvTool.Update(packData, level, growth);
		fightPet.FinalStats = finalStats;
		fightPet.MaxHp = fightPet.FinalStats.GetValueOrDefault(EnumPetBaseStats.HP, 100);
		fightPet.Hp = fightPet.MaxHp;
		return fightPet;
	}
}