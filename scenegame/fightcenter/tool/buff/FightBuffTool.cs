using Godot;
using System.Collections.Generic;

/// <summary>
/// 战斗 Buff 计算工具
/// 提供 Buff 对精灵各项个体值的总加成值计算方法
/// </summary>
public static class FightBuffTool
{
    /// <summary>
    /// 根据传入的精灵，计算所有 Buff 对该精灵各项个体值的总加成值
    /// </summary>
    /// <param name="pet">目标精灵数据</param>
    /// <returns>stat -> 总加成值 的字典（纯加值，不含基础值）</returns>
    public static Dictionary<EnumPetBaseStats, int> CalculateBuffStats(InsFightPetData pet)
    {
        var result = new Dictionary<EnumPetBaseStats, int>();

        if (pet == null || string.IsNullOrWhiteSpace(pet.PetUuid))
            return result;

        InsFightBuff[] buffs = GetBuffsByPetUuid(pet.PetUuid);
        if (buffs == null || buffs.Length == 0)
            return result;

        foreach (var buff in buffs)
        {
            if (buff == null)
                continue;

            int totalValue = CalcBuffValue(buff, pet);

            if (result.ContainsKey(buff.Stat))
                result[buff.Stat] += totalValue;
            else
                result[buff.Stat] = totalValue;
        }

        return result;
    }

    /// <summary>
    /// 计算单个 Buff 对指定精灵的加成值
    /// 百分比（IsRatio=true）：基于精灵该项最终个体值 × 层数 × 百分比值 / 100
    /// 纯加值（IsRatio=false）：层数 × 每层数值
    /// </summary>
    /// <param name="buff">Buff 数据</param>
    /// <param name="pet">精灵数据（用于百分比计算时获取基础值）</param>
    /// <returns>计算后的加成值</returns>
    private static int CalcBuffValue(InsFightBuff buff, InsFightPetData pet)
    {
        if (buff.IsRatio)
        {
            int baseStat = pet.FinalStats != null && pet.FinalStats.TryGetValue(buff.Stat, out int val) ? val : 0;
            return (int)(baseStat * (buff.Layer * buff.Value / 100.0f));
        }

        return buff.Layer * buff.Value;
    }

    /// <summary>
    /// 根据精灵 UUID 从对应的 BuffManager 获取 Buff 列表
    /// 优先查找我方（FightMyStandBuffManager），再查找敌方（FightYouStandBuffManager）
    /// </summary>
    private static InsFightBuff[] GetBuffsByPetUuid(string petUuid)
    {
        // 尝试我方
        InsFightBuff[] buffs = FightMyStandBuffManager.Instance?.GetBuffsByPetUuid(petUuid);
        if (buffs != null && buffs.Length > 0)
            return buffs;

        // 尝试敌方
        buffs = FightYouStandBuffManager.Instance?.GetBuffsByPetUuid(petUuid);
        if (buffs != null && buffs.Length > 0)
            return buffs;

        return System.Array.Empty<InsFightBuff>();
    }

    /// <summary>
    /// 将 Buff 加成值叠加到基础属性字典上
    /// </summary>
    /// <param name="baseStats">基础属性字典（会被修改）</param>
    /// <param name="buffStats">Buff 加成差值字典</param>
    public static void MergeBuffStats(Dictionary<EnumPetBaseStats, int> baseStats, Dictionary<EnumPetBaseStats, int> buffStats)
    {
        if (baseStats == null || buffStats == null)
            return;

        foreach (var kvp in buffStats)
        {
            if (baseStats.ContainsKey(kvp.Key))
                baseStats[kvp.Key] += kvp.Value;
            else
                baseStats[kvp.Key] = kvp.Value;
        }
    }
}