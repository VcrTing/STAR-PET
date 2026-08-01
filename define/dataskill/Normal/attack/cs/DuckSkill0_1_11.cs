using Godot;

[GlobalClass]
public partial class DuckSkill0_1_11 : Resource
{
    /// <summary>
    /// 大师之力技能实现
    /// 久经战斗沉淀的强力一击，每成功抵挡敌方一次进攻，本技能威力永久提升40点。
    ///
    /// 实现机制（RealtimeSync 阶段实时刷新威力）：
    /// 1. RealtimeSync 阶段（回合开始时调用，早于伤害计算）：
    ///    - 读取 FightBingoHouse 上回合应对记录（GetLastBingo(side)）
    ///    - 若上回合应对成功（lastBingoSuccess 为真，成功抵挡敌方一次进攻）
    ///      => 在 ActualAttackValue 上永久累加 40（初始=基础威力90，每抵挡成功一次 +40）
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        // 留空待实现
    }

    /// <summary>
    /// 重构 TurnAction 数组并返回
    /// </summary>
    public TurnAction[] RebuildTurn(TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        // 本技能（side 侧）行动数组
        TurnAction[] sideActions = side == EnumWho.My ? myTurnActions : youTurnActions;

        return sideActions;
    }

    /// <summary>
    /// 技能开始执行（Skill 阶段开始）
    /// </summary>
    public void StartSkill(int index, FightRunning run, InsFightSkill sideSkill, FightRunning[] sideRunnings)
    {
        // 留空待实现
    }

    /// <summary>
    /// 技能结束执行（Skill 阶段结束）
    /// </summary>
    public void EndSkill(int index, FightRunning run, InsFightSkill sideSkill, FightRunning[] sideRunnings)
    {
        // 留空待实现
    }

    /// <summary>
    /// Real-time sync skill
    /// 实时刷新技能状态
    /// 大师之力：每成功抵挡敌方一次进攻（lastBingoSuccess 为真），技能威力永久提升40点。
    /// 通过 sideSkill 干预伤害（在 ActualAttackValue 上永久累加 40）。
    /// </summary>
    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        if (sideSkill?.Skill == null)
            return;

        // 判断上回合是否成功抵挡敌方进攻（应对成功）
        EnumFightRunningType[] lastBingo = FightBingoHouse.GetLastBingo(side);
        bool lastBingoSuccess = lastBingo != null && lastBingo.Length > 0;

        // 成功抵挡一次进攻 => 本技能威力永久提升 40 点（累加在 ActualAttackValue 上）
        if (lastBingoSuccess)
        {
            int beforePower = sideSkill.ActualAttackValue;
            sideSkill.ActualAttackValue += 40;

            GD.Print($"      [RealtimeSync] DuckSkill0_1_11 | 大师之力 | 抵挡成功 1 次 | 威力: {beforePower} → {sideSkill.ActualAttackValue}");
        }
    }
}