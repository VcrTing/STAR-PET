// ════════════════════════════════════════════════════════════════
//  回合运行执行器
//  专门执行 FightRunning 中的各个阶段
// ════════════════════════════════════════════════════════════════

using Godot;
using System.Collections.Generic;

/// <summary>
/// 回合运行执行器
/// 负责遍历和执行 CurrentRunArray 中的每一个 FightRunning 阶段
/// </summary>
public static class FightRunningExe
{
    /// <summary>
    /// 执行所有 FightRunning 阶段，并打印日志
    /// 将 CurrentRunArray 中的有效阶段转为队列，逐个弹出（Dequeue）执行
    /// </summary>
    /// <returns>本回合新死亡的精灵列表（包含双方），可用于判断是否需要进入死亡处理流程</returns>
    public static List<InsFightPetData> ExecuteAll()
    {
        GD.Print($"[FightRunningExe] 开始执行 FightRunning，==================");

        // 1. 将 CurrentRunArray 中所有有效阶段入队
        //    队列保证严格先进先出，顺序与原数组一致
        var queue = new Queue<FightRunning>();
        FightRunning[] runnings = FightRunningHouse.CurrentRunArray;
        for (int i = 0; i < runnings.Length; i++)
        {
            if (runnings[i] != null)
            {
                queue.Enqueue(runnings[i]);
            }
        }

        // 收集你我的本次回合应对（执行前：清理并填充 MyCurrentBingo / YouCurrentBingo）
        FightBingoHouse.RecordBeforeExecute(runnings, EnumWho.My);
        FightBingoHouse.RecordBeforeExecute(runnings, EnumWho.You);

        // 记录本回合开始时存活的精灵 Uuid（用于对比出本回合死亡的精灵）
        var aliveMyUuids = FightAliveHouse.GetAlivePetUuids(EnumWho.My);
        var aliveYouUuids = FightAliveHouse.GetAlivePetUuids(EnumWho.You);

        // 2. 从队列中逐个弹出执行（index 为原始顺序编号，用于日志）
        int index = 0;
        while (queue.Count > 0)
        {
            FightRunning run = queue.Dequeue();

            // 使用 FightRunningTypeDesign 区分 My/You
            bool isMy = FightRunningTypeDesign.IsMyType(run.RunningType);
            bool isYou = FightRunningTypeDesign.IsYouType(run.RunningType);

            if (isMy)
            {
                ExecuteMy(run, index);
            }
            else if (isYou)
            {
                ExecuteYou(run, index);
            }
            else
            {
                ExecuteSingle(run, index);
            }

            // 阶段执行结束后检查是否需要更新 UI（StartXXX 和 GenEndActsXXX）
            if (FightRunningTypeDesign.IsNeedUpdateUi(run.RunningType))
            {
                FightUiUpdateTool.UpdateMyUi();
                FightUiUpdateTool.UpdateYouUi();
            }

            index++;
        }


        // 对比本回合前后的存活列表，收集本回合新死亡的精灵
        var newDiePets = FightAliveHouse.CollectDiePets(aliveMyUuids, aliveYouUuids);

        GD.Print($"[FightRunningExe] FightRunning 执行完毕，本回合死亡 {newDiePets.Count} 只精灵，==================");
        
        return newDiePets;
    }

    /// <summary>
    /// 执行我方（My）阶段
    /// 先执行检查，再分发各阶段
    /// </summary>
    private static void ExecuteMy(FightRunning run, int index)
    {
        if (!FightRunningBetween.CheckHpNice(run))
        {
            GD.Print("My 没血，打断后续操作 ========================");
            return;
        }

        switch (run.RunningType)
        {
            case EnumFightRunningType.DoDamageMy:
                FightRunningExeTool.ExecuteDamage(run, index);
                break;
            case EnumFightRunningType.DoAttackMy:
                FightRunningExeTool.ExecuteDoAttack(run, index);
                break;
            case EnumFightRunningType.DoDefenseMy:
                FightRunningExeTool.ExecuteDoDefense(run, index);
                break;
            case EnumFightRunningType.DoStatusMy:
                FightRunningExeTool.ExecuteDoStatus(run, index);
                break;
            case EnumFightRunningType.SwitchPetMy:
                FightRunningExeSysTool.ExecuteSwitchPet(run, index);
                break;
            case EnumFightRunningType.GenEndActsMy:
                FightRunningExeEndTool.ExecuteGenEndMy(run, index);
                break;
            default:
                ExecuteSingle(run, index);
                break;
        }
    }

    /// <summary>
    /// 执行敌方（You）阶段
    /// 先执行检查，再分发各阶段
    /// </summary>
    private static void ExecuteYou(FightRunning run, int index)
    {
        if (!FightRunningBetween.CheckHpNice(run))
        {
            GD.Print("You 没血，打断后续操作 ========================");
            return;
        }

        switch (run.RunningType)
        {
            case EnumFightRunningType.DoDamageYou:
                FightRunningExeTool.ExecuteDamage(run, index);
                break;
            case EnumFightRunningType.DoAttackYou:
                FightRunningExeTool.ExecuteDoAttack(run, index);
                break;
            case EnumFightRunningType.DoDefenseYou:
                FightRunningExeTool.ExecuteDoDefense(run, index);
                break;
            case EnumFightRunningType.DoStatusYou:
                FightRunningExeTool.ExecuteDoStatus(run, index);
                break;
            case EnumFightRunningType.SwitchPetYou:
                FightRunningExeSysTool.ExecuteSwitchPet(run, index);
                break;
            case EnumFightRunningType.GenEndActsYou:
                FightRunningExeEndTool.ExecuteGenEndYou(run, index);
                break;
            default:
                ExecuteSingle(run, index);
                break;
        }
    }

    /// <summary>
    /// 执行单个 FightRunning 阶段（非扣血类型）
    /// 若为 Start 技能类型（StartStatusMy/StartAttackMy/StartDefenseMy/...），
    /// 则调用 DuckSkillLoader.ExecuteStartSkill 执行技能实现类的 StartSkill 鸭子方法；
    /// 若为 End 技能类型（EndStatusMy/EndAttackMy/EndDefenseMy/...），
    /// 则调用 DuckSkillLoader.ExecuteEndSkill 执行技能实现类的 EndSkill 鸭子方法
    /// </summary>
    private static void ExecuteSingle(FightRunning run, int index)
    {
        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";
        GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                 $"damage={run.Damage} bingoSkillType={run.BingoSkillType} completed={run.IsCompleted}");

        InsFightSkill sideSkill = run.SideFightSkill;
        bool hasSkillImpl = sideSkill?.Skill != null && !string.IsNullOrWhiteSpace(sideSkill.ImplClass);

        // 判断是否为 Start 技能类型，是则执行技能实现类的 StartSkill 鸭子方法
        if (FightRunningTypeDesign.IsStartType(run.RunningType))
        {
            if (hasSkillImpl)
            {
                DuckSkillLoader.ExecuteStartSkill(sideSkill.ImplClass, index, run, sideSkill);
            }
        }
        // 判断是否为 End 技能类型，是则执行技能实现类的 EndSkill 鸭子方法
        else if (FightRunningTypeDesign.IsEndType(run.RunningType))
        {
            if (hasSkillImpl)
            {
                DuckSkillLoader.ExecuteEndSkill(sideSkill.ImplClass, index, run, sideSkill);
            }
        }
    }
}
