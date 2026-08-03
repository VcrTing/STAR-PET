using Godot;

[GlobalClass]
public partial class DuckSkill0_2_2 : Resource
{
    /// <summary>
    /// 特意防守技能实现
    /// 本回合应对攻击减伤50%；成功抵挡攻击后，自身下一次出手先手值+1。
    ///
    /// 实现机制：
    /// 1. 减伤由技能静态数据 damage_reduction_rate=50（50%减伤）+ bingo_skill_type=1（应对攻击）处理；
    /// 2. EndSkill 阶段（回合结束执行）：
    ///    - 扫描 sideRunnings 中本方的应对攻击阶段（BingoAttackMy / BingoAttackYou），
    ///      判断本回合是否成功抵挡攻击
    ///    - 成功抵挡 => IsEffectActiveThisTurn = true（为下回合做准备）
    ///    - 未成功抵挡 => IsEffectActiveThisTurn = false
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_2_2.DoSkill | 技能：特意防守 | bingoSkillType={run.BingoSkillType}");
    }

    /// <summary>
    /// 重构 TurnAction 数组并返回
    /// </summary>
    public TurnAction[] RebuildTurn(TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        return side == EnumWho.My ? myTurnActions : youTurnActions;
    }

    /// <summary>
    /// 技能开始执行（Skill 阶段开始）
    /// </summary>
    public void StartSkill(int index, FightRunning run, InsFightSkill sideSkill, FightRunning[] sideRunnings)
    {
        // 留空待实现
    }

    /// <summary>
    /// 技能结束执行（Skill 阶段结束）
    /// 扫描 sideRunnings 中本方的应对攻击阶段（BingoAttackMy / BingoAttackYou），
    /// 判断本回合是否成功抵挡攻击：
    /// 成功 => IsEffectActiveThisTurn = true，并给本侧宠物设置下回合先手值 +1；
    /// 失败 => IsEffectActiveThisTurn = false
    /// 注意：先手值 +1 只加一回合——排序消费完会在 FightExeAction.ExecuteActions 步骤1.6 统一重置
    /// </summary>
    public void EndSkill(int index, FightRunning run, InsFightSkill sideSkill, FightRunning[] sideRunnings)
    {
        if (sideSkill?.Skill == null)
            return;

        EnumWho side = run.Side;
        string sideLabel = side == EnumWho.My ? "🧑我方" : "👹敌方";

        EnumFightRunningType bingoAttackType = side == EnumWho.My
            ? EnumFightRunningType.BingoAttackMy
            : EnumFightRunningType.BingoAttackYou;

        // 扫描 sideRunnings 中是否包含本方的应对攻击阶段（本回合成功抵挡攻击）
        bool bingoSuccess = false;
        if (sideRunnings != null)
        {
            for (int i = 0; i < sideRunnings.Length; i++)
            {
                FightRunning r = sideRunnings[i];
                if (r != null && r.RunningType == bingoAttackType)
                {
                    bingoSuccess = true;
                    break;
                }
            }
        }

        // 成功抵挡 => 本回合特效生效（下回合先手值+1）；否则不生效
        sideSkill.IsEffectActiveThisTurn = bingoSuccess;

        if (bingoSuccess)
        {
            // 应对攻击成功 => 给本侧宠物加下回合先手值 + 1（只加一回合，排序消费后由 FightExeAction 重置）
            if (side == EnumWho.My)
                FightLandMyStandPet.Instance?.SetRoundPriorityIntervene(1);
            else
                FightLandYouStandPet.Instance?.SetRoundPriorityIntervene(1);
        }

        int nowIntervene = side == EnumWho.My
            ? (FightLandMyStandPet.Instance?.GetRoundPriorityIntervene() ?? 0)
            : (FightLandYouStandPet.Instance?.GetRoundPriorityIntervene() ?? 0);

        GD.Print($"      [{index}] DuckSkill0_2_2.EndSkill | {sideLabel} 特意防守 | 本回合抵挡攻击={bingoSuccess} | 下回合先手值+1={(bingoSuccess ? 1 : 0)} | 当前干预先手值={nowIntervene} | IsEffectActiveThisTurn={sideSkill.IsEffectActiveThisTurn}");
    }

    /// <summary>
    /// Real-time sync skill
    /// 实时刷新技能状态
    /// </summary>
    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        // 通过 sideSkill 修改技能源头（留空待实现）
    }
}