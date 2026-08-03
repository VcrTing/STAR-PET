using Godot;
using System;

public partial class FightLandMyStandPet : Node2D
{
	private static FightLandMyStandPet _instance;
	public static FightLandMyStandPet Instance => _instance;

	[Export] public EnumPet Pet { get; set; } = EnumPet.Zero;
	[Export] public EnumPetType PetType { get; set; } = EnumPetType.Gold;

	public InsFightPetData FightPetData { get; private set; }
	public PetFightWrapper PetWrapper { get; private set; }
	private Vector2 _spawnPosition = Vector2.Zero;

	/// <summary>
	/// 本回合干预的先手值，用于干预本回合先手值判断
	/// </summary>
	private int _roundPriorityIntervene;

	public override void _EnterTree()
	{
		if (_instance != null) { QueueFree(); return; }
		_instance = this;
	}

	public override void _Ready()
	{
		var point = GetNode<Node2D>("FightLandNewPetPoint");
		if (point != null) _spawnPosition = ToLocal(point.GlobalPosition);
	}

	public override void _Process(double delta) { }

	/// <summary>
	/// 设置本回合干预的先手值
	/// </summary>
	/// <param name="value">干预先手值</param>
	public void SetRoundPriorityIntervene(int value)
	{
		_roundPriorityIntervene = value;
	}

	/// <summary>
	/// 获取本回合干预的先手值
	/// </summary>
	public int GetRoundPriorityIntervene()
	{
		return _roundPriorityIntervene;
	}

	/// <summary>
	/// 获取当前宠物速度值
	/// 从 FinalStats 字典中读取 SPD，默认返回 50
	/// 计算先手时需要包含本回合干预先手值，因此返回 速度 + 干预先手值
	/// </summary>
	public int GetSpeed()
	{
		if (FightPetData?.FinalStats != null &&
			FightPetData.FinalStats.TryGetValue(EnumPetBaseStats.SPD, out int speed))
			return speed;
		return 5;
	}

	/// <summary>
	/// 销毁场上精灵的视觉表现（精灵死亡时调用）
	/// </summary>
	public void DestroyPetWrapper()
	{
		if (PetWrapper != null)
		{
			PetWrapper.QueueFree();
			PetWrapper = null;
		}
		FightPetData = null;
	}

	public void SwitchPet(InsFightPetData fightPetData)
	{
		if (fightPetData == null) return;

		if (PetWrapper != null) { PetWrapper.QueueFree(); PetWrapper = null; }
		FightPetData = fightPetData;
		//
		PetWrapper = DevPetLoadTool.SpawnPetFightWrapper(fightPetData, this, _spawnPosition, true);
		// 刷新技能UI
		if (FightPetData?.FightSkills != null)
		{
			UiHBoxSkillsManager.Instance?.SwitchSkills(FightPetData.FightSkills);
		}

		// 关闭 Choise Pan
		PanFightPlayerPack.Instance?.Close();

		// 重置先手值
		SetRoundPriorityIntervene(0);
	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}
}