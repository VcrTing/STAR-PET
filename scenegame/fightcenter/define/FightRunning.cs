// ════════════════════════════════════════════════════════════════
//  回合运行数据类
//  表示一回合中真正需要执行的一个步骤，不是 TurnAction
// ════════════════════════════════════════════════════════════════

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
