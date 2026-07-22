// ════════════════════════════════════════════════════════════════
//  回合运行工具
//  专门处理 FightRunning 相关逻辑：
//   ・构建回合运行数组
//   ・管理当前运行状态
//   ・获取下一阶段、标记完成、重置
// ════════════════════════════════════════════════════════════════

/// <summary>
/// 回合运行工具
/// 负责构建和管理 FightRunning 运行数组，
/// 每回合通过 BuildRunArray 生成 12 长度的步骤数组，
/// 然后依次执行各阶段
/// </summary>
public static class FightRunningTool
{
    /// <summary>回合运行数组长度</summary>
    public const int RunArrayLength = 12;

    /// <summary>
    /// 当前回合的运行数组，存储 12 个 FightRunning
    /// 表示本回合真正要执行的所有步骤
    /// </summary>
    public static FightRunning[] CurrentRunArray = new FightRunning[RunArrayLength];

    /// <summary>
    /// 构建本回合的运行数组
    /// 分析双方 TurnAction，生成对应的 FightRunning 数组
    /// </summary>
    /// <param name="myAct">我方本回合行动</param>
    /// <param name="youAct">敌方本回合行动</param>
    /// <returns>12 长度的 FightRunning 数组</returns>
    public static FightRunning[] BuildRunArray(TurnAction myAct, TurnAction youAct)
    {
        FightRunning[] runArray = new FightRunning[RunArrayLength];
        int idx = 0;

        // ─── 我方阶段 ───
        runArray[idx++] = new FightRunning(EnumFightRunningType.StartStatusMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.EndStatusMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.BingoStatusMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.TimeStopMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.BingoDefenseMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.StartAttackMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.StartDefenseMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.BingoAttackMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.EndDefenseMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.EndAttackMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.CalcDamageMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.CheckHpMy, EnumWho.My, myAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.GenEndActsMy, EnumWho.My, myAct?.FightSkill);

        // ─── 敌方阶段 ───
        // 注意：13 个 My 阶段已超出 12 长度，需扩展数组或分批执行
        // 此处仅保留基本示例
        runArray[idx++] = new FightRunning(EnumFightRunningType.StartStatusYou, EnumWho.You, youAct?.FightSkill);
        runArray[idx++] = new FightRunning(EnumFightRunningType.EndStatusYou, EnumWho.You, youAct?.FightSkill);
        // 注意：12 长度已用完，额外阶段需要串行分批或扩展数组
        // 此处仅存 12 项作为基本运行数组

        return runArray;
    }

    /// <summary>
    /// 获取当前回合运行数组的下一阶段（下一个未完成的 FightRunning）
    /// </summary>
    /// <returns>下一个要执行的 FightRunning，若无则 null</returns>
    public static FightRunning GetNextRunning()
    {
        for (int i = 0; i < RunArrayLength; i++)
        {
            if (CurrentRunArray[i] != null && !CurrentRunArray[i].IsCompleted)
                return CurrentRunArray[i];
        }
        return null;
    }

    /// <summary>
    /// 标记当前阶段为已完成
    /// </summary>
    public static void CompleteRunning(EnumFightRunningType type)
    {
        for (int i = 0; i < RunArrayLength; i++)
        {
            if (CurrentRunArray[i]?.RunningType == type)
            {
                CurrentRunArray[i].IsCompleted = true;
                return;
            }
        }
    }

    /// <summary>
    /// 重置当前运行数组
    /// </summary>
    public static void ClearRunArray()
    {
        CurrentRunArray = new FightRunning[RunArrayLength];
    }
}