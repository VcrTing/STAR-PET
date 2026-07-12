using Godot;
using System;

/// <summary>
/// 开发用精灵包装器
/// 用于在场景中展示/测试精灵实例
/// 内存储 EnumPet 信息以便加载对应的 pet_xxxx.gd 数据文件
/// 初始化后自动与 InstancePackPetManager 同步背包数据
/// </summary>
public partial class DevPetWrapper : Node2D
{
	/// <summary>
	/// 精灵图鉴编号，对应 res://datapet/ 下的数据文件
	/// </summary>
	[Export]
	public EnumPet Pet { get; set; } = EnumPet.Zero;

	/// <summary>
	/// 精灵系别，用于定位 datapet/ 下的文件夹（如 Metal、Fire、Water 等）
	/// 开发时手动选择，决定加载哪个系别文件夹内的精灵数据
	/// </summary>
	[Export]
	public EnumPetType PetType { get; set; } = EnumPetType.Gold;

	/// <summary>
	/// 自动生成的实例 UUID，用于关联 InstancePackPetManager 中的 InsPackPetData
	/// </summary>
	[Export]
	public string InstanceUuid { get; set; } = "";

	/// <summary>
	/// 从 datapet/ 加载的精灵资源数据
	/// </summary>
	public Resource PetData { get; private set; }

	/// <summary>
	/// 对应的背包精灵数据实例（由 InstancePackPetManager 管理）
	/// </summary>
	public InsPackPetData PackData { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// 1. 自动生成 UUID
		if (string.IsNullOrEmpty(InstanceUuid))
		{
			InstanceUuid = Guid.NewGuid().ToString();
		}

		// 2. 一站式加载静态数据 + 同步背包数据
		// 若 datapet/ 下的文件不存在，DevPackPetTool 会自动创建默认初始数据
		PackData = DevPackPetTool.LoadAndSync(InstanceUuid, Pet, PetType, out var petData);
		PetData = petData;
	}
}