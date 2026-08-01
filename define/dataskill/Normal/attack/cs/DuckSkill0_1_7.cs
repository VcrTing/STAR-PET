using Godot;

[GlobalClass]
public partial class DuckSkill0_1_7 : Resource
{
    /// <summary>
    /// 乘胜追击技能实现
    /// 本技能是连击技能，趁势持续进攻，每一次释放本技能，自身连击层数叠加1层。
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_7.DoSkill | 技能：乘胜追击 | UseCount={sideSkill?.UseCount} | bingoSkillType={run.BingoSkillType}");

        if (sideSkill?.Skill == null)
            return;

    }

    /// <summary>
    /// 重构 TurnAction 数组并返回
    /// 传入双方行动数组，返回 side 对应的行动数组
    /// ⚠ 暂为占位实现：不做额外处理，直接返回 side 的行动数组
    /// </summary>
    public TurnAction[] RebuildTurn(TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        return side == EnumWho.My ? myTurnActions : youTurnActions;
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
    /// 通过 sideSkill 修改技能源头
    /// </summary>
    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        if (sideSkill?.Skill == null)
            return;

        // 根据使用次数同步连击数：
        // 每一次释放本技能，连击层数叠加1层，同步的连击数= UseCount + 初始1
        sideSkill.ActualHitCount = Mathf.Min(1 + sideSkill.UseCount, 99);

        GD.Print($"      [RealtimeSync] DuckSkill0_1_7 | 技能：乘胜追击 | UseCount={sideSkill.UseCount} | 连击层数: {sideSkill.ActualHitCount}");
    }
}
