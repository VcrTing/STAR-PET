using Godot;

[GlobalClass]
public partial class DuckSkill0_1_7 : Resource
{
    /// <summary>
    /// 乘胜追击技能实现
    /// 本技能是连击技能，趁势持续进攻，每一次释放本技能，自身连击层数叠加1层。
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        // 连击序号来自 run.TargetFightSkill：
        //   - 连击拆分时每击的克隆挂在 TargetFightSkill 上（ComboIndex = 0,1,2,...）
        //   - 非连击/未拆分时 TargetFightSkill = 源实例（ComboIndex = 0）
        int comboIndex = run?.TargetFightSkill?.ComboIndex ?? 0;
        GD.Print($"      [{index}] DuckSkill0_1_7.DoSkill | 技能：乘胜追击 | ComboIndex={comboIndex} | bingoSkillType={run.BingoSkillType}");

        // 只在本技能的第一击（ComboIndex==0）叠加连击层数，后续击跳过，
        // 避免同一回合内每击都执行 DoSkill 导致连击数指数级增长
        if (sideSkill?.Skill == null || comboIndex != 0)
            return;

        // 每次释放本技能（一个回合内的一次技能释放 = 第一击），自身连击层数叠加1层
        // 连击层数 = 实际连击数（战斗中可被特性/道具改变），上限99
        int beforeHitCount = sideSkill.ActualHitCount;
        sideSkill.ActualHitCount = Mathf.Min(sideSkill.ActualHitCount + 1, 99);

        GD.Print($"      [{index}] DuckSkill0_1_7.DoSkill | 连击层数: {beforeHitCount} → {sideSkill.ActualHitCount}");
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
