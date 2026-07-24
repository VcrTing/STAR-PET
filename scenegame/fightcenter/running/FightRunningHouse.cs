// ════════════════════════════════════════════════════════════════
//  回合运行仓库
//  专门存储和管理 CurrentRunArray
// ════════════════════════════════════════════════════════════════

/// <summary>
/// 回合运行仓库
/// 负责存储 CurrentRunArray 并提供 AddRunning / ClearRunArray 操作
/// </summary>
public static class FightRunningHouse
{
    /// <summary>回合运行数组长度</summary>
    public const int RunArrayLength = 9999;

    /// <summary>
    /// 当前回合的运行数组，存储 FightRunning
    /// 表示本回合真正要执行的所有步骤
    /// </summary>
    public static FightRunning[] CurrentRunArray = new FightRunning[RunArrayLength];

    /// <summary>
    /// 重置当前运行数组
    /// </summary>
    public static void ClearRunArray()
    {
        CurrentRunArray = new FightRunning[RunArrayLength];
    }

    /// <summary>
    /// 向 CurrentRunArray 添加一个 FightRunning 到第一个空位
    /// </summary>
    /// <param name="type">运行阶段类型</param>
    /// <param name="side">所属方</param>
    /// <param name="fightSkill">关联的技能（可选）</param>
    /// <param name="damage">伤害值（可选）</param>
    /// <param name="bingoSkillType">应对的技能类型（None=无应对，可选）</param>
    public static void AddRunning(EnumFightRunningType type, EnumWho side, InsFightSkill fightSkill = null, int damage = 0, EnumSkillType bingoSkillType = EnumSkillType.None)
    {
        for (int i = 0; i < RunArrayLength; i++)
        {
            if (CurrentRunArray[i] == null)
            {
                CurrentRunArray[i] = new FightRunning(type, side, fightSkill)
                {
                    Damage = damage,
                    BingoSkillType = bingoSkillType
                };
                return;
            }
        }
    }
}