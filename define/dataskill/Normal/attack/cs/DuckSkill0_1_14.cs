using Godot;

[GlobalClass]
public partial class DuckSkill0_1_14 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_14.DoSkill | 技能：阻断 | Side={run.Side}");

        // 打断效果由 bingo_skill_type=3（应对状态）触发：
        // 1. 对方本回合释放状态技能时，本技能 DoAttack 的 BingoSkillType 会被标记为 STATUS
        // 2. FightRunningExe.ExecuteAll 检测到 STATUS 应对后，从执行队列中移除对方的 DoStatusXX 阶段
        // 3. 对方状态技能不会施放，达到"强行打断"效果
        // 打断的具体移除逻辑位于 scenegame/fightcenter/running/FightRunningExe.cs
    }

    /// <summary>
    /// 重构 TurnAction 数组并返回
    /// 传入双方行动数组，返回 side 对应的行动数组
    /// ⚠ 暂为占位实现：不做额外处理，直接返回 side 的行动数组
    /// </summary>
    public TurnAction[] RebuildTurn(TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        // 打断已由 FightRunningExe 队列移除机制实现，此处无需重构行动
        return side == EnumWho.My ? myTurnActions : youTurnActions;
    }

    /// <summary>
    /// Real-time sync skill
    /// 实时刷新技能状态
    /// 通过 sideSkill 修改技能源头
    /// </summary>
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

    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        // 通过 sideSkill 修改技能源头（留空待实现）
    }
}