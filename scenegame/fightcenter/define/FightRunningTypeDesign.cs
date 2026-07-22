// ════════════════════════════════════════════════════════════════
//  回合运行类型设计
//  定义 EnumFightRunningType 并提供阶段映射方法
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
    /// <summary>扣血执行阶段：根据伤害计算结果扣除我方血量</summary>
    DoDamageMy,
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
    /// <summary>扣血执行阶段：根据伤害计算结果扣除敌方血量</summary>
    DoDamageYou,
    /// <summary>检查血量阶段：检查敌方精灵血量是否归零，触发濒死逻辑</summary>
    CheckHpYou,
    /// <summary>生成回合结束数组阶段：生成敌方回合结束时要执行的持续效果列表</summary>
    GenEndActsYou,
}

/// <summary>
/// 回合运行类型设计工具
/// 提供 EnumFightRunningType 的辅助方法
/// </summary>
public static class FightRunningTypeDesign
{
    /// <summary>
    /// 判断某个 EnumFightRunningType 是否为 StartXX 类型
    /// </summary>
    public static bool IsStartType(EnumFightRunningType type)
    {
        return type == EnumFightRunningType.StartStatusMy
            || type == EnumFightRunningType.StartAttackMy
            || type == EnumFightRunningType.StartDefenseMy
            || type == EnumFightRunningType.StartStatusYou
            || type == EnumFightRunningType.StartAttackYou
            || type == EnumFightRunningType.StartDefenseYou;
    }

    /// <summary>
    /// 根据 StartXXX 类型返回对应的 EndXXX 类型
    /// 例如：StartStatusMy → EndStatusMy，StartAttackYou → EndAttackYou
    /// </summary>
    /// <param name="type">开始阶段类型（StartXXX）</param>
    /// <returns>对应的结束阶段类型（EndXXX），若没有对应关系则返回原值</returns>
    public static EnumFightRunningType GetEndType(EnumFightRunningType type)
    {
        switch (type)
        {
            // ─── 我方 ───
            case EnumFightRunningType.StartStatusMy:   return EnumFightRunningType.EndStatusMy;
            case EnumFightRunningType.StartAttackMy:   return EnumFightRunningType.EndAttackMy;
            case EnumFightRunningType.StartDefenseMy:  return EnumFightRunningType.EndDefenseMy;

            // ─── 敌方 ───
            case EnumFightRunningType.StartStatusYou:  return EnumFightRunningType.EndStatusYou;
            case EnumFightRunningType.StartAttackYou:  return EnumFightRunningType.EndAttackYou;
            case EnumFightRunningType.StartDefenseYou: return EnumFightRunningType.EndDefenseYou;

            default:
                // 如果本身就是 EndXXX 或其他无对应 End 的类型，返回自身
                return type;
        }
    }
}