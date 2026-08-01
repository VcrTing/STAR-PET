using Godot;

[GlobalClass]
public partial class DuckSkill0_1_8 : Resource
{
    /// <summary>
    /// 当头棒喝技能实现
    /// 挥出重棍猛击对手头部，若本回合敌方切换精灵，该技能威力直接翻倍。
    ///
    /// 实现机制（以 FightBingoHouse 的 Current 应对数组为核心判断）：
    /// 1. RebuildTurn 阶段（回合行动排序时调用，早于伤害计算）：
    ///    - 扫描对方行动数组，若对方使用了切换宠物技能（0_4_1）
    ///      => 通过 FightBingoHouse.AddSwitchPetBingo(side) 把"应对切换宠物"登记到 Current 应对数组
    ///      => 将本技能数组位置移到对方切换技能 index 之后
    ///      => 本技能实际威力 x2（影响 FightDamageTool 的伤害计算）
    /// 2. DoSkill 阶段（回合执行时调用，晚于伤害计算）：
    ///    - 以 FightBingoHouse.HasCurrentSwitchPetBingo(side) 判断本次回合是否应对切换宠物成功
    ///      （即以 Current 应对数组为核心，不再依赖 sideSkill.IsBingo 标记）
    ///    - 是则证明本次伤害已按翻倍威力结算 → 还原技能威力（避免跨回合累积）
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_8.DoSkill | 技能：当头棒喝 | 实际威力={sideSkill?.ActualAttackValue} | Current应对切换={FightBingoHouse.HasCurrentSwitchPetBingo(run.Side)} | bingoSkillType={run.BingoSkillType}");

        if (sideSkill?.Skill == null)
            return;

        // 伤害计算（Damage）与技能执行（DoSkill）是分开的：
        // 以 FightBingoHouse Current 应对数组为核心判断本次是否应对切换宠物成功
        if (FightBingoHouse.HasCurrentSwitchPetBingo(run.Side))
        {
            int doubledPower = sideSkill.ActualAttackValue;
            sideSkill.ActualAttackValue = sideSkill.Skill.AttackValue; // 还原基础威力
            GD.Print($"      [{index}] DuckSkill0_1_8.DoSkill | 还原当头棒喝威力（本回合应对切换已结算）: {doubledPower} → {sideSkill.ActualAttackValue}");
        }
    }

    /// <summary>
    /// 重构 TurnAction 数组并返回
    /// 判断对方是否切换宠物（使用系统技能 0_4_1），
    /// 是则：1. 把"应对切换宠物"登记到 FightBingoHouse Current 应对数组；
    ///       2. 把本技能放到对方切换宠物技能所在 index 之后；
    ///       3. 本技能实际威力 x2（供后续伤害计算使用）。
    /// </summary>
    public TurnAction[] RebuildTurn(TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        // 本技能（side 侧）与对方行动数组
        TurnAction[] sideActions = side == EnumWho.My ? myTurnActions : youTurnActions;
        TurnAction[] enemyActions = side == EnumWho.My ? youTurnActions : myTurnActions;

        // 1. 找到本技能（当头棒喝 0_1_8）所在位置
        int selfIndex = -1;
        TurnAction selfAction = null;
        for (int i = 0; i < sideActions.Length; i++)
        {
            if (sideActions[i]?.FightSkill?.Skill?.SkillId == "0_1_8")
            {
                selfIndex = i;
                selfAction = sideActions[i];
                break;
            }
        }
        if (selfIndex < 0 || selfAction == null)
            return sideActions;

        // 2. 扫描对方行动数组，判断是否切换宠物（0_4_1）
        int enemySwitchIndex = -1;
        for (int i = 0; i < enemyActions.Length; i++)
        {
            if (DevSkillHelpTool.IsSwitchPetSkill(enemyActions[i]?.FightSkill))
            {
                enemySwitchIndex = i;
                break;
            }
        }

        // 3. 对方未切换宠物：直接返回原数组
        if (enemySwitchIndex < 0)
            return sideActions;

        // 4. 对方切换了宠物：
        //    4.1 到 FightBingoHouse 登记"本方应对切换宠物成功"（装入 Current 应对数组）
        FightBingoHouse.AddSwitchPetBingo(side);

        //    4.2 本技能实际威力 x2（Damage 计算读取 ActualAttackValue）
        selfAction.FightSkill.ActualAttackValue *= 2;

        //    4.2 把本技能放到对方切换宠物技能所在的数组 index 后面
        TurnAction[] rebuilt = new TurnAction[sideActions.Length];

        // 复制除本技能外的所有行动，保持原位置
        for (int i = 0; i < sideActions.Length; i++)
        {
            if (i != selfIndex && sideActions[i] != null)
                rebuilt[i] = sideActions[i];
        }

        // 将本技能插入到对方切换宠物技能 index 之后
        int insertAt = Mathf.Min(enemySwitchIndex + 1, sideActions.Length - 1);
        rebuilt[insertAt] = selfAction;

        GD.Print($"  └─ [DuckSkill0_1_8] RebuildTurn | 对方切换精灵！已登记应对切换宠物，当头棒喝移动至 index {insertAt}（原 {selfIndex}），威力翻倍 => {selfAction.FightSkill.ActualAttackValue}");

        return rebuilt;
    }

    /// <summary>
    /// 技能开始执行（Skill 阶段开始）
    /// </summary>
    public void StartSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        // 留空待实现
    }

    /// <summary>
    /// 技能结束执行（Skill 阶段结束）
    /// </summary>
    public void EndSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        // 留空待实现
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
