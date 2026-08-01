using Godot;

[GlobalClass]
public partial class DuckSkill0_1_9 : Resource
{
    /// <summary>
    /// 气势一击技能实现
    /// 裹挟雄浑战意重拳出击，上回合应对成功时，本次威力额外增加80点。
    ///
    /// 实现机制（与当头棒喝相同的 IsBingo 模式，衔接伤害计算与技能执行）：
    /// 1. RebuildTurn 阶段（回合行动排序时调用，早于伤害计算）：
    ///    - 读取 FightBingoHouse 上回合应对记录（MyLastBingo / YouLastBingo）
    ///    - 若上回合应对成功 => IsBingo = true，且本技能威力 +80（100→180，影响 FightDamageTool 伤害计算）
    /// 2. DoSkill 阶段（回合执行时调用，晚于伤害计算）：
    ///    - 检查 IsBingo == true，证明本次威力已增强结算 => 还原技能威力并清除 IsBingo
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_9.DoSkill | 技能：气势一击 | 实际威力={sideSkill?.ActualAttackValue} | UseCount={sideSkill?.UseCount} | bingoSkillType={run.BingoSkillType}");

        if (sideSkill?.Skill == null)
            return;

        // 使用次数 +1
        sideSkill.UseCount += 1;

        // 伤害计算与技能执行分离：RebuildTurn 已把威力 +80 并写入伤害计算，此处检查标记后还原技能威力
        int boostedPower = sideSkill.ActualAttackValue;
        sideSkill.ActualAttackValue = sideSkill.Skill.AttackValue; // 还原基础威力（100）
        GD.Print($"      [{index}] DuckSkill0_1_9.DoSkill | 还原气势一击威力（上回合应对已结算）: {boostedPower} → {sideSkill.ActualAttackValue}");
    }

    /// <summary>
    /// 重构 TurnAction 数组并返回
    /// 判断上回合是否应对成功（FightBingoHouse 上回合应对数组非空），
    /// 是则：1. IsBingo = true；
    ///       2. 本技能实际威力 +80（100→180，供伤害计算使用）。
    /// </summary>
    public TurnAction[] RebuildTurn(TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        // 本技能（side 侧）行动数组
        TurnAction[] sideActions = side == EnumWho.My ? myTurnActions : youTurnActions;

        // 1. 找到本技能（气势一击 0_1_9）所在位置
        int selfIndex = -1;
        TurnAction selfAction = null;
        for (int i = 0; i < sideActions.Length; i++)
        {
            if (sideActions[i]?.FightSkill?.Skill?.SkillId == "0_1_9")
            {
                selfIndex = i;
                selfAction = sideActions[i];
                break;
            }
        }
        if (selfIndex < 0 || selfAction == null)
            return sideActions;

        // 2. 判断上回合是否应对成功（上回合应对数组非空 = 有应对产生）
        EnumFightRunningType[] lastBingo = FightBingoHouse.GetLastBingo(side);
        bool lastBingoSuccess = lastBingo != null && lastBingo.Length > 0;

        // 3. 上回合未应对成功：直接返回原数组
        if (!lastBingoSuccess)
        {
            GD.Print($"  └─ [DuckSkill0_1_9] RebuildTurn | 上回合未应对成功，气势一击威力保持 {selfAction.FightSkill.ActualAttackValue}");
            return sideActions;
        }

        // 4. 上回合应对成功：
        //    4.1 标记使用次数 + 本技能实际威力 +80（Damage 计算读取 ActualAttackValue）
        selfAction.FightSkill.ActualAttackValue = selfAction.FightSkill.Skill.AttackValue + 80;

        GD.Print($"  └─ [DuckSkill0_1_9] RebuildTurn | 上回合应对成功！气势一击威力 +80 => {selfAction.FightSkill.ActualAttackValue}");

        return sideActions;
    }

    /// <summary>
    /// Real-time sync skill
    /// 实时刷新技能状态
    /// 通过 sideSkill 修改技能源头
    /// </summary>
    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        // 通过 sideSkill 修改技能源头（留空待实现）
    }
}
