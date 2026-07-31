using Godot;

[GlobalClass]
public partial class DuckSkill0_1_8 : Resource
{
    /// <summary>
    /// 当头棒喝技能实现
    /// 挥出重棍猛击对手头部，若本回合敌方切换精灵，该技能威力直接翻倍。
    /// 
    /// ★ 注意：当前"本回合敌方是否切换精灵"的判断先默认 false（占位），
    ///   尚未接入真实判定逻辑。后续需要在回合行动构建阶段记录敌方切换精灵状态，
    ///   并在此处根据该状态将威力翻倍。
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_8.DoSkill | 技能：当头棒喝 | bingoSkillType={run.BingoSkillType}");

        // ─── 本回合敌方是否切换精灵：默认 false（占位） ───
        // TODO: 接入真实判定。若敌方在本回合行动中选择了切换精灵（TurnActionType.SwitchPet），
        //       则将本技能威力翻倍（例如 sideSkill.ActualAttackValue *= 2）。
        bool enemySwitchedThisTurn = false;

        if (!enemySwitchedThisTurn)
        {
            GD.Print($"      [{index}] DuckSkill0_1_8.DoSkill | 本回合敌方未切换精灵（判断暂为占位 false）");
            return;
        }

        // 敌方切换精灵时，本技能威力直接翻倍
        GD.Print($"      [{index}] DuckSkill0_1_8.DoSkill | 本回合敌方切换精灵！当头棒喝威力翻倍！");
        sideSkill.ActualAttackValue *= 2;
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
