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

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}
}