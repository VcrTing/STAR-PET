using Godot;
using System;

/// <summary>
/// 技能运行工具
/// 负责执行技能的具体效果，按技能类型分派：
///   SkillType=1（攻击）→ 计算伤害并扣血
///   SkillType=2（防御）→ 防御/减伤效果
///   SkillType=3（状态）→ 状态变化效果
/// </summary>
public static class FightSkillRunTool
{
	/// <summary>
	/// 执行攻击技能
	/// 计算伤害并扣除防守方血量
	/// </summary>
	/// <param name="skill">战斗技能实例</param>
	/// <param name="attacker">攻击方宠物</param>
	/// <param name="defender">防守方宠物</param>
	/// <param name="side">攻击方标识："my" 或 "you"</param>
	public static void ExecAttack(InsFightSkill skill, InsFightPetData attacker, InsFightPetData defender, 
        string side, TurnAction[] sideActions)
	{
		if (skill?.Skill == null || attacker == null || defender == null)
			return;

        

		int damage = FightDamageTool.CalcDamage(skill, attacker, defender);
		defender.Hp = Math.Max(defender.Hp - damage, 0);

		string label = side == "my" ? "🧑我方" : "👹敌方";
		GD.Print($"      → [{label}攻击] {attacker.PetName} 使用 {skill.Skill.SkillName}，" +
				 $"对 {defender.PetName} 造成 {damage} 点伤害，" +
				 $"剩余 HP: {defender.Hp}/{defender.MaxHp}");
	}

	/// <summary>
	/// 执行防御技能
	/// 提升自身防御/魔防或附加护盾等
	/// </summary>
	/// <param name="skill">战斗技能实例</param>
	/// <param name="attacker">使用技能方宠物（自身）</param>
	/// <param name="defender">对方宠物</param>
	/// <param name="side">使用方标识："my" 或 "you"</param>
	public static void ExecDefense(InsFightSkill skill, InsFightPetData attacker, InsFightPetData defender, 
        string side, TurnAction[] sideActions)
	{
		if (skill?.Skill == null || attacker == null)
			return;

		string label = side == "my" ? "🧑我方" : "👹敌方";
		GD.Print($"      → [{label}防御] {attacker.PetName} 使用 {skill.Skill.SkillName}，进入防御状态。");
	}

	/// <summary>
	/// 执行状态技能
	/// 施加异常状态、增益/减益、治疗等
	/// </summary>
	/// <param name="skill">战斗技能实例</param>
	/// <param name="attacker">使用技能方宠物</param>
	/// <param name="defender">对方宠物</param>
	/// <param name="side">使用方标识："my" 或 "you"</param>
	public static void ExecStatus(InsFightSkill skill, InsFightPetData attacker, InsFightPetData defender, 
        string side, TurnAction[] sideActions)
	{
		if (skill?.Skill == null || attacker == null)
			return;

		string label = side == "my" ? "🧑我方" : "👹敌方";
		GD.Print($"      → [{label}状态] {attacker.PetName} 使用 {skill.Skill.SkillName}，产生状态效果。");
	}
}