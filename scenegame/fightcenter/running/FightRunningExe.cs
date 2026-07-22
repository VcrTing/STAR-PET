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
                ExecuteDamage(run, i);
            }
            else
            {
                ExecuteSingle(run, i);
            }
        }

        GD.Print($"    └─ [FightRunningExe] FightRunning 执行完毕");
    }

    /// <summary>
    /// 执行扣血阶段：根据 FightRunning.Damage 扣除对应方精灵的 HP
    /// </summary>
    private static void ExecuteDamage(FightRunning run, int index)
    {
        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";

        // 获取要扣血的精灵
        InsFightPetData targetPet = run.Side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        if (targetPet == null)
        {
            GD.Print($"      [{index}] {sideLabel} {run.RunningType} | 目标精灵为空，跳过扣血");
            return;
        }

        // 执行扣血
        int actualDamage = run.Damage;
        targetPet.Hp = Mathf.Max(targetPet.Hp - actualDamage, 0);

        GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                 $"damage={actualDamage} {targetPet.PetName} HP: {targetPet.Hp}/{targetPet.MaxHp} " +
                 $"bingo={run.IsBingo} bSkillType={run.BingoSkillType}");
    }

    /// <summary>
    /// 执行单个 FightRunning 阶段（非扣血类型）
    /// </summary>
    private static void ExecuteSingle(FightRunning run, int index)
    {
        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";
        GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                 $"damage={run.Damage} bingo={run.IsBingo} " +
                 $"bSkillType={run.BingoSkillType} completed={run.IsCompleted}");
    }
}