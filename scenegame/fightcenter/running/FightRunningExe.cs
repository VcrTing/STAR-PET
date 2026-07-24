// ════════════════════════════════════════════════════════════════
//  回合运行执行器
//  专门执行 FightRunning 中的各个阶段
// ════════════════════════════════════════════════════════════════

using Godot;

/// <summary>
/// 回合运行执行器
/// 负责遍历和执行 CurrentRunArray 中的每一个 FightRunning 阶段
/// </summary>
public static class FightRunningExe
{
    /// <summary>
    /// 执行所有 FightRunning 阶段，并打印日志
    /// </summary>
    public static void ExecuteAll()
    {
        FightRunning[] runnings = FightRunningHouse.CurrentRunArray;
        GD.Print($"    └─ [FightRunningExe] 开始执行 FightRunning，共 {runnings.Length} 个阶段");

        for (int i = 0; i < runnings.Length; i++)
        {
            FightRunning run = runnings[i];
            if (run == null)
                continue;

            // 扣血环节：DoDamageMy / DoDamageYou → 执行扣血
            if (run.RunningType == EnumFightRunningType.DoDamageMy
                || run.RunningType == EnumFightRunningType.DoDamageYou)
            {
                FightRunningExeTool.ExecuteDamage(run, i);
            }
            // 状态环节：StartStatusMy / StartStatusYou → 执行状态技能效果
            else if (run.RunningType == EnumFightRunningType.StartStatusMy
                || run.RunningType == EnumFightRunningType.StartStatusYou)
            {
                FightRunningExeTool.ExecuteStatus(run, i);
            }
            else
            {
                ExecuteSingle(run, i);
            }
        }

        GD.Print($"    └─ [FightRunningExe] FightRunning 执行完毕");
    }


    /// <summary>
    /// 执行单个 FightRunning 阶段（非扣血类型）
    /// </summary>
    private static void ExecuteSingle(FightRunning run, int index)
    {
        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";
        GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                 $"damage={run.Damage} bingoSkillType={run.BingoSkillType} completed={run.IsCompleted}");
    }
}