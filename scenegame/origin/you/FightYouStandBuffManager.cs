using Godot;
using System.Collections.Generic;

public partial class FightYouStandBuffManager : Node2D
{
    private static FightYouStandBuffManager _instance;
    public static FightYouStandBuffManager Instance => _instance;

    /// <summary>
    /// Buff 字典，key = 精灵 PetUuid，value = 属于该精灵的 Buff 列表
    /// </summary>
    private Dictionary<string, List<InsFightBuff>> _buffsDict = new();

    public override void _EnterTree()
    {
        if (_instance != null) { QueueFree(); return; }
        _instance = this;
    }

    public override void _ExitTree()
    {
        if (_instance == this) _instance = null;
    }

    /// <summary>
    /// 传入一组 Buff，根据每个 Buff 的 PetUuid 存入对应精灵的 Buff 列表
    /// 无 PetUuid 的 Buff 跳过
    /// </summary>
    /// <param name="newBuffs">要添加的 Buff 数组</param>
    public void AddBuffs(InsFightBuff[] newBuffs)
    {
        if (newBuffs == null || newBuffs.Length == 0)
            return;

        GD.Print($"[FightYouStandBuffManager] AddBuffs: {newBuffs.Length} 个 Buff 加入");

        foreach (var buff in newBuffs)
        {
            if (buff == null || string.IsNullOrWhiteSpace(buff.PetUuid))
                continue;

            if (!_buffsDict.ContainsKey(buff.PetUuid))
                _buffsDict[buff.PetUuid] = new List<InsFightBuff>();

            _buffsDict[buff.PetUuid].Add(buff);
        }

        // 合并相同 Stat 的 Buff（同一精灵内）
        foreach (var kvp in _buffsDict)
        {
            kvp.Value.Sort((a, b) => a.Stat.CompareTo(b.Stat));
        }

        // 更新当前场上精灵的 Buff 视图
        RefreshCurrentView();
    }

    /// <summary>
    /// 获取指定精灵的所有 Buff 数组
    /// </summary>
    /// <param name="petUuid">精灵 UUID</param>
    /// <returns>Buff 数组，无 Buff 返回空数组</returns>
    public InsFightBuff[] GetBuffsByPetUuid(string petUuid)
    {
        if (string.IsNullOrWhiteSpace(petUuid) || !_buffsDict.ContainsKey(petUuid))
            return System.Array.Empty<InsFightBuff>();

        return _buffsDict[petUuid].ToArray();
    }

    /// <summary>
    /// 获取当前敌方场上精灵的所有 Buff
    /// </summary>
    public InsFightBuff[] GetCurrentPetBuffs()
    {
        string petUuid = FightLandYouStandPet.Instance?.FightPetData?.PetUuid;
        return GetBuffsByPetUuid(petUuid);
    }

    /// <summary>
    /// 刷新 UI 视图，显示当前敌方场上精灵的 Buff
    /// </summary>
    public void RefreshCurrentView()
    {
        if (VBoxViewBuffsContentYou.Instance != null)
        {
            VBoxViewBuffsContentYou.Instance.UpdateBuffs(GetCurrentPetBuffs());
        }
    }

    /// <summary>
    /// 精灵离场时调用，清除该精灵所有 ThisPetAppear 类型的 Buff
    /// </summary>
    /// <param name="pet">离场精灵数据</param>
    public void RemoveThisPetAppearBuffs(InsFightPetData pet)
    {
        if (pet == null || string.IsNullOrWhiteSpace(pet.PetUuid))
            return;

        if (!_buffsDict.ContainsKey(pet.PetUuid))
            return;

        var list = _buffsDict[pet.PetUuid];
        int removed = list.RemoveAll(buff => buff != null && buff.ActiveMode == EnumBuffActiveMode.ThisPetAppear);

        if (removed > 0)
        {
            GD.Print($"[FightYouStandBuffManager] 精灵 [{pet.PetName}] 离场，清除 ThisPetAppear Buff {removed} 个");

            // 如果该精灵的 Buff 列表为空，则删除该键
            if (list.Count == 0)
                _buffsDict.Remove(pet.PetUuid);

            // 刷新当前场上精灵的 Buff 视图
            RefreshCurrentView();
        }
    }

    /// <summary>
    /// 根据传入的精灵，计算所有 Buff 对该精灵各项个体值的总加成值
    /// </summary>
    /// <param name="pet">目标精灵数据</param>
    /// <returns>stat -> 总加成值 的字典（纯加值，不含基础值）</returns>
    public Dictionary<EnumPetBaseStats, int> CalculateBuffStats(InsFightPetData pet)
    {
        var result = new Dictionary<EnumPetBaseStats, int>();

        if (pet == null || string.IsNullOrWhiteSpace(pet.PetUuid))
            return result;

        if (!_buffsDict.ContainsKey(pet.PetUuid))
            return result;

        foreach (var buff in _buffsDict[pet.PetUuid])
        {
            if (buff == null)
                continue;

            int totalValue = buff.Layer * buff.Value;

            if (result.ContainsKey(buff.Stat))
                result[buff.Stat] += totalValue;
            else
                result[buff.Stat] = totalValue;
        }

        return result;
    }
}