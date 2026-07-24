using Godot;

/// <summary>
/// 回合运行执行工具
/// 提供扣血、执行状态等可复用的执行方法
/// </summary>
public static class FightRunningExeTool
{
    /// <summary>
    /// 执行扣血阶段：根据 FightRunning.Damage 扣除对应方精灵的 HP
    /// </summary>
    public static void ExecuteDamage(FightRunning run, int index)
    {
        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";

        // 获取要扣血的精灵
        InsFightPetData targetPet = run.Side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        if (targetPet == null)
        {
            GD.Print($"      [{index}] {sideLabel} {run.RunningType} | 目标精灵为空，跳过扣血");
            return;
        }

        // 执行扣血
        int actualDamage = run.Damage;
        targetPet.Hp = Mathf.Max(targetPet.Hp - actualDamage, 0);

        GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                 $"damage={actualDamage} {targetPet.PetName} HP: {targetPet.Hp}/{targetPet.MaxHp} " +
                 $"bingoSkillType={run.BingoSkillType}");
    }

    /// <summary>
    /// 执行 DoAttack 阶段：处理 DoAttackMy / DoAttackYou 攻击技能执行
    /// </summary>
    public static void ExecuteDoAttack(FightRunning run, int index)
    {
        // 留空
    }

    /// <summary>
    /// 执行 DoDefense 阶段：处理 DoDefenseMy / DoDefenseYou 防御技能执行
    /// </summary>
    public static void ExecuteDoDefense(FightRunning run, int index)
    {
        // 留空
    }

    /// <summary>
    /// 执行 DoStatus 阶段：处理 DoStatusMy / DoStatusYou 状态技能执行
    /// 判断 BingoSkillType 是否是防御，是则根据 GainBuffBingo 生成 InsFightBuff 数组，否则根据 GainBuff 生成
    /// </summary>
    public static void ExecuteDoStatus(FightRunning run, int index)
    {
        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";

        InsFightSkill sideSkill = run.SideFightSkill;
        if (sideSkill?.Skill == null)
        {
            GD.Print($"      [{index}] {sideLabel} {run.RunningType} | 技能为空，跳过状态执行");
            return;
        }

        InsSkill skill = sideSkill.Skill;

        // 判断 BingoSkillType 是否是防御
        Godot.Collections.Array buffSource = run.BingoSkillType == EnumSkillType.DEFENSE
            ? skill.GainBuffBingo
            : skill.GainBuff;

        if (buffSource != null && buffSource.Count > 0)
        {
            var buffs = DevBuffTool.CreateFromArray(buffSource);
            if (buffs != null && buffs.Count > 0)
            {
                // 根据 Side 判断保存到哪个 BuffManager
                if (run.Side == EnumWho.My)
                {
                    FightMyStandBuffManager.Instance?.AddBuffs(buffs.ToArray());
                }

                EnumWho targetSide = run.Side == EnumWho.My ? EnumWho.You : EnumWho.My;
                GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                         $"bingoSkillType={run.BingoSkillType} 生成 InsFightBuff {buffs.Count} 个 → 目标:{targetSide}");
            }
        }

        GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                 $"skill={skill.SkillName} bingoSkillType={run.BingoSkillType} sideSkill={sideSkill.Skill.SkillName} completed={run.IsCompleted}");
    }

}
