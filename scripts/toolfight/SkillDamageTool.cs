using Godot;

/// <summary>
/// 技能伤害计算工具
/// 用于计算技能最终伤害数值，处理攻击技能、防御技能、状态技能的不同伤害逻辑
/// </summary>
public static class SkillDamageTool
{
    /// <summary>
    /// 计算技能最终伤害
    /// </summary>
    /// <param name="attacker">攻击方精灵数据</param>
    /// <param name="skill">技能实例</param>
    /// <param name="side">攻击方阵营</param>
    /// <returns>计算后的伤害值（非负整数）</returns>
    public static int CalculateDamage(InsFightPetData attacker, InsFightSkill skill, EnumWho side)
    {
        if (attacker == null || skill?.Skill == null)
            return 0;

        // 基础伤害取技能的最终威力
        int baseDamage = skill.ActualAttackValue;

        // TODO: 完善伤害公式：等级修正、属性克制、Buff 修正、随机浮动等
        // 当前只返回基础威力值

        return Mathf.Max(baseDamage, 0);
    }
}