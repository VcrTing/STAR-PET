using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 技能判断工具（内部辅助方法）
/// 从 FightSkillJudgeTool 提取的辅助性静态方法
/// </summary>
public static class FightSkillJudge2Tool
{
    /// <summary>
    /// 从 TurnAction 数组中查找所有有效的 InsFightSkill，返回数组
    /// </summary>
    public static InsFightSkill[] GetFightSkillFromActions(TurnAction[] actions)
    {
        if (actions == null) return Array.Empty<InsFightSkill>();
        List<InsFightSkill> res = new List<InsFightSkill>();
        for (int i = 0; i < actions.Length; i++)
        {
            if (actions[i]?.FightSkill != null)
                res.Add(actions[i].FightSkill);
        }
        return res.ToArray();
    }

    /// <summary>
    /// Side 攻击，对方也攻击 — 连击拆分 + 伤害计算
    /// 根据 side 自动获取场上精灵数据
    /// </summary>
    public static void SideAttackWhenOtherOneAttack(InsFightSkill sideSkill, EnumWho side)
    {
        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(sideSkill);
        EnumWho targetSide = side == EnumWho.My ? EnumWho.You : EnumWho.My;

        for (int h = 0; h < hitSkills.Length; h++)
        {
            InsFightSkill hit = hitSkills[h];
            FightRunningHouse.AddRunning2(
                side == EnumWho.My ? EnumFightRunningType.DoAttackMy : EnumFightRunningType.DoAttackYou,
                side, sideSkill, 0, hit, EnumSkillType.None);
            int basicDamag = FightDamageTool.CalcSkillFinalDamage(hit, side);
            FightRunningHouse.AddRunning2(
                side == EnumWho.My ? EnumFightRunningType.DoDamageYou : EnumFightRunningType.DoDamageMy,
                targetSide, sideSkill, basicDamag, hit);
        }
    }

    /// <summary>
    /// Side 防御，对方攻击 — 减伤计算
    /// 根据 side 自动获取场上精灵数据
    /// </summary>
    public static void SideDefenseWhenOtherOneAttack(InsFightSkill sideSkill, EnumWho side, InsFightSkill otherOneSkill)
    {
        // side 是防御方，攻击方是对方
        EnumWho attackerSide = side == EnumWho.My ? EnumWho.You : EnumWho.My;

        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(otherOneSkill);

        for (int h = 0; h < hitSkills.Length; h++)
        {
            int basicDamag = FightDamageTool.CalcSkillFinalDamage(hitSkills[h], attackerSide);
            int finalDamage = DevSkillCompuTool.DamageBeDefense(basicDamag, sideSkill.Skill.DamageReductionRate);
            GD.Print($"      防御第{h + 1}击: 原始伤害={basicDamag}, 最终伤害={finalDamage}");
            FightRunningHouse.AddRunning2(
                side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                side, sideSkill, finalDamage, hitSkills[h]);
        }
    }

    /// <summary>
    /// Side 状态，对方攻击 — 连击拆分 + 伤害计算
    /// 根据 side 自动获取场上精灵数据
    /// </summary>
    public static void SideStatusWhenOtherOneAttack(InsFightSkill sideSkill, EnumWho side, InsFightSkill otherOneSkill)
    {
        // side 是状态方（被攻击方），攻击方是对方
        EnumWho attackerSide = side == EnumWho.My ? EnumWho.You : EnumWho.My;

        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(otherOneSkill);

        for (int h = 0; h < hitSkills.Length; h++)
        {
            InsFightSkill otherHit = hitSkills[h];
            int basicDamag = FightDamageTool.CalcSkillFinalDamage(otherHit, attackerSide);
            GD.Print($"      被攻击第{h + 1}击: 原始伤害={basicDamag}, 最终伤害={basicDamag}");
            FightRunningHouse.AddRunning2(
                side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                side, sideSkill, basicDamag, otherHit);
        }
    }

    /// <summary>
    /// Side 系统，对方攻击 — 连击拆分 + 伤害计算
    /// 根据 side 自动获取场上精灵数据
    /// </summary>
    public static void SideSystemWhenOtherOneAttack(InsFightSkill sideSkill, EnumWho side, InsFightSkill otherOneSkill)
    {
        // side 是系统方（被攻击方），攻击方是对方
        EnumWho attackerSide = side == EnumWho.My ? EnumWho.You : EnumWho.My;

        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(otherOneSkill);

        for (int h = 0; h < hitSkills.Length; h++)
        {
            InsFightSkill otherHit = hitSkills[h];
            int basicDamag = FightDamageTool.CalcSkillFinalDamage(otherHit, attackerSide);
            GD.Print($"      被攻击第{h + 1}击: 原始伤害={basicDamag}, 最终伤害={basicDamag}");
            FightRunningHouse.AddRunning2(
                side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                side, sideSkill, basicDamag, otherHit);
        }
    }
}