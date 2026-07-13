using Godot;
using System;

/// <summary>
/// 开发用精灵包装器（单例）
/// 用于在场景中展示/测试精灵实例
/// 内存储 EnumPet 信息以便加载对应的数据文件
/// 初始化后自动与 InstancePackPetManager 同步背包数据
/// </summary>
public partial class DevPetWrapper : Node2D
{
	private static DevPetWrapper _instance;
	public static DevPetWrapper Instance => _instance;

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

	public override void _EnterTree()
	{
		if (_instance != null)
		{
			QueueFree();
			return;
		}
		_instance = this;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	/// <summary>
	/// 水平翻转所有子节点（适用于精灵默认朝左，需翻转时调用）
	/// 通过 Transform.X 向量取反实现，不影响 Scale
	/// </summary>
	public void FlipChildrenX()
	{
		foreach (Node child in GetChildren())
		{
			if (child is Node2D node2d)
			{
				var t = node2d.Transform;
				t.X = new Vector2(-t.X.X, t.X.Y);
				node2d.Transform = t;
			}
		}
	}

	/// <summary>
	/// 初始化并加载精灵数据
	/// UUID 由 DevPackPetTool.LoadAndSync 内部自动生成
	/// </summary>
	/// <param name="pet">精灵图鉴编号</param>
	/// <param name="petType">精灵系别</param>
	public void Init(EnumPet pet, EnumPetType petType)
	{
		Pet = pet;
		PetType = petType;

		// 一站式加载静态数据 + 同步背包数据（UUID 内部自动生成）
		// 若 datapet/ 下的文件不存在，DevPackPetTool 会自动创建默认初始数据
		PackData = DevPackPetTool.LoadAndSync(Pet, PetType, out var petData);
		InstanceUuid = PackData.PetUuid;
		PetData = petData;
	}
}