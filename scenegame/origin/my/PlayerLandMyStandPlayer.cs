using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 玩家场上阵营管理（单例）
/// 维护玩家上阵的精灵列表及战斗数据镜像
/// </summary>
public partial class PlayerLandMyStandPlayer : Node2D
{
	private static PlayerLandMyStandPlayer _instance;
	public static PlayerLandMyStandPlayer Instance => _instance;

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
		StandPets = DefPackPet.testModeMyPackPets();
	}

	public void InitFight(bool isPvp, int fightLevel)
	{
		FightPets = DevFightPackPetTool.InitPackPetsToFight(StandPets, fightLevel, isPvp);
	}

	/// <summary>
	/// 获取我方战斗精灵数组
	/// </summary>
	/// <param name="excludeStanding">是否排除本次上场宠物；
	/// =true 则排除 FightLandMyStandPet.FightPetData 已上场的宠物后返回；
	/// =false 返回原数组</param>
	/// <returns>战斗精灵数组</returns>
	public InsFightPetData[] GetCanSiwtchFightPets(bool excludeStanding)
	{
		if (FightPets == null || FightPets.Count == 0)
			return new InsFightPetData[0];

		// =false 返回原数组
		if (!excludeStanding)
			return FightPets.ToArray();

		// =true 排除已上场宠物
		var standingPet = FightLandMyStandPet.Instance?.FightPetData;
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
	/// PlayerTurn 状态下允许切换（含正常回合和濒死强制换宠）
	/// </summary>
	public bool CanSwitchPet()
	{
		return FightCenterManger.Instance?.CanPlayerAct() ?? false;
	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}
}