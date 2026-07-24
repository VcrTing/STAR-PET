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
public static class FightSkillJudgeTool
{
    /// <summary>
    /// 从 TurnAction 数组中查找所有有效的 InsFightSkill，返回数组
    /// </summary>
    /// <param name="actions">TurnAction 数组</param>
    /// <returns>有效的 InsFightSkill 数组，无则返回空数组</returns>
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

    // Side 攻击 对方 也攻击
    public static void SideAttackWhenOtherOneAttack(InsFightSkill sideSkill, EnumWho side, InsFightPetData attacker, InsFightPetData defender)
    {
        
        // 2. 根据 sideSkill 计算出连击后的 InsFightSkill[]
        //    如为连击技能则拆分为多段，非连击技能则返回包含自身的单元素数组
        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(sideSkill);

        // 计算对方（被攻击方）的标识
        EnumWho targetSide = side == EnumWho.My ? EnumWho.You : EnumWho.My;

        // 3. 循环处理每一段连击
        for (int h = 0; h < hitSkills.Length; h++)
        {
            InsFightSkill hit = hitSkills[h];

            // 3a. 生成执行攻击阶段 FightRunning，记录本次连击的执行
            FightRunningHouse.AddRunning(
                side == EnumWho.My ? EnumFightRunningType.DoAttackMy : EnumFightRunningType.DoAttackYou,
                side, hit, 0, EnumSkillType.None);

            // 3b. 用当前连击段计算攻击对方要扣的血
            int basicDamag = FightDamageTool.CalcBasicDamage(hit, attacker, defender);

            // 3c. 生成扣血阶段 FightRunning，扣除的是对面的血
            FightRunningHouse.AddRunning(
                side == EnumWho.My ? EnumFightRunningType.DoDamageYou : EnumFightRunningType.DoDamageMy,
                targetSide, hit, basicDamag);
        }
    }
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
        FightRunningHouse.AddRunning(
            side == EnumWho.My ? EnumFightRunningType.StartAttackMy : EnumFightRunningType.StartAttackYou,
            side, sideSkill);

