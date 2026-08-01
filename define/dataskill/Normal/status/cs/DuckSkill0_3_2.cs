using Godot;

[GlobalClass]
public partial class DuckSkill0_3_2 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        // GD.Print($"      [{index}] DuckSkill0_3_2.DoSkill | 技能：加固 | bingoSkillType={run.BingoSkillType}");

        // 加固效果：根据 BingoSkillType 判断使用 GainBuff 还是 GainBuffBingo
        Godot.Collections.Array buffSource = run.BingoSkillType == EnumSkillType.DEFENSE
            ? sideSkill.Skill.GainBuffBingo
            : sideSkill.Skill.GainBuff;
        FightSkillStatusRunTool.ExecuteStatusBuff(run, index, buffSource);
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
    /// Real-time sync skill
    /// 实时刷新技能状态
    /// 通过 sideSkill 修改技能源头
    /// </summary>
    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        // 通过 sideSkill 修改技能源头（留空待实现）
    }
}
