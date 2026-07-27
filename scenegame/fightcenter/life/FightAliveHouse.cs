using Godot;
using System.Collections.Generic;

/// <summary>
/// 精灵生死管理容器
/// 存放战斗中已死亡的精灵数据，提供存活精灵查询方法
/// 内部实现委托给 FightPetLifeTool
/// </summary>
public static class FightAliveHouse
{
	/// <summary>
	/// 我方拥有精灵的心数（即生命次数，默认4）
	/// </summary>
	public static int HeartMy { get; set; } = 4;

	/// <summary>
	/// 敌方拥有精灵的心数（即生命次数，默认4）
	/// </summary>
	public static int HeartYou { get; set; } = 4;

	/// <summary>
	/// 我方已死亡的精灵列表
	/// </summary>
	public static List<InsFightPetData> MyDiePets { get; private set; } = new();

	/// <summary>
	/// 敌方已死亡的精灵列表
	/// </summary>
	public static List<InsFightPetData> YouDiePets { get; private set; } = new();

	/// <summary>
	/// 将精灵加入死亡列表
	/// 根据 side 判断加入 MyDiePets 或 YouDiePets
	/// </summary>
	/// <param name="pet">已死亡的精灵数据</param>
	/// <param name="side">所属方</param>
	public static void AddDiePet(InsFightPetData pet, EnumWho side)
	{
		FightPetLifeTool.AddDiePet(pet, side);
	}

	/// <summary>
	/// 获取指定方的所有存活精灵 Uuid（包含背包和场上）
	/// </summary>
	/// <param name="side">所属方</param>
	/// <returns>存活精灵的 Uuid 集合</returns>
	public static HashSet<string> GetAlivePetUuids(EnumWho side)
	{
		return FightPetLifeTool.GetAlivePetUuids(side);
	}

	/// <summary>
	/// 对比本回合前后存活列表，将本回合新死亡的精灵存入死亡列表
	/// </summary>
	/// <param name="aliveMyUuids">本回合开始时我方存活精灵 Uuid</param>
	/// <param name="aliveYouUuids">本回合开始时敌方存活精灵 Uuid</param>
	public static void CollectDiePets(HashSet<string> aliveMyUuids, HashSet<string> aliveYouUuids)
	{
		FightPetLifeTool.CollectDiePets(aliveMyUuids, aliveYouUuids);
	}

	/// <summary>
	/// 清空所有死亡列表
	/// </summary>
	public static void Clear()
	{
		MyDiePets.Clear();
		YouDiePets.Clear();
	}
}
