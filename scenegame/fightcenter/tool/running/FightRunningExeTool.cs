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
    /// 执行状态阶段：处理 StartStatusMy / StartStatusYou 状态技能效果
    /// 根据技能配置的 gain_buff / gain_buff_bingo 向对应方添加 Buff
    /// </summary>
    public static void ExecuteStatus(FightRunning run, int index)
    {
        return;
        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";

        InsFightSkill fightSkill = run.FightSkill;
        if (fightSkill?.Skill == null)
        {
            GD.Print($"      [{index}] {sideLabel} {run.RunningType} | 技能为空，跳过状态执行");
            return;
        }

        InsSkill skill = fightSkill.Skill;

        // 用 DevBuffTool 解析 gain_buff 为 InsFightBuff[]
        if (skill.GainBuff != null && skill.GainBuff.Count > 0)
        {
            var buffs = DevBuffTool.CreateFromArray(skill.GainBuff);
            if (buffs != null && buffs.Count > 0)
            {
                EnumWho targetSide = run.Side == EnumWho.My ? EnumWho.You : EnumWho.My;
                // TODO: 找到对应方的 BuffManager 后调用 AddBuffs
                GD.Print($"      [{index}] {sideLabel} {run.RunningType} | gain_buff {buffs.Count} 个 → 目标:{targetSide}");
            }
        }

        // 用 DevBuffTool 解析 gain_buff_bingo（应对成功后额外效果）
        if (run.BingoSkillType > 0 && skill.GainBuffBingo != null && skill.GainBuffBingo.Count > 0)
        {
            var bingoBuffs = DevBuffTool.CreateFromArray(skill.GainBuffBingo);
            if (bingoBuffs != null && bingoBuffs.Count > 0)
            {
                EnumWho targetSide = run.Side == EnumWho.My ? EnumWho.You : EnumWho.My;
                GD.Print($"      [{index}] {sideLabel} {run.RunningType} | gain_buff_bingo {bingoBuffs.Count} 个 → 目标:{targetSide}");
            }
        }

        GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                 $"skill={skill.SkillName} bingoSkillType={run.BingoSkillType} completed={run.IsCompleted}");
    }
}
