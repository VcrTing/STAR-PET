using Godot;
using System.Collections.Generic;

/// <summary>
/// 精灵生死管理容器
/// 存放战斗中已死亡的精灵数据，提供存活精灵查询方法
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
		if (pet == null) return;

		string sideLabel = side == EnumWho.My ? "🧑我方" : "👹敌方";
		GD.Print($"      💀 [FightAliveHouse] {sideLabel} 精灵死亡: {pet.PetName} (Uuid={pet.PetUuid})");

		if (side == EnumWho.My)
		{
			MyDiePets.Add(pet);
			HeartMy = Mathf.Max(HeartMy - 1, 0);
			FightLandMyStandPet.Instance?.DestroyPetWrapper();
		}
		else
		{
			YouDiePets.Add(pet);
			HeartYou = Mathf.Max(HeartYou - 1, 0);
			FightLandYouStandPet.Instance?.DestroyPetWrapper();
		}
	}

	/// <summary>
	/// 获取指定方的所有存活精灵 Uuid（包含背包和场上）
	/// </summary>
	/// <param name="side">所属方</param>
	/// <returns>存活精灵的 Uuid 集合</returns>
	public static HashSet<string> GetAlivePetUuids(EnumWho side)
	{
		var uuids = new HashSet<string>();
		List<InsFightPetData> pets = side == EnumWho.My
			? PlayerLandMyStandPlayer.Instance?.FightPets
			: PlayerLandYouStandPlayer.Instance?.FightPets;

		if (pets != null)
		{
			foreach (var pet in pets)
			{
				if (pet != null && pet.Hp > 0)
					uuids.Add(pet.PetUuid);
			}
		}

		// 场上精灵
		InsFightPetData standPet = side == EnumWho.My
			? FightLandMyStandPet.Instance?.FightPetData
			: FightLandYouStandPet.Instance?.FightPetData;

		if (standPet != null && standPet.Hp > 0)
			uuids.Add(standPet.PetUuid);

		return uuids;
	}

	/// <summary>
	/// 对比本回合前后存活列表，将本回合新死亡的精灵存入死亡列表
	/// </summary>
	/// <param name="aliveMyUuids">本回合开始时我方存活精灵 Uuid</param>
	/// <param name="aliveYouUuids">本回合开始时敌方存活精灵 Uuid</param>
	public static void CollectDiePets(HashSet<string> aliveMyUuids, HashSet<string> aliveYouUuids)
	{
		// 我方：检查所有精灵，若不在 aliveMyUuids 中且 Hp <= 0，则为本回合死亡
		var myFightPets = PlayerLandMyStandPlayer.Instance?.FightPets;
		if (myFightPets != null)
		{
			foreach (var pet in myFightPets)
			{
				if (pet != null && pet.Hp <= 0 && !aliveMyUuids.Contains(pet.PetUuid))
				{
					AddDiePet(pet, EnumWho.My);
				}
			}
		}
		var myStandPet = FightLandMyStandPet.Instance?.FightPetData;
		if (myStandPet != null && myStandPet.Hp <= 0 && !aliveMyUuids.Contains(myStandPet.PetUuid))
		{
			AddDiePet(myStandPet, EnumWho.My);
		}

		// 敌方：同样逻辑
		var youFightPets = PlayerLandYouStandPlayer.Instance?.FightPets;
		if (youFightPets != null)
		{
			foreach (var pet in youFightPets)
			{
				if (pet != null && pet.Hp <= 0 && !aliveYouUuids.Contains(pet.PetUuid))
				{
					AddDiePet(pet, EnumWho.You);
				}
			}
		}
		var youStandPet = FightLandYouStandPet.Instance?.FightPetData;
		if (youStandPet != null && youStandPet.Hp <= 0 && !aliveYouUuids.Contains(youStandPet.PetUuid))
		{
			AddDiePet(youStandPet, EnumWho.You);
		}
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