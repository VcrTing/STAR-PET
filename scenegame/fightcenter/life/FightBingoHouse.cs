using Godot;
using System.Collections.Generic;

/// <summary>
/// 战斗应对管理容器（Fight Bingo House）
/// 存放本次回合与上次回合的应对阶段类型数组，供技能判断应对/追溯使用
/// </summary>
public static class FightBingoHouse
{
    // ════════════════════════ 本次回合应对 ════════════════════════

    /// <summary>
    /// 我方本次回合应对阶段类型数组
    /// 记录本次回合我方产生的应对（如 BingoAttackMy / BingoDefenseMy / BingoStatusMy / BingoSwitchPetMy 等）
    /// </summary>
    public static List<EnumFightRunningType> MyCurrentBingo { get; private set; } = new();

    /// <summary>
    /// 敌方本次回合应对阶段类型数组
    /// 记录本次回合敌方产生的应对（如 BingoAttackYou / BingoDefenseYou / BingoStatusYou / BingoSwitchPetYou 等）
    /// </summary>
    public static List<EnumFightRunningType> YouCurrentBingo { get; private set; } = new();

    // ════════════════════════ 上次回合应对 ════════════════════════

    /// <summary>
    /// 我方上次回合应对阶段类型数组
    /// 记录上一回合我方产生的应对，供跨回合技能效果追溯使用
    /// </summary>
    public static List<EnumFightRunningType> MyLastBingo { get; private set; } = new();

    /// <summary>
    /// 敌方上次回合应对阶段类型数组
    /// 记录上一回合敌方产生的应对，供跨回合技能效果追溯使用
    /// </summary>
    public static List<EnumFightRunningType> YouLastBingo { get; private set; } = new();

    // ════════════════════════ 添加方法 ════════════════════════

    /// <summary>
    /// 按 side 向对应方本次回合应对数组添加一个应对阶段类型
    /// </summary>
    /// <param name="side">所属方（My/You）</param>
    /// <param name="type">应对阶段类型</param>
    public static void Add(EnumWho side, EnumFightRunningType type)
    {
        if (side == EnumWho.My)
            MyCurrentBingo.Add(type);
        else
            YouCurrentBingo.Add(type);
    }

    /// <summary>
    /// 按 side 向对应方本次回合应对数组批量添加应对阶段类型
    /// </summary>
    public static void AddRange(EnumWho side, IEnumerable<EnumFightRunningType> types)
    {
        if (types == null) return;
        if (side == EnumWho.My)
            MyCurrentBingo.AddRange(types);
        else
            YouCurrentBingo.AddRange(types);
    }

    /// <summary>
    /// 添加"应对切换宠物"到指定方（side）的本场回合应对数组（MyCurrentBingo / YouCurrentBingo）
    /// My 方应对切换宠物 => 加入 BingoSwitchPetMy
    /// You 方应对切换宠物 => 加入 BingoSwitchPetYou
    /// 由技能 RebuildTurn 检测到对方切换宠物时调用
    /// </summary>
    /// <param name="side">所属方（My/You）</param>
    public static void AddSwitchPetBingo(EnumWho side)
    {
        if (side == EnumWho.My)
            Add(EnumWho.My, EnumFightRunningType.BingoSwitchPetMy);
        else
            Add(EnumWho.You, EnumFightRunningType.BingoSwitchPetYou);
    }

    // ════════════════════════ 查询方法 ════════════════════════

    /// <summary>
    /// 获取指定方的本次回合应对阶段数组
    /// </summary>
    public static EnumFightRunningType[] GetCurrentBingo(EnumWho side)
    {
        return side == EnumWho.My ? MyCurrentBingo.ToArray() : YouCurrentBingo.ToArray();
    }

    /// <summary>
    /// 获取指定方的上次回合应对阶段数组
    /// </summary>
    public static EnumFightRunningType[] GetLastBingo(EnumWho side)
    {
        return side == EnumWho.My ? MyLastBingo.ToArray() : YouLastBingo.ToArray();
    }

    /// <summary>
    /// 判断指定方本次回合是否产生过某类应对
    /// </summary>
    public static bool HasCurrentBingo(EnumWho side, EnumFightRunningType type)
    {
        return side == EnumWho.My ? MyCurrentBingo.Contains(type) : YouCurrentBingo.Contains(type);
    }

    /// <summary>
    /// 判断指定方本次回合是否"应对切换宠物成功"
    /// 即 Current 应对数组中是否包含 BingoSwitchPetMy / BingoSwitchPetYou
    /// </summary>
    /// <param name="side">所属方（My/You）</param>
    /// <returns>true=本次回合应对切换宠物成功</returns>
    public static bool HasCurrentSwitchPetBingo(EnumWho side)
    {
        return side == EnumWho.My
            ? MyCurrentBingo.Contains(EnumFightRunningType.BingoSwitchPetMy)
            : YouCurrentBingo.Contains(EnumFightRunningType.BingoSwitchPetYou);
    }