        // 检查应对
        InsFightSkill[] sideSkills = GetFightSkillFromActions(otherOneActions);

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
                SideAttackWhenOtherOneAttack(sideSkill, side, attacker, defender);
            }
            // 防御
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.DEFENSE)
            {
                // 检查应对防御
                if (bingoType == (int)EnumSkillType.DEFENSE)
                {
                    // 应对防御，暂无作用
                }
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoAttackMy : EnumFightRunningType.DoAttackYou,
                    side, otherOneSkill, 0, isBingo ? EnumSkillType.STATUS : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对防御暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, otherOneSkill, basicDamag);
            }
            // 状态
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.STATUS)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.STATUS)
                {
                    isBingo = true;
                    // 2. side 应对状态，FightRunning 加入CurrentRunArray
                    /*
                    FightRunningHouse.AddRunning(
                        side == EnumWho.My ? EnumFightRunningType.BingoStatusMy : EnumFightRunningType.BingoStatusYou,
                        side, otherOneSkill);
                    */
                    //
                    GD.Print(side + " 攻击应对了状态----------");
                }
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoAttackMy : EnumFightRunningType.DoAttackYou,
                    side, otherOneSkill, 0, isBingo ? EnumSkillType.STATUS : EnumSkillType.None);
                // 3. side 计算自己要扣的血，根据 otherOneSkill 计算 自身要扣的血量 basicDamag，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, otherOneSkill, basicDamag);
            }
        }
	}

    // Side 防御 对方 攻击
    public static void SideDefenseWhenOtherOneAttack(InsFightSkill sideSkill, EnumWho side, InsFightPetData attacker, InsFightPetData defender, InsFightSkill otherOneSkill)
    {
        // 3. 计算 otherOneSkill 的连击，生成 InsFightSkill[]
        //    如对方为连击技能则拆分为多段，非连击技能则返回包含自身的单元素数组
        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(otherOneSkill);

        // 4. 循环处理对方每一段连击，计算我方（side）要扣的血量
        for (int h = 0; h < hitSkills.Length; h++)
        {
            InsFightSkill otherHit = hitSkills[h];

            // 4a. 根据每一连击 otherHit，计算出原始伤害
            int basicDamag = FightDamageTool.CalcBasicDamage(otherHit, defender, attacker);

            // 4b. 提出防御减伤，计算出减伤后的伤害
            int jsRatio = sideSkill.Skill.DamageReductionRate;
            if (jsRatio == null || jsRatio == 0) { GD.Print("是不是忘记定义减伤率了"); }
            int finalDamage = (int)(basicDamag * ((float)jsRatio / 100));

            GD.Print($"      防御第{h + 1}击: 原始伤害={basicDamag}, 减伤率={jsRatio}%, 最终伤害={finalDamage}");

            // 4c. 生成扣血阶段 FightRunning，扣的是我方（side）的血
            FightRunningHouse.AddRunning(
                side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                side, otherHit, finalDamage);
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
        FightRunningHouse.AddRunning(
            side == EnumWho.My ? EnumFightRunningType.StartDefenseMy : EnumFightRunningType.StartDefenseYou,
            side, sideSkill);

        bool isBingo = false;
        // 检查应对
        InsFightSkill[] sideSkills = GetFightSkillFromActions(otherOneActions);
        for (int i = 0; i < sideSkills.Length; i++)
        {
            InsFightSkill otherOneSkill = sideSkills[i];
            if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.ATTACK)
            {
                // 检查应对攻击
                if (bingoType == (int)EnumSkillType.ATTACK)
                {
                    isBingo = true;
                    // 2. 应对攻击，FightRunning 加入CurrentRunArray
                    /*
                    FightRunningHouse.AddRunning(
                        side == EnumWho.My ? EnumFightRunningType.BingoAttackMy : EnumFightRunningType.BingoAttackYou,
                        side, otherOneSkill);
                    */
                    GD.Print(side + " 防御应对了攻击----------");
                }
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoDefenseMy : EnumFightRunningType.DoDefenseYou,
                    side, otherOneSkill, 0, isBingo ? EnumSkillType.ATTACK : EnumSkillType.None);

                SideDefenseWhenOtherOneAttack(sideSkill, side, attacker, defender, otherOneSkill);
            }
            // 应对防御
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.DEFENSE)
            {
                // 检查应对防御
                if (bingoType == (int)EnumSkillType.DEFENSE)
                {
                    // 应对防御，暂无作用
                }
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoDefenseMy : EnumFightRunningType.DoDefenseYou,
                    side, otherOneSkill, 0, isBingo ? EnumSkillType.ATTACK : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对防御暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, otherOneSkill, basicDamag);
            }
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.STATUS)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.STATUS)
                {
                    // 应对状态，暂无作用
                }
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoDefenseMy : EnumFightRunningType.DoDefenseYou,
                    side, otherOneSkill, 0, isBingo ? EnumSkillType.ATTACK : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对状态暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, otherOneSkill, basicDamag);
            }
        }
    }

    public static void SideStatusWhenOtherOneAttack(InsFightSkill sideSkill, EnumWho side, InsFightPetData attacker, InsFightPetData defender, InsFightSkill otherOneSkill)
    {
        /*
        // 2. side 计算自己要扣的血，根据 otherOneSkill 计算 自身要扣的血量 basicDamag 加入CurrentRunArray
        int basicDamag = FightDamageTool.CalcBasicDamage(otherOneSkill, defender, attacker);
        FightRunningHouse.AddRunning(
            side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
            side, otherOneSkill, basicDamag);
        */
        // 3. 计算 otherOneSkill 的连击，生成 InsFightSkill[]
        //    如对方为连击技能则拆分为多段，非连击技能则返回包含自身的单元素数组
        InsFightSkill[] hitSkills = FightSkillGenTool.GenerateHitCombo(otherOneSkill);

        // 4. 循环处理对方每一段连击，计算我方（side）要扣的血量
        for (int h = 0; h < hitSkills.Length; h++)
        {
            InsFightSkill otherHit = hitSkills[h];
            // 4a. 根据每一连击 otherHit，计算出原始伤害
            int basicDamag = FightDamageTool.CalcBasicDamage(otherHit, defender, attacker);
            GD.Print($"      被攻击第{h + 1}击: 原始伤害={basicDamag}, 最终伤害={basicDamag}");
            // 4b. 生成扣血阶段 FightRunning，扣的是我方（side）的血
            FightRunningHouse.AddRunning(
                side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                side, otherHit, basicDamag);
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
        FightRunningHouse.AddRunning(
            side == EnumWho.My ? EnumFightRunningType.StartStatusMy : EnumFightRunningType.StartStatusYou,
            side, sideSkill);

        bool isBingo = false;
        // 检查应对
        InsFightSkill[] sideSkills = GetFightSkillFromActions(otherOneActions);
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
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoStatusMy : EnumFightRunningType.DoStatusYou,
                    side, otherOneSkill, 0, isBingo ? EnumSkillType.DEFENSE : EnumSkillType.None);
                SideStatusWhenOtherOneAttack(sideSkill, side, attacker, defender, otherOneSkill);
            }
            // 应对防御
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.DEFENSE)
            {
                // 检查应对防御
                if (bingoType == (int)EnumSkillType.DEFENSE)
                {
                    // 2. side 应对防御，暂无作用，FightRunning 加入CurrentRunArray
                    /*
                    FightRunningHouse.AddRunning(
                        side == EnumWho.My ? EnumFightRunningType.BingoDefenseMy : EnumFightRunningType.BingoDefenseYou,
                        side, otherOneSkill);
                    */
                    isBingo = true;
                    //
                    GD.Print(side + " 状态应对了防御----------");
                }
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoStatusMy : EnumFightRunningType.DoStatusYou,
                    side, otherOneSkill, 0, isBingo ? EnumSkillType.DEFENSE : EnumSkillType.None);
                // 3. side 计算自己要扣的血量，应对防御暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, otherOneSkill, basicDamag);
            }
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.STATUS)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.STATUS)
                {
                    // 应对状态，暂无作用
                }
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoStatusMy : EnumFightRunningType.DoStatusYou,
                    side, otherOneSkill, 0, isBingo ? EnumSkillType.DEFENSE : EnumSkillType.None);
                // 2. side 计算自己要扣的血量，应对状态暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, otherOneSkill, basicDamag);
            }
        }
	}
}