using Godot;
using System.Collections.Generic;

/// <summary>
/// 精灵生命周期工具
/// 提供精灵死亡判定、存活查询等可复用的生命周期方法
/// </summary>
public static class FightPetLifeTool
{
    /// <summary>
    /// 将精灵加入死亡列表，并处理离场逻辑
    /// 操作 FightAliveHouse 的静态数据
    /// </summary>
    /// <param name="pet">已死亡的精灵数据</param>
    /// <param name="side">所属方</param>
    public static void AddDiePet(InsFightPetData pet, EnumWho side)
    {
        if (pet == null) return;

        string sideLabel = side == EnumWho.My ? "🧑我方" : "👹敌方";
        GD.Print($"      💀 [FightPetLifeTool] {sideLabel} 精灵死亡: {pet.PetName} (Uuid={pet.PetUuid})");

        if (side == EnumWho.My)
        {
            FightAliveHouse.MyDiePets.Add(pet);
            FightAliveHouse.HeartMy = Mathf.Max(FightAliveHouse.HeartMy - 1, 0);
            FightLandMyStandPet.Instance?.DestroyPetWrapper();

            // 清除该精灵的 ThisPetAppear Buff
            FightMyStandBuffManager.Instance?.WhenPetDisAppear(pet);
        }
        else
        {
            FightAliveHouse.YouDiePets.Add(pet);
            FightAliveHouse.HeartYou = Mathf.Max(FightAliveHouse.HeartYou - 1, 0);
            FightLandYouStandPet.Instance?.DestroyPetWrapper();

            // 清除该精灵的 ThisPetAppear Buff
            FightYouStandBuffManager.Instance?.WhenPetDisAppear(pet);
        }
    }

    /// <summary>
    /// 从死亡精灵列表中筛选出指定方的死亡精灵
    /// </summary>
    /// <param name="deadPets">死亡精灵列表（可能包含双方）</param>
    /// <param name="side">要筛选的阵营</param>
    /// <returns>属于该阵营的死亡精灵列表</returns>
    public static List<InsFightPetData> FilterDeadBySide(List<InsFightPetData> deadPets, EnumWho side)
    {
        var result = new List<InsFightPetData>();
        if (deadPets == null || deadPets.Count == 0)
            return result;

        // 获取该方所有 FightPets 的 Uuid 集合，用于判定归属
        HashSet<string> sidePetUuids = new HashSet<string>();
        List<InsFightPetData> sidePets = side == EnumWho.My
            ? PlayerLandMyStandPlayer.Instance?.FightPets
            : PlayerLandYouStandPlayer.Instance?.FightPets;

        if (sidePets != null)
        {
            foreach (var p in sidePets)
            {
                if (p != null && !string.IsNullOrEmpty(p.PetUuid))
                    sidePetUuids.Add(p.PetUuid);
            }
        }

        foreach (var pet in deadPets)
        {
            if (pet != null && sidePetUuids.Contains(pet.PetUuid))
                result.Add(pet);
        }

        return result;
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
    /// 操作 FightAliveHouse 的静态数据
    /// </summary>
    /// <param name="aliveMyUuids">本回合开始时我方存活精灵 Uuid</param>
    /// <param name="aliveYouUuids">本回合开始时敌方存活精灵 Uuid</param>
    /// <returns>返回本回合新死亡的精灵列表（包含双方）</returns>
    public static List<InsFightPetData> CollectDiePets(HashSet<string> aliveMyUuids, HashSet<string> aliveYouUuids)
    {
        var newDiePets = new List<InsFightPetData>();

        // 我方：检查所有精灵，若不在 aliveMyUuids 中且 Hp <= 0，则为本回合死亡
        var myFightPets = PlayerLandMyStandPlayer.Instance?.FightPets;
        if (myFightPets != null)
        {
            foreach (var pet in myFightPets)
            {
                if (pet != null && pet.Hp <= 0 && !aliveMyUuids.Contains(pet.PetUuid))
                {
                    AddDiePet(pet, EnumWho.My);
                    newDiePets.Add(pet);
                }
            }
        }
        var myStandPet = FightLandMyStandPet.Instance?.FightPetData;
        if (myStandPet != null && myStandPet.Hp <= 0 && !aliveMyUuids.Contains(myStandPet.PetUuid))
        {
            AddDiePet(myStandPet, EnumWho.My);
            newDiePets.Add(myStandPet);
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
                    newDiePets.Add(pet);
                }
            }
        }
        var youStandPet = FightLandYouStandPet.Instance?.FightPetData;
        if (youStandPet != null && youStandPet.Hp <= 0 && !aliveYouUuids.Contains(youStandPet.PetUuid))
        {
            AddDiePet(youStandPet, EnumWho.You);
            newDiePets.Add(youStandPet);
        }

        return newDiePets;
    }
}