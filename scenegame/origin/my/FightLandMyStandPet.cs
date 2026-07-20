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
	/// 获取当前宠物速度值
	/// 从 FinalStats 字典中读取 SPD，默认返回 50
	/// </summary>
	public int GetSpeed()
	{
		if (FightPetData?.FinalStats != null &&
			FightPetData.FinalStats.TryGetValue(EnumPetBaseStats.SPD, out int speed))
			return speed;
		return 50;
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
	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}
}