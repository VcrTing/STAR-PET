using Godot;
using System;

/// <summary>
/// 行动执行中间处理类
/// 每个 Exe（单个行动执行）之间要执行的逻辑
/// 例如换宠后的数据同步、状态刷新、血量检测等
/// </summary>
public static class FightExeBetween
{
    /// <summary>
    /// 在两个行动执行之间调用
    /// 在 ExecuteAll 的每个 Running 阶段之间执行
    /// </summary>
    /// <param name="index">当前执行的索引</param>
    /// <param name="totalCount">总执行数</param>
    /// <param name="run">当前正在执行的 Running 数据</param>
    public static void ExecuteBetween(int index, int totalCount, FightRunning run)
    {
        if (run == null) return;

        // ── 换宠后的数据同步 ──
        if (IsSwitchPetType(run.RunningType))
        {
            AfterSwitchPet(run);
        }
    }

    /// <summary>
    /// 判断是否为切换宠物类阶段
    /// </summary>
    private static bool IsSwitchPetType(EnumFightRunningType type)
    {
        return type == EnumFightRunningType.SwitchPetMy
            || type == EnumFightRunningType.SwitchPetYou;
    }

    /// <summary>
    /// 切换宠物后的处理
    /// 刷新 UI 显示的血量、能量等数据
    /// </summary>
    private static void AfterSwitchPet(FightRunning run)
    {
        if (run.SwitchPet == null) return;

        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";
        GD.Print($"      [FightExeBetween] {sideLabel} 换宠后同步: {run.SwitchPet.PetName}");

        // 刷新详情面板
        VBoxPetMsgContent.Instance?.UpdatePetData(run.SwitchPet);

        // 刷新技能 UI
        if (run.SwitchPet.FightSkills != null && run.SwitchPet.FightSkills.Count > 0)
        {
            UiHBoxSkillsManager.Instance?.SwitchSkills(run.SwitchPet.FightSkills);
        }
    }
}