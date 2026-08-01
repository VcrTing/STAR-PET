using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 敌方场上阵营管理（单例）
/// 维护敌方上阵的精灵列表及战斗数据镜像
/// 与 PlayerLandMyStandPlayer 结构一致，是敌方版本
/// </summary>
public partial class PlayerLandYouStandPlayer : Node2D
{
	private static PlayerLandYouStandPlayer _instance;
	public static PlayerLandYouStandPlayer Instance => _instance;

	public List<InsPackPetData> StandPets { get; private set; } = new();
	public List<InsFightPetData> FightPets { get; private set; } = new();

	public override void _EnterTree()
	{
		if (_instance != null) { QueueFree(); return; }
		_instance = this;
	}

	public override void _Ready() { }
	public override void _Process(double delta) { }

	public void Init(List<InsPackPetData> pets = null)
	{
		if (pets != null && pets.Count > 0)
		{
			StandPets = pets;
			return;
		}
		StandPets = DefPackPet.testModeYouPackPets();
	}

	public void InitFight(bool isPvp, int fightLevel)
	{
		FightPets = DevFightPackPetTool.InitPackPetsToFight(StandPets, fightLevel, isPvp);
	}

	/// <summary>
	/// 获取敌方战斗精灵数组
	/// </summary>
	/// <param name="excludeStanding">是否排除本次上场宠物；
	/// =true 则排除 FightLandYouStandPet.FightPetData 已上场的宠物后返回；
	/// =false 返回原数组</param>
	/// <returns>战斗精灵数组</returns>
	public InsFightPetData[] GetFightPets(bool excludeStanding)
	{
		if (FightPets == null || FightPets.Count == 0)
			return new InsFightPetData[0];

		// =false 返回原数组
		if (!excludeStanding)
			return FightPets.ToArray();

		// =true 排除已上场宠物
		var standingPet = FightLandYouStandPet.Instance?.FightPetData;
		var result = new System.Collections.Generic.List<InsFightPetData>();
		for (int i = 0; i < FightPets.Count; i++)
		{
			var pet = FightPets[i];
			if (pet == null)
				continue;
			// 排除当前场上宠物（按 Uuid 匹配）
			if (standingPet != null && pet.PetUuid == standingPet.PetUuid)
				continue;
			result.Add(pet);
		}
		return result.ToArray();
	}

	/// <summary>
	/// 判断当前是否可以切换宠物
	/// 敌方换宠由系统自动处理，此处预留接口
	/// </summary>
	public bool CanSwitchPet()
	{
		return false;
	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}
}