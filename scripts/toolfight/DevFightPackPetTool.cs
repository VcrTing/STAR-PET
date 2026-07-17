using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用战斗精灵数据转换工具
/// 将背包精灵数据（InsPackPetData）转换为战斗精灵数据（InsFightPetData）
/// </summary>
public static class DevFightPackPetTool
{
	/// <summary>
	/// 将背包精灵列表深拷贝为战斗精灵数据列表
	/// </summary>
	/// <param name="pets">背包精灵数据列表</param>
	/// <param name="fightLevel">战斗等级（PVP 时统一为 60）</param>
	/// <param name="isPvp">是否为 PVP 模式（true=使用统一 fightLevel，false=使用各精灵原始等级）</param>
	/// <returns>战斗精灵数据列表</returns>
	public static List<InsFightPetData> InitPackPetsToFight(List<InsPackPetData> pets, int fightLevel, bool isPvp)
	{
		var fightPets = new List<InsFightPetData>();

		if (pets == null || pets.Count == 0)
		{
			GD.Print("[DevFightPackPetTool] 背包精灵列表为空");
			return fightPets;
		}

		foreach (var packPet in pets)
		{
			// PVP 使用统一 fightLevel，非 PVP 使用精灵自身背包等级
			int level = isPvp ? fightLevel : packPet.Level;
			fightPets.Add(InitFightPets(packPet, level));
		}

		// 打印上阵精灵名字
		var namesList = new List<string>();
		foreach (var p in pets)
		{
			var name = p.PetName;
			if (string.IsNullOrEmpty(name))
				name = $"ID:{p.PetId}";
			namesList.Add(name);
			GD.Print($"  - 精灵: {name}, PetUuid={p.PetUuid}, PetId={p.PetId}");
		}
		GD.Print($"[DevFightPackPetTool] 转换精灵: {string.Join(", ", namesList)}");

		return fightPets;
	}

	/// <summary>
	/// 从 InsPackPetData 转换为 InsFightPetData（深拷贝）
	/// </summary>
	/// <param name="packData">背包中的精灵数据</param>
	/// <param name="level">精灵等级（用于最终属性计算，独立于 packData.Level）</param>
	/// <returns>战斗中的精灵数据副本</returns>
	public static InsFightPetData InitFightPets(InsPackPetData packData, int level)
	{
		if (packData == null)
			return null;

		var fightPet = new InsFightPetData
		{
			PetUuid = packData.PetUuid,
			PetId = packData.PetId,
			PetName = packData.PetName,
			PetTypes = new List<EnumPetType>(packData.PetTypes),
			Nickname = packData.Nickname,
			Level = level,
			Exp = packData.Exp,
			Hp = packData.Hp,
			MaxHp = packData.MaxHp,
			Iv = new Dictionary<EnumPetBaseStats, int>(packData.Iv),
			Talent = new Dictionary<EnumPetBaseStats, int>(packData.Talent),
			Skills = new List<string>(packData.Skills),
			CarriedSkills = new List<string>(packData.CarriedSkills),
			Nature = packData.Nature,
			Gender = packData.Gender,
			BallType = packData.BallType,
			PetFly = packData.PetFly,
			PetBig = packData.PetBig,
			PetAbility = packData.PetAbility,
			IsShiny = packData.IsShiny,
			HatchCounter = packData.HatchCounter,
			Intimacy = packData.Intimacy,
			IsLocked = packData.IsLocked,
			IsSpecial = packData.IsSpecial,
			ObtainedDate = packData.ObtainedDate,
			ObtainedMethod = packData.ObtainedMethod,
			ObtainedLocation = packData.ObtainedLocation,
			Medals = new List<EnumPetMedal>(packData.Medals),
			FinalStats = new Dictionary<EnumPetBaseStats, int>(packData.FinalStats)
		};

		// 根据 CarriedSkills 加载战斗技能
		if (packData.CarriedSkills != null && packData.CarriedSkills.Count > 0)
		{
			var skillIds = packData.CarriedSkills.ToArray();
			var skills = DevSkillLoadTool.LoadSkills(skillIds);
			foreach (var skill in skills)
			{
				fightPet.FightSkills.Add(InsFightSkill.FromInsSkill(skill));
			}
		}

		// 重新计算 FinalStats（使用方案3完整版公式）
		// 亲密度默认取 100（最亲密）
		var growth = DevPetIvTool.GetGrowth2(level, packData.Intimacy);

		var finalStats = DevPetIvTool.Update(
			packData,        // iv — 个体值
			// packData.Talent,    // talent — 天赋值（0-10）
			level,              // level — 精灵等级（使用传入的 fightLevel）
			// packData.Nature,    // nature — 性格
			growth              // growth — 成长值（按等级+亲密100计算）
		);
		GD.Print($"{packData.PetName} 最终个体值: {string.Join(", ", finalStats)}");
		fightPet.FinalStats = finalStats;

		// MaxHp 从 FinalStats 中取值，Hp 取 MaxHp（满血出战）
		fightPet.MaxHp = fightPet.FinalStats.GetValueOrDefault(EnumPetBaseStats.HP, 100);
		fightPet.Hp = fightPet.MaxHp;

		return fightPet;
	}
}