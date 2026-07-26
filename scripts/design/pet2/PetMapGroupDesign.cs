using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 宠物地图组设计
/// 宠物出现/栖息的地图区域类型：高空、低空、水下、水边、地下、陆地、虚拟、特殊
/// </summary>
public static class PetMapGroupDesign
{

	/// <summary>
	/// 地图组中文名称映射
	/// </summary>
	private static readonly Dictionary<int, string> _mapGroupData = new()
	{
		{ (int)EnumPetMapGroup.HighAlt, "高空" },
		{ (int)EnumPetMapGroup.LowAlt, "低空" },
		{ (int)EnumPetMapGroup.Underwater, "水下" },
		{ (int)EnumPetMapGroup.WaterEdge, "水边" },
		{ (int)EnumPetMapGroup.Underground, "地下" },
		{ (int)EnumPetMapGroup.Land, "陆地" },
		{ (int)EnumPetMapGroup.Virtual, "虚拟" },
		{ (int)EnumPetMapGroup.Special, "特殊" },
	};

	/// <summary>
	/// 获取地图组的中文名称
	/// </summary>
	public static string GetMapGroupName(int mapGroupId)
	{
		if (_mapGroupData.TryGetValue(mapGroupId, out var name))
			return name;
		return $"Unknown({mapGroupId})";
	}

	/// <summary>
	/// 获取所有地图组 ID 列表
	/// </summary>
	public static List<int> GetAllMapGroupIds()
	{
		return new List<int>(_mapGroupData.Keys);
	}
}