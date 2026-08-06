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
    /// 阶段执行队列（类内字段，ExecuteAll 执行完成后清空）
    /// 队列保证严格先进先出，顺序与原数组一致
    /// </summary>
    private static readonly Queue<FightRunning> _queue = new();

    /// <summary>
    /// 执行所有 FightRunning 阶段，并打印日志
    /// 将 CurrentRunArray 中的有效阶段转为队列，逐个弹出（Dequeue）执行
    /// </summary>
    /// <returns>本回合新死亡的精灵列表（包含双方），可用于判断是否需要进入死亡处理流程</returns>
    public static List<InsFightPetData> ExecuteAll()
    {
        GD.Print($"[FightRunningExe] 开始执行 FightRunning，==================");

        // 1. 获取 CurrentRunArray 并通过"阻断打断"过滤（移除被阻断的 DoStatusXX 阶段）
        FightRunning[] runnings = BlockIntercept(FightRunningHouse.CurrentRunArray);

        // 2. 清空上一次队列，将过滤后的有效阶段入队
        _queue.Clear();
        for (int i = 0; i < runnings.Length; i++)
        {
            if (runnings[i] != null)
            {
                _queue.Enqueue(runnings[i]);
            }
        }

        // 收集你我的本次回合应对（执行前：清理并填充 MyCurrentBingo / YouCurrentBingo）
        FightBingoHouse.RecordBeforeExecute(runnings, EnumWho.My);
        FightBingoHouse.RecordBeforeExecute(runnings, EnumWho.You);

        // 记录本回合开始时存活的精灵 Uuid（用于对比出本回合死亡的精灵）
        var aliveMyUuids = FightAliveHouse.GetAlivePetUuids(EnumWho.My);
        var aliveYouUuids = FightAliveHouse.GetAlivePetUuids(EnumWho.You);

        // 3. 从队列中逐个弹出执行（index 为原始顺序编号，用于日志）
        int index = 0;
        while (_queue.Count > 0)
        {
            FightRunning run = _queue.Dequeue();

            // ★ 阶段有效性校验：施法方/受击方已死亡时跳过该阶段
            //   修复：连击鞭尸（目标已死仍继续打）、死亡方技能仍结算（攻击源已死伤害仍生效）
            if (!IsRunningValid(run, index))
            {
                index++;
                continue;
            }

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

        // 执行完成后清空队列
        _queue.Clear();

        GD.Print($"[FightRunningExe] FightRunning 执行完毕，本回合死亡 {newDiePets.Count} 只精灵，==================");
        
        return newDiePets;
    }

    /// <summary>
    /// 阻断打断过滤：
    /// 扫描传入的 runnings 数组中任意 Running 是否应对了状态（BingoSkillType=STATUS），
    /// 若应对了状态，不管敌我速度，将对方的 DoStatusXX 阶段置空（打断），
    /// 返回过滤后的 runnings 数组。
    /// </summary>
    /// <param name="runnings">原始 FightRunning 数组</param>
    /// <returns>过滤后的 FightRunning 数组（被阻断的 DoStatusXX 置为 null）</returns>
    private static FightRunning[] BlockIntercept(FightRunning[] runnings)
    {
        if (runnings == null)
            return runnings;

        // 查找首个应对了状态（BingoSkillType=STATUS）的 Running
        EnumFightRunningType? blockedDoStatusType = null;
        EnumWho? blockSide = null;
        for (int i = 0; i < runnings.Length; i++)
        {
            FightRunning r = runnings[i];
            if (r != null && r.BingoSkillType == EnumSkillType.STATUS)
            {
                EnumWho otherSide = r.Side == EnumWho.My ? EnumWho.You : EnumWho.My;
                blockedDoStatusType = otherSide == EnumWho.My
                    ? EnumFightRunningType.DoStatusMy
                    : EnumFightRunningType.DoStatusYou;
                blockSide = r.Side;
                break;
            }
        }

        // 找到触发阻断的 Running，将对方 DoStatusXX 阶段置空
        if (blockedDoStatusType.HasValue)
        {
            int removedCount = 0;
            for (int i = 0; i < runnings.Length; i++)
            {
                if (runnings[i] != null && runnings[i].RunningType == blockedDoStatusType.Value)
                {
                    runnings[i] = null;
                    removedCount++;
                }
            }
            GD.Print($"      ⚡ {blockSide} 阻断应对状态 → 移除对方 {blockedDoStatusType.Value} × {removedCount}，对方状态技能被强行打断");
        }

        return runnings;
    }

    /// <summary>
    /// 获取 run.Side 方当前精灵的血量（用于实时监听 HP）
    /// </summary>
    private static int GetNowHp(FightRunning run)
    {
        if (FightLandMyStandPet.Instance.FightPetData != null && FightLandYouStandPet.Instance.FightPetData != null)
        {
            return run.Side == EnumWho.My ? FightLandMyStandPet.Instance.FightPetData.Hp : FightLandYouStandPet.Instance.FightPetData.Hp;
        }
        return 0;
    }

    /// <summary>
    /// 处理某方 HP 归零（公共逻辑，My/You 通用）：
    /// 1. 清空 CurrentRunArray 中关于本 side 的后续所有 running；
    /// 2. 同时清空执行队列中本 side 尚未执行的 running（另一边的 running 保留继续执行）；
    /// 3. 为本 side 添加对应的回合结束阶段（My → GenEndActsMy，You → GenEndActsYou）。
    /// </summary>
    private static void HandleSideDead(FightRunning run)
    {
        // 1. 清空 CurrentRunArray 中关于本 side 的后续所有 running
        for (int i = 0; i < FightRunningHouse.RunArrayLength; i++)
        {
            FightRunning r = FightRunningHouse.CurrentRunArray[i];
            if (r != null && r.Side == run.Side)
            {
                FightRunningHouse.CurrentRunArray[i] = null;
            }
        }
        // 同时清空执行队列中本 side 尚未执行的 running，
        // 另一边的 running 保留继续执行（队列已提取为类内字段）
        var remaining = _queue.ToArray();
        _queue.Clear();
        for (int i = 0; i < remaining.Length; i++)
        {
            if (remaining[i] != null && remaining[i].Side != run.Side)
            {
                _queue.Enqueue(remaining[i]);
            }
        }
        // 2. 然后为本 side 添加对应的回合结束阶段 running
        //    （My 方 → GenEndActsMy，You 方 → GenEndActsYou）
        EnumFightRunningType genEndType = run.Side == EnumWho.My
            ? EnumFightRunningType.GenEndActsMy
            : EnumFightRunningType.GenEndActsYou;
        FightRunningHouse.AddRunningEasy(genEndType, run.Side);
    }

    /// <summary>
    /// 阶段有效性校验：在执行阶段前判断该阶段是否仍应执行。
    ///
    /// 背景：原 CheckHpNice 只检查 run.Side（阶段所属方）自身的血量，
    /// 导致两类 BUG：
    ///   1. 连击鞭尸：我方连击把敌方打死后，后续 DoAttackMy（Side=My）阶段
    ///      因只检查我方血量而继续执行，白白扣 PP 且对已倒下目标造成无效伤害。
    ///   2. 已亡精灵的伤害仍结算：攻击方（敌方）已死亡，但其"垂死反击"等技能的
    ///      DoDamageMy（Side=My）阶段仍结算伤害，把已不在场精灵的伤害打出。
    ///
    /// 校验规则：
    ///   - DoAttack（施法阶段，Side=施法方）：施法方存活，且受击方（另一侧）存活
    ///   - DoDefense / DoStatus（施法阶段）：仅要求施法方存活（增益/防御作用于自身）
    ///   - DoDamage（扣血阶段，Side=受击方）：受击方存活，且攻击方（另一侧）存活
    ///   - 其余阶段沿用原有 CheckHpNice
    /// </summary>
    private static bool IsRunningValid(FightRunning run, int index)
    {
        if (run == null) return false;

        EnumFightRunningType type = run.RunningType;

        bool isDoAttack = type == EnumFightRunningType.DoAttackMy
                       || type == EnumFightRunningType.DoAttackYou;
        bool isDoSkill = isDoAttack
                      || type == EnumFightRunningType.DoDefenseMy
                      || type == EnumFightRunningType.DoDefenseYou
                      || type == EnumFightRunningType.DoStatusMy
                      || type == EnumFightRunningType.DoStatusYou;
        bool isDoDamage = type == EnumFightRunningType.DoDamageMy
                       || type == EnumFightRunningType.DoDamageYou;

        // 非特殊阶段：沿用原有逻辑（仅检查自身存活）
        if (!isDoSkill && !isDoDamage)
            return FightRunningBetween.CheckHpNice(run);

        EnumWho side = run.Side;
        EnumWho otherSide = side == EnumWho.My ? EnumWho.You : EnumWho.My;

        // 施法阶段：施法方必须存活（死亡宠物不能继续行动）
        if (isDoSkill)
        {
            if (!IsSideAlive(side))
            {
                GD.Print($"      ⏭ [{index}] {type} | 施法方 {side} 已死亡，跳过该阶段");
                return false;
            }

            // 攻击类施法额外要求受击方存活：目标倒下后剩余连击立即停止
            if (isDoAttack && !IsSideAlive(otherSide))
            {
                GD.Print($"      ⏭ [{index}] {type} | 受击方 {otherSide} 已死亡，跳过（目标已倒下，连击停止）");
                return false;
            }

            return true;
        }

        // DoDamage 扣血阶段：受击方必须存活（无需再扣）
        if (!IsSideAlive(side))
        {
            GD.Print($"      ⏭ [{index}] {type} | 受击方 {side} 已死亡，跳过该阶段");
            return false;
        }

        // 攻击方必须存活：攻击方已倒下则本次伤害不结算（垂死反击无效）
        if (!IsSideAlive(otherSide))
        {
            GD.Print($"      ⏭ [{index}] {type} | 攻击方 {otherSide} 已死亡，跳过该阶段（已亡精灵伤害不结算）");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 判断某阵营当前场上精灵是否存活
    /// </summary>
    private static bool IsSideAlive(EnumWho side)
    {
        InsFightPetData pet = side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;
        return pet != null && pet.Hp > 0;
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
        int nowHp = GetNowHp(run);
        // 
        switch (run.RunningType)
        {
            case EnumFightRunningType.DoDamageMy:
                nowHp = FightRunningExeTool.ExecuteDamage(run, index);
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

        if (nowHp <= 0)
        {
            HandleSideDead(run);
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
        int nowHp = GetNowHp(run);
        // 
        switch (run.RunningType)
        {
            case EnumFightRunningType.DoDamageYou:
                nowHp = FightRunningExeTool.ExecuteDamage(run, index);
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

        if (nowHp <= 0)
        {
            HandleSideDead(run);
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

        // 本方的 FightRunning 数组（传入技能实现类供 StartSkill/EndSkill 检测应对等阶段）
        FightRunning[] sideRunnings = FightRunningHouse.CurrentRunArray;

        // 判断是否为 Start 技能类型，是则执行技能实现类的 StartSkill 鸭子方法
        if (FightRunningTypeDesign.IsStartType(run.RunningType))
        {
            if (hasSkillImpl)
            {
                DuckSkillLoader.ExecuteStartSkill(sideSkill.ImplClass, index, run, sideSkill, sideRunnings);
            }
        }
        // 判断是否为 End 技能类型，是则执行技能实现类的 EndSkill 鸭子方法
        else if (FightRunningTypeDesign.IsEndType(run.RunningType))
        {
            if (hasSkillImpl)
            {
                DuckSkillLoader.ExecuteEndSkill(sideSkill.ImplClass, index, run, sideSkill, sideRunnings);
            }
        }
    }
}
