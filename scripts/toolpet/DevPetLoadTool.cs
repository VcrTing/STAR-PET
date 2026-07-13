using Godot;
using System;

/// <summary>
/// 开发用精灵加载工具
/// 负责从 pet_origin.tscn 实例化 DevPetWrapper 并初始化精灵数据，
/// 以及将实例挂载到指定父节点并设置位置
/// </summary>
public static class DevPetLoadTool
{
	/// <summary>
	/// 在指定父节点下生成一只精灵
	/// </summary>
	/// <param name="pet">精灵图鉴编号</param>
	/// <param name="petType">精灵系别</param>
	/// <param name="parent">父节点，调用其 AddChild 挂载精灵</param>
	/// <param name="position">生成位置</param>
	/// <param name="flipX">是否水平翻转（默认 false，宠物朝左；设为 true 则朝右）</param>
	/// <returns>已挂载并定位完成的 DevPetWrapper 实例</returns>
	public static DevPetWrapper SpawnDevPet(EnumPet pet, EnumPetType petType, Node parent, Vector2 position, bool flipX = false)
	{
		// 加载并实例化 DevPetWrapper 场景
		var scene = GD.Load<PackedScene>("res://scenepet/origin/pet_origin.tscn");
		var devPet = scene.Instantiate<DevPetWrapper>();

		// 初始化精灵数据（UUID 由 Init 内部自动生成）
		devPet.Init(pet, petType);

		// 挂载到父节点下
		parent.AddChild(devPet);

		// 设置生成位置
		devPet.Position = position;

		// 水平翻转子节点（默认朝左，flipX=true 则朝右）
		if (flipX)
		{
			devPet.FlipChildrenX();
		}

		// GD.Print($"[DevPetLoadTool] 已生成精灵: Pet={pet}({(int)pet}), PetType={petType}, UUID={devPet.InstanceUuid}, 位置=({position.X:F1}, {position.Y:F1}), 父节点={parent.Name}, flipX={flipX}");

		return devPet;
	}

	/// <summary>
	/// 根据战斗数据生成精灵
	/// </summary>
	/// <param name="fightPetData">战斗中的精灵数据</param>
	/// <param name="parent">父节点，调用其 AddChild 挂载精灵</param>
	/// <param name="position">生成位置</param>
	/// <param name="flipX">是否水平翻转（默认 false，宠物朝左；设为 true 则朝右）</param>
	/// <returns>已挂载并定位完成的 DevPetWrapper 实例</returns>
	public static DevPetWrapper SpawnDevPetFromFightData(InsFightPetData fightPetData, Node parent, Vector2 position, bool flipX = false)
	{
		if (fightPetData == null)
			return null;

		// 从战斗数据中提取图鉴编号和系别
		int petId = int.Parse(fightPetData.PetId);
		var pet = (EnumPet)petId;
		var petType = fightPetData.PetTypes.Count > 0 ? fightPetData.PetTypes[0] : EnumPetType.Gold;

		// 加载并实例化 DevPetWrapper 场景
		var scene = GD.Load<PackedScene>("res://scenepet/origin/pet_origin.tscn");
		var devPet = scene.Instantiate<DevPetWrapper>();

		// 初始化精灵数据（UUID 由 Init 内部自动生成）
		devPet.Init(pet, petType);

		// 挂载到父节点下
		parent.AddChild(devPet);

		// 设置生成位置
		devPet.Position = position;

		// 水平翻转子节点（默认朝左，flipX=true 则朝右）
		if (flipX)
		{
			devPet.FlipChildrenX();
		}

		GD.Print($"[DevPetLoadTool] 已从战斗数据生成精灵: Pet={pet}({petId}), UUID={devPet.InstanceUuid}, 位置=({position.X:F1}, {position.Y:F1}), 父节点={parent.Name}, flipX={flipX}");

		return devPet;
	}
}
