// ════════════════════════════════════════════════════════════════
//  回合运行数据类
//  表示一回合中真正需要执行的一个步骤，不是 TurnAction
// ════════════════════════════════════════════════════════════════

/// <summary>
/// 回合运行类型枚举
/// 每方都有多个阶段，按顺序在回合中执行
/// </summary>
public enum EnumFightRunningType
{
    // ════════════════════════ 我方阶段 ════════════════════════

    /// <summary>开始状态阶段：回合开始时处理我方身上的持续状态效果（中毒、灼烧等）</summary>
    StartStatusMy,
    /// <summary>结束状态阶段：回合结束时处理我方身上的持续状态效果</summary>
    EndStatusMy,
    /// <summary>应对状态阶段：检查敌方是否有状态技能，产生应对效果</summary>
    BingoStatusMy,
    /// <summary>时停阶段：时停效果判定，控制我方是否跳过本回合行动</summary>
    TimeStopMy,
    /// <summary>应对防御阶段：检查敌方是否有防御技能，产生应对效果</summary>
    BingoDefenseMy,
    /// <summary>开始攻击阶段：我方执行攻击技能，计算伤害</summary>
    StartAttackMy,
    /// <summary>开始防御阶段：我方执行防御技能，提升减伤</summary>
    StartDefenseMy,
    /// <summary>应对攻击阶段：检查敌方是否有攻击技能，产生应对效果</summary>
    BingoAttackMy,
    /// <summary>结束防御阶段：防御效果结算完毕，清除临时防御加成</summary>
    EndDefenseMy,
    /// <summary>结束攻击阶段：攻击效果结算完毕，清除临时攻击加成</summary>
    EndAttackMy,
    /// <summary>计算扣血阶段：根据伤害计算结果扣除我方血量</summary>
    CalcDamageMy,
    /// <summary>检查血量阶段：检查我方精灵血量是否归零，触发濒死逻辑</summary>
    CheckHpMy,
    /// <summary>生成回合结束数组阶段：生成我方回合结束时要执行的持续效果列表</summary>
    GenEndActsMy,

    // ════════════════════════ 敌方阶段 ════════════════════════

    /// <summary>开始状态阶段：回合开始时处理敌方身上的持续状态效果</summary>
    StartStatusYou,
    /// <summary>结束状态阶段：回合结束时处理敌方身上的持续状态效果</summary>
    EndStatusYou,
    /// <summary>应对状态阶段：检查我方是否有状态技能，产生应对效果</summary>
    BingoStatusYou,
    /// <summary>时停阶段：时停效果判定，控制敌方是否跳过本回合行动</summary>
    TimeStopYou,
    /// <summary>应对防御阶段：检查我方是否有防御技能，产生应对效果</summary>
    BingoDefenseYou,
    /// <summary>开始攻击阶段：敌方执行攻击技能，计算伤害</summary>
    StartAttackYou,
    /// <summary>开始防御阶段：敌方执行防御技能，提升减伤</summary>
    StartDefenseYou,
    /// <summary>应对攻击阶段：检查我方是否有攻击技能，产生应对效果</summary>
    BingoAttackYou,
    /// <summary>结束防御阶段：防御效果结算完毕</summary>
    EndDefenseYou,
    /// <summary>结束攻击阶段：攻击效果结算完毕</summary>
    EndAttackYou,
    /// <summary>计算扣血阶段：根据伤害计算结果扣除敌方血量</summary>
    CalcDamageYou,
    /// <summary>检查血量阶段：检查敌方精灵血量是否归零，触发濒死逻辑</summary>
    CheckHpYou,
    /// <summary>生成回合结束数组阶段：生成敌方回合结束时要执行的持续效果列表</summary>
    GenEndActsYou,
}

/// <summary>
/// 回合运行数据
/// 每回合真正要执行的东西，不是 TurnAction
/// FightRunningTool 中使用 12 长度的数组存放
/// </summary>
public class FightRunning
{
    /// <summary>运行阶段类型</summary>
    public EnumFightRunningType RunningType;

    /// <summary>所属方</summary>
    public EnumWho Side;

    /// <summary>关联的战斗技能实例（如果是技能相关阶段）</summary>
    public InsFightSkill FightSkill;

    /// <summary>伤害值（计算扣血阶段使用）</summary>
    public int Damage;

    /// <summary>是否已完成</summary>
    public bool IsCompleted;

    /// <summary>是否应对阶段</summary>
    public bool IsBingo;

    /// <summary>应对的技能类型（对应 BingoSkillType）</summary>
    public int BingoSkillType;

    public FightRunning() { }

    public FightRunning(EnumFightRunningType type, EnumWho side, InsFightSkill fightSkill = null)
    {
        RunningType = type;
        Side = side;
        FightSkill = fightSkill;
        IsCompleted = false;
    }

    /// <summary>是否为 My 方</summary>
    public bool IsMy => Side == EnumWho.My;
}
