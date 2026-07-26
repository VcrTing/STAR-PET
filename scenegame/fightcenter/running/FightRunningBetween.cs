using Godot;
using System;

/// <summary>
/// 回合运行中间处理类
/// 每个 FightRunning 阶段之间要执行的逻辑
/// 用于 Running 阶段的中间数据处理、状态刷新等
/// </summary>
public static class FightRunningBetween
{
    /// <summary>
    /// 在两个 Running 阶段之间调用
    /// 在 ExecuteAll 的每个 Running 循环中，每个阶段执行后调用
    /// </summary>
    /// <param name="index">当前执行的索引</param>
    /// <param name="totalCount">总执行数</param>
    /// <param name="run">当前正在执行的 Running 数据</param>
    public static void ExecuteBetween(int index, int totalCount, FightRunning run)
    {
        if (run == null) return;
    }

    public static void RunningWrapper(FightRunning run)
    {
        if (run == null) return;

        // 检查当前方精灵血量
        CheckHpNice(run);

        // 再执行
    }

    /// <summary>
    /// 检查当前 Running side 的场上精灵血量
    /// 血量 <= 0 时打印警告日志
    /// </summary>
    public static bool CheckHpNice(FightRunning run)
    {
        if (run == null) return false;

        InsFightPetData pet = run.Side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        if (pet == null) return false;

        if (pet.Hp <= 0)
        {
            string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";
            GD.Print($"      ⚠ [FightRunningBetween.CheckHp] {sideLabel} {pet.PetName} HP={pet.Hp}/{pet.MaxHp} 已归零！");
            return false;
        }
        return true;
    }

}