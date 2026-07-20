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

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}
}