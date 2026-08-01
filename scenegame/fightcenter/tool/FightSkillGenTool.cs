using Godot;
using System;

/// <summary>
/// 技能生成工具
/// 负责连击技能生成
/// </summary>
public static class FightSkillGenTool
{
    /// <summary>
    /// 连击技能生成
    /// 根据传入的 InsFightSkill 判断是否为连击技能（IsHitCombo），
    /// 如果是则根据 HitCount 生成多个 InsFightSkill 实例，
    /// 每个实例代表一次连击中的一击。
    /// 第一击保留 ActualPpCost，后续连击的 ActualPpCost = 0。
    /// </summary>
    /// <param name="sourceSkill">源技能实例</param>
    /// <returns>连击分解后的 InsFightSkill 数组。非连击技能返回包含自身的单元素数组</returns>
    public static InsFightSkill[] GenerateHitCombo(InsFightSkill sourceSkill)
    {
        if (sourceSkill?.Skill == null)
            return Array.Empty<InsFightSkill>();

        // 不是连击技能，返回包含自身的单元素数组
        if (!sourceSkill.Skill.IsHitCombo)
            return new[] { sourceSkill };

        // 使用实际连击数（战斗中可被特性/道具改变），上限99
        int hitCount = Math.Min(sourceSkill.ActualHitCount, 99);
        if (hitCount <= 1)
            return new[] { sourceSkill };

        InsFightSkill[] result = new InsFightSkill[hitCount];

        for (int i = 0; i < hitCount; i++)
        {
            // 用 FromInsSkill 创建克隆（Skill 的 setter 是 private）
            InsFightSkill hitSkill = InsFightSkill.FromInsSkill(sourceSkill.Skill, sourceSkill.SlotIndex);
            if (hitSkill == null)
            {
                GD.PrintErr($"      ❌ FightSkillGenTool: 连击技能克隆失败 '{sourceSkill.Skill.SkillName}'");
                return Array.Empty<InsFightSkill>();
            }

            // 覆盖战斗状态
            hitSkill.IsFrozen = sourceSkill.IsFrozen;
            hitSkill.CooldownTurns = sourceSkill.CooldownTurns;
            hitSkill.ActualPetType = sourceSkill.ActualPetType;
            hitSkill.DisplayAttackValue = sourceSkill.DisplayAttackValue;
            // 每一个伤害都是连击伤害
            hitSkill.ActualAttackValue = sourceSkill.Skill.AttackValue;
            // 第一击消耗PP，后续连击不消耗PP
            hitSkill.ActualPpCost = i == 0 ? sourceSkill.ActualPpCost : 0;

            result[i] = hitSkill;
        }

        return result;
    }
}