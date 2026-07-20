using Godot;
using System;

/// <summary>
/// 敌方场上当前精灵节点（单例）
/// 与 FightLandMyStandPet 结构一致，是敌方版本
/// </summary>
public partial class FightLandYouStandPet : Node2D
{
	private static FightLandYouStandPet _instance;
	public static FightLandYouStandPet Instance => _instance;

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

		var scene = GD.Load<PackedScene>("res://scenepet/__wrapper/pet_fight_wrapper.tscn");
		PetWrapper = scene.Instantiate<PetFightWrapper>();
		AddChild(PetWrapper);
		PetWrapper.Init(_spawnPosition, false, FightPetData);
		
		GD.Print($"[FightLandYouStandPet] 切换精灵: {FightPetData?.PetName}, HP={FightPetData?.Hp}/{FightPetData?.MaxHp}, Level={FightPetData?.Level}");
		if (FightPetData?.FinalStats != null)
			GD.Print($"  └─ FinalStats: {string.Join(", ", FightPetData.FinalStats)}");

	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}
}