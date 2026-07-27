using Godot;
using System;

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
        if (run == null) { GD.PrintErr("ExecuteDoAttack | run 为 null"); return; }

        // 执行前扣除 PP
        FightPpTool.DeductPp(run, index);

        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";

        InsFightSkill sideSkill = run.SideFightSkill;
        if (sideSkill == null) { GD.PrintErr($"      [{index}] {sideLabel} {run.RunningType} | run.SideFightSkill 为 null"); return; }
        if (sideSkill.Skill == null) { GD.PrintErr($"      [{index}] {sideLabel} {run.RunningType} | sideSkill.Skill 为 null"); return; }

        InsSkill skill = sideSkill.Skill;

        if (string.IsNullOrWhiteSpace(sideSkill.ImplClass))
        {
            GD.PrintErr($"      [{index}] {sideLabel} {run.RunningType} | 错误：技能 {skill.SkillName} 未配置 impl_class，缺少鸭子实现");
            return;
        }

        DuckSkillLoader.ExecuteDuckSkill(sideSkill.ImplClass, index, run, sideSkill);
    }

    /// <summary>
    /// 执行 DoDefense 阶段：处理 DoDefenseMy / DoDefenseYou 防御技能执行
    /// </summary>
    public static void ExecuteDoDefense(FightRunning run, int index)
    {
        if (run == null) { GD.PrintErr("ExecuteDoDefense | run 为 null"); return; }

        // 执行前扣除 PP
        FightPpTool.DeductPp(run, index);

        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";

        InsFightSkill sideSkill = run.SideFightSkill;
        if (sideSkill == null) { GD.PrintErr($"      [{index}] {sideLabel} {run.RunningType} | run.SideFightSkill 为 null"); return; }
        if (sideSkill.Skill == null) { GD.PrintErr($"      [{index}] {sideLabel} {run.RunningType} | sideSkill.Skill 为 null"); return; }

        InsSkill skill = sideSkill.Skill;

        if (string.IsNullOrWhiteSpace(sideSkill.ImplClass))
        {
            GD.PrintErr($"      [{index}] {sideLabel} {run.RunningType} | 错误：技能 {skill.SkillName} 未配置 impl_class，缺少鸭子实现");
            return;
        }

        DuckSkillLoader.ExecuteDuckSkill(sideSkill.ImplClass, index, run, sideSkill);
    }

    /// <summary>
    /// 执行 DoStatus 阶段：处理 DoStatusMy / DoStatusYou 状态技能执行
    /// </summary>
    public static void ExecuteDoStatus(FightRunning run, int index)
    {
        if (run == null) { GD.PrintErr("ExecuteDoStatus | run 为 null"); return; }

        // 执行前扣除 PP
        FightPpTool.DeductPp(run, index);

        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";

        InsFightSkill sideSkill = run.SideFightSkill;
        if (sideSkill == null) { GD.PrintErr($"      [{index}] {sideLabel} {run.RunningType} | run.SideFightSkill 为 null"); return; }
        if (sideSkill.Skill == null) { GD.PrintErr($"      [{index}] {sideLabel} {run.RunningType} | sideSkill.Skill 为 null"); return; }

        InsSkill skill = sideSkill.Skill;

        if (string.IsNullOrWhiteSpace(sideSkill.ImplClass))
        {
            GD.PrintErr($"      [{index}] {sideLabel} {run.RunningType} | 错误：技能 {skill.SkillName} 未配置 impl_class，缺少鸭子实现");
            return;
        }

        DuckSkillLoader.ExecuteDuckSkill(sideSkill.ImplClass, index, run, sideSkill);

        GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                 $"skill={skill.SkillName} bingoSkillType={run.BingoSkillType} sideSkill={sideSkill.Skill.SkillName} completed={run.IsCompleted}");
    }
}