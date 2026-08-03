using Godot;
using System.Collections.Generic;

/// <summary>
/// 我方战斗中能力（Power）管理器
/// 与 FightMyStandBuffManager 结构一致，管理"能力"而非"个体值 Buff"
/// </summary>
public partial class FightMyStandPowerManager : Node2D
{
    private static FightMyStandPowerManager _instance;
    public static FightMyStandPowerManager Instance => _instance;

    /// <summary>
    /// 能力字典，key = 精灵 PetUuid，value = 属于该精灵的能力列表
    /// </summary>
    private Dictionary<string, List<InsFightPower>> _powersDict = new();

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
    /// 传入一组能力，根据每个能力的 PetUuid 存入对应精灵的能力列表
    /// 无 PetUuid 的能力跳过
    /// 同类能力（相同 PowerType、Value、IsRatio）自动叠加层数
    /// </summary>
    /// <param name="newPowers">要添加的能力数组</param>
    public void AddPowers(InsFightPower[] newPowers)
    {
        if (newPowers == null || newPowers.Length == 0)
            return;

        GD.Print($"[FightMyStandPowerManager] AddPowers: {newPowers.Length} 个能力加入");

        foreach (var power in newPowers)
        {
            if (power == null || string.IsNullOrWhiteSpace(power.PetUuid))
                continue;

            if (!_powersDict.ContainsKey(power.PetUuid))
                _powersDict[power.PetUuid] = new List<InsFightPower>();

            // 查找是否已有同类能力（相同 PowerType、Value、IsRatio）
            InsFightPower existing = _powersDict[power.PetUuid].Find(
                p => p.PowerType == power.PowerType && p.Value == power.Value && p.IsRatio == power.IsRatio);

            if (existing != null)
            {
                // 同类能力：叠加层数（Value 固定不变）
                existing.Layer += power.Layer;
                GD.Print($"      [FightMyStandPowerManager] 同类能力叠加: PowerType={power.PowerType} Layer={existing.Layer} Value={existing.Value}");
            }
            else
            {
                // 不同类能力：直接添加
                _powersDict[power.PetUuid].Add(power);
            }
        }

        // 更新当前场上精灵的 Power 视图
        RefreshCurrentView();
    }

    /// <summary>
    /// 获取指定精灵的所有能力数组
    /// </summary>
    /// <param name="petUuid">精灵 UUID</param>
    /// <returns>能力数组，无能力返回空数组</returns>
    public InsFightPower[] GetPowersByPetUuid(string petUuid)
    {
        if (string.IsNullOrWhiteSpace(petUuid) || !_powersDict.ContainsKey(petUuid))
            return System.Array.Empty<InsFightPower>();

        return _powersDict[petUuid].ToArray();
    }

    /// <summary>
    /// 获取当前我方场上精灵的所有能力
    /// </summary>
    public InsFightPower[] GetCurrentPetPowers()
    {
        string petUuid = FightLandMyStandPet.Instance?.FightPetData?.PetUuid;
        return GetPowersByPetUuid(petUuid);
    }

    /// <summary>
    /// 获取指定精灵的连击数 Power 加成（ComboCount 层数之和）
    /// 用于连击技能生成时叠加连击次数
    /// </summary>
    /// <param name="petUuid">精灵 UUID</param>
    /// <returns>连击数加成（无则返回0）</returns>
    public int GetComboCount(string petUuid)
    {
        if (string.IsNullOrWhiteSpace(petUuid) || !_powersDict.ContainsKey(petUuid))
            return 0;

        int totalCombo = 0;
        var list = _powersDict[petUuid];
        for (int i = 0; i < list.Count; i++)
        {
            InsFightPower power = list[i];
            if (power != null && power.PowerType == EnumFightPowerType.ComboCount)
            {
                totalCombo += power.Layer * power.Value;
            }
        }
        return totalCombo;
    }

    /// <summary>
    /// 刷新 UI 视图，显示当前场上精灵的 Power
    /// </summary>
    public void RefreshCurrentView()
    {
        if (VBoxViewBuffsContentMy.Instance != null)
        {
            VBoxViewBuffsContentMy.Instance.UpdatePowers(GetCurrentPetPowers());
        }
    }

    /// <summary>
    /// 精灵登场时调用，将传入的精灵所拥有的 Power 更新到 UI 视图
    /// </summary>
    /// <param name="pet">登场精灵数据</param>
    public void WhenPetAppear(InsFightPetData pet)
    {
        if (pet == null || string.IsNullOrWhiteSpace(pet.PetUuid))
            return;

        // 将该精灵的 Power 更新到 UI
        if (VBoxViewBuffsContentMy.Instance != null)
        {
            InsFightPower[] powers = GetPowersByPetUuid(pet.PetUuid);
            VBoxViewBuffsContentMy.Instance.UpdatePowers(powers);
        }
    }

    /// <summary>
    /// 精灵离场时调用，清除该精灵除 ThisPetPermanent（永久）之外的所有能力
    /// </summary>
    /// <param name="pet">离场精灵数据</param>
    public void WhenPetDisAppear(InsFightPetData pet)
    {
        if (pet == null || string.IsNullOrWhiteSpace(pet.PetUuid))
            return;

        if (!_powersDict.ContainsKey(pet.PetUuid))
            return;

        var list = _powersDict[pet.PetUuid];
        int removed = list.RemoveAll(power => power != null && power.ActiveMode != EnumBuffActiveMode.ThisPetPermanent);

        if (removed > 0)
        {
            GD.Print($"[FightMyStandPowerManager] 精灵 [{pet.PetName}] 离场，清除非永久能力 {removed} 个");

            if (list.Count == 0)
                _powersDict.Remove(pet.PetUuid);

            // 刷新当前场上精灵的 Power 视图
            RefreshCurrentView();
        }
    }
}