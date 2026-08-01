using Godot;

[GlobalClass]
public partial class DuckSkill0_4_1 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_4_1.DoSkill | 技能：选择切换宠物 | bingoSkillType={run.BingoSkillType}");
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
        // 通过 sideSkill 修改技能源头（留空待实现）
    }
}
