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

	/// <summary>
	/// 玩家上阵的精灵背包数据列表（背包中的原始数据）
	/// </summary>
	public List<InsPackPetData> StandPets { get; private set; } = new();

	/// <summary>
	/// 战斗中使用的精灵数据镜像（背包数据的深拷贝副本，战斗期间修改不影响背包）
	/// </summary>
	public List<InsFightPetData> FightPets { get; private set; } = new();

	public override void _EnterTree()
	{
		if (_instance != null)
		{
			QueueFree();
			return;
		}
		_instance = this;
	}

	public override void _Ready()
	{
		Init();
		InitFight();

		// 切换第一只精灵上场
		if (FightPets.Count > 0)
		{
			FightLandMyStandPet.Instance.SwitchPet(FightPets[0]);
		}
	}

	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 初始化上阵精灵列表（从背包数据中选择）
	/// 若列表为空，则自动加载零号精灵（EnumPet.Zero / EnumPetType.Gold）作为默认精灵
	/// </summary>
	/// <param name="pets">玩家上阵的精灵背包数据列表（可为 null 或空）</param>
	public void Init(List<InsPackPetData> pets = null)
	{
		if (pets != null && pets.Count > 0)
		{
			StandPets = pets;
			return;
		}

		// 列表为空，加载零号精灵作为默认上阵精灵
		var packData = DevPackPetTool.LoadAndSync(EnumPet.Zero, EnumPetType.Gold, out _);
		StandPets = new List<InsPackPetData> { packData };
		// GD.Print("[PlayerLandMyStandPlayer] 上阵精灵列表为空，已自动加载零号精灵作为默认上阵精灵");
	}

	/// <summary>
	/// 战斗开始：将 StandPets 中的背包数据深拷贝为战斗数据 FightPets
	/// </summary>
	public void InitFight()
	{
		FightPets.Clear();
		foreach (var packPet in StandPets)
		{
			FightPets.Add(InsFightPetData.FromPackData(packPet));
		}

		// 打印上阵精灵名字
		if (StandPets.Count == 0)
		{
			GD.Print("[PlayerLandMyStandPlayer] 上阵精灵列表为空");
			return;
		}

		var namesList = new List<string>();
		foreach (var p in StandPets)
		{
			var name = p.PetName;
			if (string.IsNullOrEmpty(name))
				name = $"ID:{p.PetId}";
			namesList.Add(name);
			GD.Print($"  - 精灵: {name}, PetUuid={p.PetUuid}, PetId={p.PetId}");
		}
		GD.Print($"[PlayerLandMyStandPlayer] 上阵精灵: {string.Join(", ", namesList)}");
	}

	public override void _ExitTree()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}