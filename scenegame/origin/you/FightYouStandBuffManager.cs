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
    /// 同类 Buff（相同 Stat、IsRatio、ActiveMode）自动叠加层数
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

            // 查找是否已有同类 Buff（相同 Stat、Value、IsRatio）
            InsFightBuff existing = _buffsDict[buff.PetUuid].Find(
                b => b.Stat == buff.Stat && b.Value == buff.Value && b.IsRatio == buff.IsRatio);

            if (existing != null)
            {
                // 同类 Buff：叠加层数（Value 固定不变）
                existing.Layer += buff.Layer;
                GD.Print($"      [FightYouStandBuffManager] 同类 Buff 叠加: Stat={buff.Stat} Layer={existing.Layer} Value={existing.Value}");
            }
            else
            {
                // 不同类 Buff：直接添加
                _buffsDict[buff.PetUuid].Add(buff);
            }
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
    /// 精灵登场时调用，将传入的精灵所拥有的 Buff 更新到 UI 视图
    /// </summary>
    /// <param name="pet">登场精灵数据</param>
    public void WhenPetAppear(InsFightPetData pet)
    {
        if (pet == null || string.IsNullOrWhiteSpace(pet.PetUuid))
            return;

        // 将该精灵的 Buff 更新到 UI
        if (VBoxViewBuffsContentYou.Instance != null)
        {
            InsFightBuff[] buffs = GetBuffsByPetUuid(pet.PetUuid);
            VBoxViewBuffsContentYou.Instance.UpdateBuffs(buffs);
        }
    }

    /// <summary>
    /// 精灵离场时调用，清除该精灵除 ThisPetPermanent（永久）之外的所有 Buff
    /// </summary>
    /// <param name="pet">离场精灵数据</param>
    public void WhenPetDisAppear(InsFightPetData pet)
    {
        if (pet == null || string.IsNullOrWhiteSpace(pet.PetUuid))
            return;

        if (!_buffsDict.ContainsKey(pet.PetUuid))
            return;

        var list = _buffsDict[pet.PetUuid];
        int removed = list.RemoveAll(buff => buff != null && buff.ActiveMode != EnumBuffActiveMode.ThisPetPermanent);

        if (removed > 0)
        {
            GD.Print($"[FightYouStandBuffManager] 精灵 [{pet.PetName}] 离场，清除非永久 Buff {removed} 个");

            // 如果该精灵的 Buff 列表只剩永久 Buff 或为空，则删除该键（永久 Buff 随精灵永存，但效果保留到切换）
            if (list.Count == 0)
                _buffsDict.Remove(pet.PetUuid);

            // 刷新当前场上精灵的 Buff 视图
            RefreshCurrentView();
        }
    }
}