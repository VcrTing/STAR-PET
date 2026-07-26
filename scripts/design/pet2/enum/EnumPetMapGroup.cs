using Godot;

/// <summary>
/// 宠物地图组枚举
/// 宠物出现/栖息的地图区域类型：高空、低空、水下、水边、地下、陆地、虚拟、特殊
/// </summary>
public enum EnumPetMapGroup
{
	HighAlt = 1,    // 高空
	LowAlt = 2,     // 低空
	Underwater = 3, // 水下
	WaterEdge = 4,  // 水边
	Underground = 5,// 地下
	Land = 6,       // 陆地
	Virtual = 7,    // 虚拟
	Special = 8,    // 特殊
}