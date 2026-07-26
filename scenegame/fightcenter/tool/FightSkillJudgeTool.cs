using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 技能判断工具
/// 负责判断和执行技能的具体效果，按技能类型分派：
///   SkillType=1（攻击）→ 计算伤害并扣血
///   SkillType=2（防御）→ 防御/减伤效果
///   SkillType=3（状态）→ 状态变化效果
/// </summary>
/// <summary>
/// 技能判断工具
/// 负责判断和执行技能的具体效果，按技能类型分派：
///   SkillType=1（攻击）→ 计算伤害并扣血
///   SkillType=2（防御）→ 防御/减伤效果
///   SkillType=3（状态）→ 状态变化效果
/// 辅助方法已移至 FightSkillJudge2Tool
/// </summary>
public static class FightSkillJudgeTool
{
	/// <summary>
	/// 执行攻击技能
	/// 计算伤害并扣除防守方血量
	/// </summary>
	/// <param name="skill">战斗技能实例</param>
	/// <param name="attacker">攻击方宠物</param>
	/// <param name="defender">防守方宠物</param>
	/// <param name="side">攻击方标识</param>
	public static void ExecAttack(InsFightSkill sideSkill, InsFightPetData attacker, InsFightPetData defender, 
        EnumWho side, TurnAction[] otherOneActions)
	{
		if (sideSkill?.Skill == null || attacker == null || defender == null)
			return;

        // 提取应对
        int bingoType = sideSkill.Skill.BingoSkillType;

        // 1. side 开始攻击，FightRunning 加入CurrentRunArray
        FightRunningHouse.AddRunning2(
            side == EnumWho.My ? EnumFightRunningType.StartAttackMy : EnumFightRunningType.StartAttackYou,
            side, sideSkill, 0, sideSkill);

        // 检查应对
        InsFightSkill[] sideSkills = FightSkillJudge2Tool.GetFightSkillFromActions(otherOneActions);

        // 打印 sideSkills 数组内容
        // string sideLabel = side == EnumWho.My ? "🧑我方" : "👹敌方";
        // GD.Print($"      → [{sideLabel}攻击] sideSkills 共 {sideSkills.Length} 个:");
        bool isBingo = false;
        for (int i = 0; i < sideSkills.Length; i++)
        {
            InsFightSkill otherOneSkill = sideSkills[i];
            // 攻击
            if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.ATTACK)
            {
                // 检查应对攻击
                if (bingoType == (int)EnumSkillType.ATTACK)
                {
                    // 应对攻击：扣除对面血量
                }
                FightSkillJudge2Tool.SideAttackWhenOtherOneAttack(sideSkill, side, attacker, defender);
            }
            // 防御
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.DEFENSE)
            {
                // 检查应对防御
                if (bingoType == (int)EnumSkillType.DEFENSE)
                {
                    // 应对防御，暂无作用
                }
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoAttackMy : EnumFightRunningType.DoAttackYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.STATUS : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对防御暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
            // 状态
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.STATUS)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.STATUS)
                {
                    isBingo = true;
                    GD.Print(side + " 攻击应对了状态----------");
                }
                // 2. side 应对状态，FightRunning 加入CurrentRunArray
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoAttackMy : EnumFightRunningType.DoAttackYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.STATUS : EnumSkillType.None);
                // 3. side 计算自己要扣的血，根据 otherOneSkill 计算 自身要扣的血量 basicDamag，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
            // 系统技能
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.SYSTEM)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.SYSTEM)
                {
                }
                // 2. side 应对状态，FightRunning 加入CurrentRunArray
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoAttackMy : EnumFightRunningType.DoAttackYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.SYSTEM : EnumSkillType.None);
                // 3. side 计算自己要扣的血，根据 otherOneSkill 计算 自身要扣的血量 basicDamag，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
        }
	}

	/// <summary>
	/// 执行防御技能
	/// 提升自身防御/魔防或附加护盾等
	/// </summary>
	/// <param name="skill">战斗技能实例</param>
	/// <param name="attacker">使用技能方宠物（自身）</param>
	/// <param name="defender">对方宠物</param>
	/// <param name="side">使用方标识</param>
	public static void ExecDefense(InsFightSkill sideSkill, InsFightPetData attacker, InsFightPetData defender, 
        EnumWho side, TurnAction[] otherOneActions)
	{
		if (sideSkill?.Skill == null || attacker == null)
			return;

        // 提取应对
        int bingoType = sideSkill.Skill.BingoSkillType;
    
        // 1. side 开始防御，FightRunning 加入CurrentRunArray
        FightRunningHouse.AddRunning2(
            side == EnumWho.My ? EnumFightRunningType.StartDefenseMy : EnumFightRunningType.StartDefenseYou,
            side, sideSkill, 0, null);

        bool isBingo = false;
        // 检查应对
        InsFightSkill[] sideSkills = FightSkillJudge2Tool.GetFightSkillFromActions(otherOneActions);
        for (int i = 0; i < sideSkills.Length; i++)
        {
            InsFightSkill otherOneSkill = sideSkills[i];
            if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.ATTACK)
            {
                // 检查应对攻击
                if (bingoType == (int)EnumSkillType.ATTACK)
                {
                    isBingo = true;
                    GD.Print(side + " 防御应对了攻击----------");
                }
                // 2. 应对攻击，FightRunning 加入CurrentRunArray
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDefenseMy : EnumFightRunningType.DoDefenseYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.ATTACK : EnumSkillType.None);
                FightSkillJudge2Tool.SideDefenseWhenOtherOneAttack(sideSkill, side, attacker, defender, otherOneSkill);
            }
            // 应对防御
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.DEFENSE)
            {
                // 检查应对防御
                if (bingoType == (int)EnumSkillType.DEFENSE)
                {
                    // 应对防御，暂无作用
                }
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDefenseMy : EnumFightRunningType.DoDefenseYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.DEFENSE : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对防御暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.STATUS)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.STATUS)
                {
                    // 应对状态，暂无作用
                }
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDefenseMy : EnumFightRunningType.DoDefenseYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.STATUS : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对状态暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.SYSTEM)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.SYSTEM)
                {
                    // 应对状态，暂无作用
                }
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDefenseMy : EnumFightRunningType.DoDefenseYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.SYSTEM : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对状态暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
        }
    }

	/// <summary>
	/// 执行状态技能
	/// 施加异常状态、增益/减益、治疗等
	/// </summary>
	/// <param name="skill">战斗技能实例</param>
	/// <param name="attacker">使用技能方宠物</param>
	/// <param name="defender">对方宠物</param>
	/// <param name="side">使用方标识</param>
	public static void ExecStatus(InsFightSkill sideSkill, InsFightPetData attacker, InsFightPetData defender, 
        EnumWho side, TurnAction[] otherOneActions)
	{
		if (sideSkill?.Skill == null || attacker == null)
			return;

        // 提取应对
        int bingoType = sideSkill.Skill.BingoSkillType;

        // 1. side 开始状态，FightRunning 加入CurrentRunArray
        FightRunningHouse.AddRunning2(
            side == EnumWho.My ? EnumFightRunningType.StartStatusMy : EnumFightRunningType.StartStatusYou,
            side, sideSkill, 0, null);

        bool isBingo = false;
        // 检查应对
        InsFightSkill[] sideSkills = FightSkillJudge2Tool.GetFightSkillFromActions(otherOneActions);
        for (int i = 0; i < sideSkills.Length; i++)
        {
            InsFightSkill otherOneSkill = sideSkills[i];
            if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.ATTACK)
            {
                // 检查应对攻击
                if (bingoType == (int)EnumSkillType.ATTACK)
                {
                    // 应对攻击，没什么影响
                }
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoStatusMy : EnumFightRunningType.DoStatusYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.DEFENSE : EnumSkillType.None);
                FightSkillJudge2Tool.SideStatusWhenOtherOneAttack(sideSkill, side, attacker, defender, otherOneSkill);
            }
            // 应对防御
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.DEFENSE)
            {
                // 检查应对防御
                if (bingoType == (int)EnumSkillType.DEFENSE)
                {
                    isBingo = true;
                    GD.Print(side + " 状态应对了防御----------");
                }
                // 2. side 应对防御，暂无作用，FightRunning 加入CurrentRunArray
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoStatusMy : EnumFightRunningType.DoStatusYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.DEFENSE : EnumSkillType.None);
                // 3. side 计算自己要扣的血量，应对防御暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.STATUS)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.STATUS)
                {
                    // 应对状态，暂无作用
                }
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoStatusMy : EnumFightRunningType.DoStatusYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.DEFENSE : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对状态暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.SYSTEM)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.SYSTEM)
                {
                    // 应对状态，暂无作用
                }
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoStatusMy : EnumFightRunningType.DoStatusYou,
                    side, sideSkill, 0, otherOneSkill, isBingo ? EnumSkillType.SYSTEM : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对状态暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
        }
	}
}