    /// <summary>
    /// 判断指定方上次回合是否产生过某类应对
    /// </summary>
    public static bool HasLastBingo(EnumWho side, EnumFightRunningType type)
    {
        return side == EnumWho.My ? MyLastBingo.Contains(type) : YouLastBingo.Contains(type);
    }

    // ════════════════════════ 回合执行记录 ════════════════════════

    /// <summary>
    /// 回合执行前调用：扫描传入的 FightRunning 数组，收集指定方（side）的应对阶段
    /// 1. 本方产生的 Bingo 应对阶段（BingoAttack/Defense/Status/SwitchPet，由 IsBingoType 判断）
    /// 2. 对方切换宠物（SwitchPetMy/You）=> 本方应对切换宠物成功，加入 BingoSwitchPetMy/You
    /// 先清空本方本次回合应对数组，再全部加入（供本次回合技能判断应对使用）
    /// </summary>
    /// <param name="runnings">本次回合的 FightRunning 数组</param>
    /// <param name="side">要记录的所属方（My/You）</param>
    public static void RecordBeforeExecute(FightRunning[] runnings, EnumWho side)
    {
        if (runnings == null) return;

        foreach (FightRunning run in runnings)
        {
            if (run == null) continue;

            // 1. 本方产生的 Bingo 应对阶段（IsBingoType 带 side 精确区分 My/You）
            if (run.Side == side && FightRunningTypeDesign.IsBingoType(side, run.RunningType))
                Add(side, run.RunningType);

            // 2. 对方切换宠物 => 本方应对切换宠物成功
            if (IsSwitchPetBingoFor(side, run.RunningType))
                Add(side, side == EnumWho.My ? EnumFightRunningType.BingoSwitchPetMy : EnumFightRunningType.BingoSwitchPetYou);
        }
    }

    /// <summary>
    /// 回合执行结束后调用：将本次回合应对数组移入上次回合应对数组，并清空本次回合应对
    /// （不重新扫描 FightRunning，直接复用 RecordBeforeExecute 已填充的 Current 数组）
    /// </summary>
    /// <param name="runnings">本次回合已执行的 FightRunning 数组（此方法不再使用该参数）</param>
    /// <param name="side">要记录的所属方（My/You）</param>
    public static void RecordAfterExecute(EnumWho side)
    {
        if (side == EnumWho.My)
        {
            // MyCurrentBingo → MyLastBingo，再清空 MyCurrentBingo
            MyLastBingo.Clear();
            MyLastBingo.AddRange(MyCurrentBingo);
            MyCurrentBingo.Clear();
        }
        else
        {
            // YouCurrentBingo → YouLastBingo，再清空 YouCurrentBingo
            YouLastBingo.Clear();
            YouLastBingo.AddRange(YouCurrentBingo);
            YouCurrentBingo.Clear();
        }
    }

    /// <summary>
    /// 判断指定方是否"应对切换宠物成功"
    /// 规则：对方切换宠物（SwitchPetMy/You）即视为本方应对切换宠物成功
    /// </summary>
    /// <param name="side">本方（My/You）</param>
    /// <param name="runningType">对方切换宠物的运行阶段类型</param>
    /// <returns>true=应对切换宠物成功</returns>
    private static bool IsSwitchPetBingoFor(EnumWho side, EnumFightRunningType runningType)
    {
        // 对方是 My（我方切换宠物）→ 本方 You 应对切换宠物成功
        if (side == EnumWho.You && runningType == EnumFightRunningType.SwitchPetMy)
            return true;
        // 对方是 You（敌方切换宠物）→ 本方 My 应对切换宠物成功
        if (side == EnumWho.My && runningType == EnumFightRunningType.SwitchPetYou)
            return true;
        return false;
    }

    // ════════════════════════ 回合切换 ════════════════════════

    /// <summary>
    /// 回合切换：将本次回合应对移入上次回合，并清空本次回合应对
    /// 在回合开始时调用，供跨回合技能效果追溯使用
    /// </summary>
    public static void NextTurn()
    {
        MyLastBingo = new List<EnumFightRunningType>(MyCurrentBingo);
        YouLastBingo = new List<EnumFightRunningType>(YouCurrentBingo);
        MyCurrentBingo.Clear();
        YouCurrentBingo.Clear();
    }

    // ════════════════════════ 清空 ════════════════════════

    /// <summary>
    /// 清空所有应对数组（战斗初始化/结束后调用）
    /// </summary>
    public static void Clear()
    {
        MyCurrentBingo.Clear();
        YouCurrentBingo.Clear();
        MyLastBingo.Clear();
        YouLastBingo.Clear();
    }
}