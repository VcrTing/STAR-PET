using Godot;

/// <summary>
/// 物理层配置，集中管理所有物理层编号
/// </summary>
public static class GamePhysicsConf
{
	/// <summary>
	/// 玩家物理层
	/// </summary>
	public static int PlayerLayer = 10;
	
	/// <summary>
	/// 精灵
	/// </summary>
	public static int PetLayer = 12;

	/// <summary>
	/// MapLand 地图地面物理层
	/// </summary>
	public static int MapLandLayer = 4;
	
	/// <summary>
	/// 精灵球
	/// </summary>
	public static int PetBallLayer = 17;
}