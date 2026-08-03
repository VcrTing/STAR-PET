using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 技能判断工具（内部辅助方法）
/// 从 FightSkillJudgeTool 提取的辅助性静态方法
/// </summary>
public static class FightSkillJudge2Tool
{
    private static readonly Random _random = new();

    /// <summary>
    /// 检查技能是否命中（基于 HitRate）
    /// </summary>
    /// <param name="skill">战斗技能实例</param>
    /// <returns>true=命中, false=未命中</returns>
    private static bool IsHit(InsFightSkill skill)
    {
        if (skill?.Skill == null)
            return true;

        float hitRate = skill.Skill.HitRate;
        // 命中率 >= 100 必定命中
        if (hitRate >= 100.0f)
            return true;

        float roll = (float)_random.NextDouble() * 100.0f;
        bool hit = roll < hitRate;
        if (!hit)
            GD.Print($"      [命中判定] 技能={skill.Skill.SkillName} 命中率={hitRate}% 掷骰={roll:F1}% → 未命中");
        return hit;
    }

    /// <summary>
    /// 检查技能是否命中，若未命中则将伤害置为 0
    /// </summary>
    private static int ApplyHitCheck(InsFightSkill skill, int damage)
    {
        return IsHit(skill) ? damage : 0;
    }
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
        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(sideSkill, side);
        EnumWho targetSide = side == EnumWho.My ? EnumWho.You : EnumWho.My;

        for (int h = 0; h < hitSkills.Length; h++)
        {
            InsFightSkill hit = hitSkills[h];
            FightRunningHouse.AddRunning2(
                side == EnumWho.My ? EnumFightRunningType.DoAttackMy : EnumFightRunningType.DoAttackYou,
                side, sideSkill, 0, hit, EnumSkillType.None);
            int basicDamag = ApplyHitCheck(hit, FightDamageTool.CalcSkillFinalDamage(hit, side));
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

        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(otherOneSkill, attackerSide);

        for (int h = 0; h < hitSkills.Length; h++)
        {
            int basicDamag = ApplyHitCheck(hitSkills[h], FightDamageTool.CalcSkillFinalDamage(hitSkills[h], attackerSide));
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

        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(otherOneSkill, attackerSide);

        for (int h = 0; h < hitSkills.Length; h++)
        {
            InsFightSkill otherHit = hitSkills[h];
            int basicDamag = ApplyHitCheck(otherHit, FightDamageTool.CalcSkillFinalDamage(otherHit, attackerSide));
            GD.Print($"      被攻击第{h + 1}击: 原始伤害={basicDamag}, 最终伤害={basicDamag}");
            FightRunningHouse.AddRunning2(
                side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                side, sideSkill, basicDamag, otherHit);
        }
    }

    /// <summary>
    /// Side 系统（换宠），对方攻击 — 连击拆分 + 伤害计算
    /// 若正在换宠，使用将上场的精灵（switchingPet）作为防守方计算伤害；
    /// 否则使用当前场上精灵数据
    /// </summary>
    /// <param name="sideSkill">换宠方技能实例</param>
    /// <param name="side">换宠方（被攻击方）标识</param>
    /// <param name="otherOneSkill">对方攻击技能实例</param>
    /// <param name="switchingPet">将上场的精灵（换宠目标），null 则用当前场上精灵</param>
    public static void SideSystemWhenOtherOneAttack(InsFightSkill sideSkill, EnumWho side, InsFightSkill otherOneSkill, InsFightPetData switchingPet = null)
    {
        // side 是系统方（被攻击方），攻击方是对方
        EnumWho attackerSide = side == EnumWho.My ? EnumWho.You : EnumWho.My;

        // 攻击方精灵
        InsFightPetData attacker = attackerSide == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        // 防守方精灵：换宠中则使用将上场的精灵，否则用当前场上精灵
        InsFightPetData defender = switchingPet != null ? switchingPet
            : (side == EnumWho.My
                ? FightLandMyStandPet.Instance?.FightPetData
                : FightLandYouStandPet.Instance?.FightPetData);

        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(otherOneSkill, attackerSide);

        for (int h = 0; h < hitSkills.Length; h++)
        {
            InsFightSkill otherHit = hitSkills[h];
            int basicDamag = ApplyHitCheck(otherHit, FightDamageTool.CalcSkillFinalDamage(otherHit, attacker, defender));
            GD.Print($"      被攻击第{h + 1}击: 原始伤害={basicDamag}, 最终伤害={basicDamag}");
            FightRunningHouse.AddRunning2(
                side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                side, sideSkill, basicDamag, otherHit);
        }
    }
}
