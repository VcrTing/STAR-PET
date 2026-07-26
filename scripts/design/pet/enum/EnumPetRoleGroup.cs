using Godot;

/// <summary>
/// 宠物定位组枚举
/// 宠物战斗定位/角色类型：速物攻、速魔攻、坦克、慢启动、增益辅助、清理辅助、低速物攻、低速魔攻、正常
/// </summary>
public enum EnumPetRoleGroup
{
	Normal = 0,        // 正常
	FastPhysAtk = 1,   // 速物攻
	FastMagAtk = 2,    // 速魔攻
	Tank = 3,          // 坦克
	SlowStart = 4,     // 慢启动
	BuffSupport = 5,   // 增益辅助
	CleanSupport = 6,  // 清理辅助
	SlowPhysAtk = 7,   // 低速物攻
	SlowMagAtk = 8,    // 低速魔攻
}