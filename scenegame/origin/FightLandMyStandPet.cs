using Godot;
using System;

public partial class FightLandMyStandPet : Node2D
{
	private static FightLandMyStandPet _instance;
	public static FightLandMyStandPet Instance => _instance;

	[Export]
	public EnumPet Pet { get; set; } = EnumPet.Zero;

	[Export]
	public EnumPetType PetType { get; set; } = EnumPetType.Gold;

	/// <summary>
	/// 当前上场精灵的战斗数据（来自 PlayerLandMyStandPlayer 的战斗镜像）
	/// </summary>
	public InsFightPetData FightPetData { get; private set; }

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
		InitPet(Pet, PetType);
	}

	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// 生成当前上场宠物精灵：通过 DevPetLoadTool 根据战斗数据生成精灵
	/// 并同步战斗数据
	/// </summary>
	/// <param name="pet">精灵图鉴编号</param>
	/// <param name="petType">精灵系别</param>
	public void InitPet(EnumPet pet, EnumPetType petType)
	{
		Pet = pet;
		PetType = petType;

		// 同步战斗数据：取 PlayerLandMyStandPlayer 中第一只战斗精灵
		var fightPets = PlayerLandMyStandPlayer.Instance.FightPets;
		if (fightPets != null && fightPets.Count > 0)
		{
			FightPetData = fightPets[0];
		}

		// 获取生成位置（FightLandNewPetPoint 在 Ux 子层，需转换到本节点坐标系）
		var point = GetNode<Node2D>("Ux/FightLandNewPetPoint");
		var spawnPos = point != null ? ToLocal(point.GlobalPosition) : Vector2.Zero;

		// 通过战斗数据生成精灵
		DevPetLoadTool.SpawnDevPetFromFightData(FightPetData, this, spawnPos, true);
	}

	/// <summary>
	/// 切换上场精灵：移除旧精灵，根据战斗数据生成新精灵
	/// </summary>
	/// <param name="fightPetData">新上场的战斗精灵数据</param>
	public void SwitchPet(InsFightPetData fightPetData)
	{
		if (fightPetData == null)
			return;

		// 移除旧精灵（DevPetWrapper 实例）
		foreach (Node child in GetChildren())
		{
			if (child is DevPetWrapper)
			{
				child.QueueFree();
				break;
			}
		}

		// 更新战斗数据引用
		FightPetData = fightPetData;

		// 获取生成位置
		var point = GetNode<Node2D>("Ux/FightLandNewPetPoint");
		var spawnPos = point != null ? ToLocal(point.GlobalPosition) : Vector2.Zero;

		// 根据战斗数据生成新精灵
		DevPetLoadTool.SpawnDevPetFromFightData(FightPetData, this, spawnPos, true);
	}

	public override void _ExitTree()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}