using Godot;

[GlobalClass]
public partial class DuckSkill0_3_1 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_3_1.DoSkill | 技能：聚能 | bingoSkillType={run.BingoSkillType}");

        // 聚能效果：委托 FightSkillStatusRunTool 补充 5 点能量（Pp）
        FightSkillStatusRunTool.ExecuteFocusEnergy(index, run);
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
}
