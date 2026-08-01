using Godot;

[GlobalClass]
public partial class DuckSkill0_1_12 : Resource
{
    /// <summary>
    /// 吞噬技能实现
    /// 张开巨口吞噬敌方精灵，若本次攻击成功击杀目标，自身额外获得6点能耗。
    ///
    /// 实现机制：
    /// 1. DoSkill 阶段（回合执行时调用）：
    ///    - 检查目标精灵是否被本技能击杀
    ///    - 若击杀成功 => 通过 FightPpTool.GainPp 为自身额外获得 6 点能耗
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_12.DoSkill | 技能：吞噬 | bingoSkillType={run.BingoSkillType}");

        if (sideSkill?.Skill == null)
            return;

        // 击杀判定：CheckHp 阶段发现目标精灵 HP<=0 即视为击杀
        // 注意：实际扣血在 DoDamage 阶段，DoSkill 早于扣血执行前调用，
        // 因此通过 FightAliveHouse 判断目标是否在本回合死亡
        EnumWho targetSide = run.Side == EnumWho.My ? EnumWho.You : EnumWho.My;

        // 目标精灵当前血量
        int targetHp = FightPetHpTool.GetCurrentHp(targetSide);
        if (targetHp <= 0)
        {
            // 目标已被击杀（HP 归零）
            int gainAmount = sideSkill.Skill.GainEnergy; // 6
            if (gainAmount > 0)
            {
                FightPpTool.GainPp(run.Side, gainAmount, false, index);
                GD.Print($"      [{index}] DuckSkill0_1_12.DoSkill | 吞噬成功击杀目标！自身额外获得 {gainAmount} 点能耗");
            }
        }
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
    /// </summary>
    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        // 通过 sideSkill 修改技能源头（留空待实现）
    }
}