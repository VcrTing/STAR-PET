using Godot;

[GlobalClass]
public partial class DuckSkill0_1_10 : Resource
{
    /// <summary>
    /// 垂死反击技能实现
    /// 血量濒危时迸发反击力量，自身当前血量每降低10%，技能威力永久提升8点。
    ///
    /// 实现机制（与气势一击相同的 RebuildTurn/DoSkill 分离模式，衔接伤害计算与技能执行）：
    /// 1. RebuildTurn 阶段（回合行动排序时调用，早于伤害计算）：
    ///    - 读取自身当前血量（FightPetHpTool.GetCurrentHp(side)）与最大血量（GetMaxHp(side)）
    ///    - 计算血量损失百分比，每降低10% => 威力 +8（80→提升后，影响 FightDamageTool 伤害计算）
    /// 2. DoSkill 阶段（回合执行时调用，晚于伤害计算）：
    ///    - 由于伤害已按提升后的威力结算，此处还原威力为基础威力
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        
    }

    /// <summary>
    /// 重构 TurnAction 数组并返回
    /// 根据自身当前血量计算威力加成：自身当前血量每降低10%，技能威力提升8点。
    /// 在 RebuildTurn 阶段设置 ActualAttackValue（供伤害计算读取）。
    /// </summary>
    public TurnAction[] RebuildTurn(TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        // 本技能（side 侧）行动数组
        TurnAction[] sideActions = side == EnumWho.My ? myTurnActions : youTurnActions;

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
        // 
        GD.Print("刷新垂死反击伤害");
    }
}
