using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 行动执行器
/// 负责执行双方行动数组，处理技能、换宠、道具等行动，产生回合结束效果
/// </summary>
public static class FightExeAction
{
	private static readonly System.Random _random = new();

	/// <summary>
	/// 执行双方行动数组
	/// 先通过 FightExeAfter.SortActionsByPriority 排序，
	/// 然后按排序后的顺序逐个索引执行双方行动
	/// </summary>
	/// <param name="myTurnActs">我方行动数组（TurnAction[9]）</param>
	/// <param name="youTurnActs">敌方行动数组（TurnAction[9]）</param>
	public static void ExecuteActions(
		TurnAction[] myTurnActs,
		TurnAction[] youTurnActs)
	{
		if (myTurnActs == null || youTurnActs == null)
			return;

		// 步骤1：通过 FightExeAfter 进行技能优先级排序
		FightExeAfter.SortActionsByPriority(myTurnActs, youTurnActs, out var sortedMy, out var sortedYou);

		//
		GD.Print("TurnAction 排序完成，开始进入 Running 环节。");
		// 步骤2：按排序后的顺序，逐个索引执行双方行动
		// 根据速度+应对，把行动做入 FightRunning[] 
		int len = Math.Max(sortedMy.Length, sortedYou.Length);
		for (int i = 0; i < len; i++)
		{
			TurnAction myAction = i < sortedMy.Length ? sortedMy[i] : null;
			TurnAction youAction = i < sortedYou.Length ? sortedYou[i] : null;

			// 跳过双方都无行动的索引
			if (myAction == null && youAction == null)
				continue;

			GD.Print($"    └─ [FightExeAction] 执行回合行动 index = " + i);
			ExecuteActionPair(myAction, sortedMy, sortedYou, youAction);
		}

		// 步骤3：技能循序判断结束，构建Running结束
		FightRunningTool.BuildEndSkill();

		// 步骤5：加入血量检查 Running
		FightRunningTool.BuildCheckHp();

		// 步骤6：最终执行 Running[]
		FightRunningExe.ExecuteAll();

		// 步骤7：清空，开始下一个回合
		FightRunningHouse.ClearRunArray();
	}

	/// <summary>
	/// 执行同一索引位置的我方行动和敌方行动
	/// 当双方同时有行动时，对比宠物速度决定先手
	/// </summary>
	private static void ExecuteActionPair(
		TurnAction myAction,
		TurnAction[] myActions,
		TurnAction[] youActions,
		TurnAction youAction)
	{
		if (myAction != null && youAction != null)
		{
			// 双方都有行动 → 对比宠物速度决定先手
			int mySpeed = FightLandMyStandPet.Instance?.GetSpeed() ?? 10;
			int youSpeed = FightLandYouStandPet.Instance?.GetSpeed() ?? 10;

			GD.Print($"    └─ [FightExeAction] 速度对比: 我方={mySpeed}, 敌方={youSpeed}");

			if (mySpeed > youSpeed)
			{
				// 我速度快 → 我先手
				ExecuteSingleAction(myAction, EnumWho.My, youActions);
				ExecuteSingleAction(youAction, EnumWho.You, myActions);
			}
			else if (youSpeed > mySpeed)
			{
				// 敌速度快 → 敌先手
				ExecuteSingleAction(youAction, EnumWho.You, myActions);
				ExecuteSingleAction(myAction, EnumWho.My, youActions);
			}
			else
			{
				// 速度相同 → 随机 50% 概率决定先手
				if (_random.Next(2) == 0)
				{
					GD.Print($"    └─ [FightExeAction] 速度相同，随机: 我方先手");
					ExecuteSingleAction(myAction, EnumWho.My, youActions);
					ExecuteSingleAction(youAction, EnumWho.You, myActions);
				}
				else
				{
					GD.Print($"    └─ [FightExeAction] 速度相同，随机: 敌方先手");
					ExecuteSingleAction(youAction, EnumWho.You, myActions);
					ExecuteSingleAction(myAction, EnumWho.My, youActions);
				}
			}
		}
		else
		{
			// 只有一方有行动
			if (myAction != null)
				ExecuteSingleAction(myAction, EnumWho.My, youActions);

			if (youAction != null)
				ExecuteSingleAction(youAction, EnumWho.You, myActions);
		}
	}

	/// <summary>
	/// 执行单个行动
	/// </summary>
	/// <param name="act">要执行的行动</param>
	/// <param name="side">行动方标识</param>
	private static void ExecuteSingleAction(
		TurnAction sideAct,
		EnumWho side,
		TurnAction[] otherOneActions)
	{
		// 维护我方和敌方宠物数据
		InsFightPetData myPet = FightLandMyStandPet.Instance?.FightPetData;
		InsFightPetData youPet = FightLandYouStandPet.Instance?.FightPetData;

		string label = side == EnumWho.My ? "🧑我方" : "👹敌方";
		GD.Print($"    └─ [FightExeAction] ({label})...");

		switch (sideAct.ActionType)
		{
			case TurnActionType.UseSkill:
				ExecSkill(sideAct, side, otherOneActions, myPet, youPet);
				break;
			case TurnActionType.SwitchPet:
				ExecSwitch(sideAct, side, otherOneActions, myPet, youPet);
				break;
			case TurnActionType.UseItem:
				ExecUseItem(sideAct, side, otherOneActions, myPet, youPet);
				break;
			case TurnActionType.Charge:
			default:
				GD.Print($"      → [聚能] {side} 进行聚能");
				break;
		}
	}

	// ───────────────────────────── 技能执行 ─────────────────────────

	private static void ExecSkill(TurnAction sideAct, EnumWho side,
		TurnAction[] otherOneActions, InsFightPetData myPet, InsFightPetData youPet)
	{
		InsFightSkill skill = sideAct.FightSkill;
		InsFightPetData nowPet = side == EnumWho.My ? myPet : youPet;
		InsFightPetData targetPet = side == EnumWho.My ? youPet : myPet;

		GD.Print($"      → [ExecSkill] {side} " + nowPet.PetName + " 使用技能：" + skill.Skill.SkillName);

		// 根据技能类型分派执行
		switch (skill.Skill.SkillType)
		{
			case 1: // 攻击技能
				FightSkillJudgeTool.ExecAttack(skill, nowPet, targetPet, side, otherOneActions);
				break;
			case 2: // 防御技能
				FightSkillJudgeTool.ExecDefense(skill, nowPet, targetPet, side, otherOneActions);
				break;
			case 3: // 状态技能
				FightSkillJudgeTool.ExecStatus(skill, nowPet, targetPet, side, otherOneActions);
				break;
			default:
				GD.Print($"      → [ExecSkill] 未知技能类型: {skill.Skill.SkillType}");
				break;
		}
	}

	// ───────────────────────────── 换宠执行 ─────────────────────────

	private static void ExecSwitch(TurnAction act, EnumWho side,
		TurnAction[] otherOneActions, InsFightPetData myPet, InsFightPetData youPet)
	{
		GD.Print($"      → [ExecSwitch] {side} 切换宠物");
	}

	// ───────────────────────────── 道具执行 ─────────────────────────

	private static void ExecUseItem(TurnAction act, EnumWho side,
		TurnAction[] otherOneActions, InsFightPetData myPet, InsFightPetData youPet)
	{
		// TODO: 使用道具逻辑（回血、加buff等）
		GD.Print($"      → [道具] {side} 使用道具");
	}

	// ───────────────────────────── 回合结束效果 ─────────────────────

}