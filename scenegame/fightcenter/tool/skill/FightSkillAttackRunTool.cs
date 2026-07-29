using Godot;
using System;

/// <summary>
/// 技能攻击运行工具
/// 提供攻击技能执行时可复用的方法
/// </summary>
public static class FightSkillAttackRunTool
{
    private static readonly Random _random = new();

    /// <summary>
    /// 执行秒杀/即死效果判定与扣血（基于 InstantKillRate + InstantKillDamage）
    /// 触发成功后使用 FightPetHpTool 对敌方精灵造成固定伤害
    /// </summary>
    /// <param name="index">日志索引</param>
    /// <param name="sideSkill">攻击方技能实例</param>
    /// <param name="run">当前回合运行数据（用于确定攻击方和目标方）</param>
    /// <param name="skillName">技能名（用于日志输出）</param>
    /// <param name="effectName">效果名（如"秒杀"、"冰核冲击"，用于日志输出）</param>
    /// <returns>true=触发秒杀, false=未触发</returns>
    public static bool ExecuteInstantKill(int index, InsFightSkill sideSkill, FightRunning run, string skillName, string effectName)
    {
        if (sideSkill?.Skill == null)
            return false;

        float instantKillRate = sideSkill.Skill.InstantKillRate;
        int instantKillDamage = sideSkill.Skill.InstantKillDamage;

        if (instantKillRate <= 0.0f || instantKillDamage <= 0)
            return false;

        // 掷骰判定是否触发
        float roll = (float)_random.NextDouble() * 100.0f;
        if (roll < instantKillRate)
        {
            GD.Print($"      [{index}] 🧊 {skillName}·{effectName}触发！掷骰={roll:F2}%，概率={instantKillRate}%，造成 {instantKillDamage} 点固定伤害！");

            // 使用 FightPetHpTool 对敌方造成固定伤害
            EnumWho targetSide = run.Side == EnumWho.My ? EnumWho.You : EnumWho.My;
            FightPetHpTool.DeductHp(targetSide, instantKillDamage, index);

            return true;
        }

        GD.Print($"      [{index}] {skillName}·{effectName}未触发，掷骰={roll:F2}%，概率={instantKillRate}%");
        return false;
    }
}