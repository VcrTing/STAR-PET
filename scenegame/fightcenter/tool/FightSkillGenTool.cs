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
    /// 如果是则根据 HitCount + Power 连击数加成 生成多个 InsFightSkill 实例，
    /// 每个实例代表一次连击中的一击。
    /// 第一击保留 ActualPpCost，后续连击的 ActualPpCost = 0。
    /// </summary>
    /// <param name="sourceSkill">源技能实例</param>
    /// <param name="side">使用技能的阵营（用于读取该精灵的 ComboCount Power 连击数加成）</param>
    /// <returns>连击分解后的 InsFightSkill 数组。非连击技能返回包含自身的单元素数组</returns>
    public static InsFightSkill[] GenerateHitCombo(InsFightSkill sourceSkill, EnumWho side = EnumWho.My)
    {
        if (sourceSkill?.Skill == null)
            return Array.Empty<InsFightSkill>();

        // 不是连击技能，返回包含自身的单元素数组
        if (!sourceSkill.Skill.IsHitCombo)
            return new[] { sourceSkill };

        // 计算最终总连击数：源技能连击数 + Power 连击加成，上限99
        int hitCount = GetHitCount(sourceSkill, side);
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

    /// <summary>
    /// 计算连击技能最终总连击数
    /// = 源技能实际连击数（ActualHitCount）+ Power 连击数加成（ComboCount），上限 99
    /// </summary>
    /// <param name="sourceSkill">源技能实例</param>
    /// <param name="side">使用技能的阵营（用于读取该精灵的 ComboCount Power 连击数加成）</param>
    /// <returns>最终总连击数</returns>
    private static int GetHitCount(InsFightSkill sourceSkill, EnumWho side)
    {
        // 使用实际连击数（战斗中可被特性/道具改变），上限99
        int hitCount = Math.Min(sourceSkill.ActualHitCount, 99);

        // 叠加 Power 连击数加成（ComboCount）：源技能连击数 + 精灵身上的连击 Power 层数
        int powerCombo = GetPowerComboCount(side);
        hitCount += powerCombo;
        hitCount = Math.Min(hitCount, 99);
        if (powerCombo > 0)
        {
            GD.Print($"      [FightSkillGenTool] 连击数叠加: 源技能={sourceSkill.Skill.HitCount} + Power连击={powerCombo} → 总连击={hitCount}");
        }
        return hitCount;
    }

    /// <summary>
    /// 获取指定阵营场上精灵的 Power 连击数加成（ComboCount 层数之和）
    /// 用于连击技能生成时叠加连击次数
    /// </summary>
    /// <param name="side">阵营（My=读取我方精灵，You=读取敌方精灵）</param>
    /// <returns>连击数加成（无 Power 或未找到精灵返回 0）</returns>
    private static int GetPowerComboCount(EnumWho side)
    {
        int powerCombo = 0;
        string petUuid = side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData?.PetUuid
            : FightLandYouStandPet.Instance?.FightPetData?.PetUuid;
        if (string.IsNullOrWhiteSpace(petUuid))
            return 0;

        powerCombo = side == EnumWho.My
            ? FightMyStandPowerManager.Instance?.GetComboCount(petUuid) ?? 0
            : FightYouStandPowerManager.Instance?.GetComboCount(petUuid) ?? 0;
        return powerCombo;
    }
}
