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
        GD.Print($"[FightRunningExe] 开始执行 FightRunning，==================");

        for (int i = 0; i < runnings.Length; i++)
        {
            FightRunning run = runnings[i];
            if (run == null) continue;

            // 使用 FightRunningTypeDesign 区分 My/You
            bool isMy = FightRunningTypeDesign.IsMyType(run.RunningType);
            bool isYou = FightRunningTypeDesign.IsYouType(run.RunningType);

            if (isMy)
            {
                ExecuteMy(run, i);
            }
            else if (isYou)
            {
                ExecuteYou(run, i);
            }
            else
            {
                ExecuteSingle(run, i);
            }
        }

        GD.Print($"[FightRunningExe] FightRunning 执行完毕，==================");
    }

    /// <summary>
    /// 执行我方（My）阶段
    /// </summary>
    private static void ExecuteMy(FightRunning run, int index)
    {
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
                bool canMy = FightRunningBetween.CheckHpNice(run);
                if (canMy)
                    FightRunningExeSysTool.ExecuteSwitchPet(run, index);
                break;
            default:
                ExecuteSingle(run, index);
                break;
        }
    }

    /// <summary>
    /// 执行敌方（You）阶段
    /// </summary>
    private static void ExecuteYou(FightRunning run, int index)
    {
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
                bool canYou = FightRunningBetween.CheckHpNice(run);
                if (canYou)
                    FightRunningExeSysTool.ExecuteSwitchPet(run, index);
                break;
            default:
                ExecuteSingle(run, index);
                break;
        }
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