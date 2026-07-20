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
	/// <param name="myEndActs">输出的我方回合结束效果数组（TurnAction[4]）</param>
	/// <param name="youEndActs">输出的敌方回合结束效果数组（TurnAction[4]）</param>
	public static void ExecuteActions(
		TurnAction[] myTurnActs,
		TurnAction[] youTurnActs,
		out TurnAction[] myEndActs,
		out TurnAction[] youEndActs)
	{
		myEndActs = new TurnAction[4];
		youEndActs = new TurnAction[4];

		if (myTurnActs == null || youTurnActs == null)
			return;

		GD.Print($"    └─ [FightExeAction] 开始执行回合行动...");

		// 步骤1：通过 FightExeAfter 进行排序
		FightExeAfter.SortActionsByPriority(myTurnActs, youTurnActs, out var sortedMy, out var sortedYou);

		// 步骤2：按排序后的顺序，逐个索引执行双方行动
		int len = Math.Max(sortedMy.Length, sortedYou.Length);
		for (int i = 0; i < len; i++)
		{
			var myAction = i < sortedMy.Length ? sortedMy[i] : null;
			var youAction = i < sortedYou.Length ? sortedYou[i] : null;

			// 跳过双方都无行动的索引
			if (myAction == null && youAction == null)
				continue;

			ExecuteActionPair(myAction, youAction, myEndActs, youEndActs);
		}

		// 步骤3：处理回合结束效果

		GD.Print($"    └─ [FightExeAction] ✓");
	}

	/// <summary>
	/// 执行同一索引位置的我方行动和敌方行动
	/// 当双方同时有行动时，对比宠物速度决定先手
	/// </summary>
	private static void ExecuteActionPair(
		TurnAction myAction,
		TurnAction youAction,
		TurnAction[] myEndActs,
		TurnAction[] youEndActs)
	{
		if (myAction != null && youAction != null)
		{
			// 双方都有行动 → 对比宠物速度决定先手
			int mySpeed = FightLandMyStandPet.Instance?.GetSpeed() ?? 50;
			int youSpeed = FightLandYouStandPet.Instance?.GetSpeed() ?? 50;

			GD.Print($"    └─ [FightExeAction] 速度对比: 我方={mySpeed}, 敌方={youSpeed}");

			if (mySpeed > youSpeed)
			{
				// 我速度快 → 我先手
				ExecuteSingleAction(myAction, "my", myEndActs, youEndActs);
				ExecuteSingleAction(youAction, "you", myEndActs, youEndActs);
			}
			else if (youSpeed > mySpeed)
			{
				// 敌速度快 → 敌先手
				ExecuteSingleAction(youAction, "you", myEndActs, youEndActs);
				ExecuteSingleAction(myAction, "my", myEndActs, youEndActs);
			}
			else
			{
				// 速度相同 → 随机 50% 概率决定先手
				if (_random.Next(2) == 0)
				{
					GD.Print($"    └─ [FightExeAction] 速度相同，随机: 我方先手");
					ExecuteSingleAction(myAction, "my", myEndActs, youEndActs);
					ExecuteSingleAction(youAction, "you", myEndActs, youEndActs);
				}
				else
				{
					GD.Print($"    └─ [FightExeAction] 速度相同，随机: 敌方先手");
					ExecuteSingleAction(youAction, "you", myEndActs, youEndActs);
					ExecuteSingleAction(myAction, "my", myEndActs, youEndActs);
				}
			}
		}
		else
		{
			// 只有一方有行动
			if (myAction != null)
				ExecuteSingleAction(myAction, "my", myEndActs, youEndActs);

			if (youAction != null)
				ExecuteSingleAction(youAction, "you", myEndActs, youEndActs);
		}
	}

	/// <summary>
	/// 执行单个行动
	/// </summary>
	/// <param name="act">要执行的行动</param>
	/// <param name="side">行动方标识："my" 或 "you"</param>
	private static void ExecuteSingleAction(
		TurnAction act,
		string side,
		TurnAction[] myEndActs,
		TurnAction[] youEndActs)
	{
		// 维护我方和敌方宠物数据
		InsFightPetData myPet = FightLandMyStandPet.Instance?.FightPetData;
		InsFightPetData youPet = FightLandYouStandPet.Instance?.FightPetData;

		string label = side == "my" ? "🧑我方" : "👹敌方";
		GD.Print($"    └─ [FightExeAction] ({label})...");

		switch (act.ActionType)
		{
			case TurnActionType.UseSkill:
				ExecSkill(act, side, myPet, youPet);
				break;
			case TurnActionType.SwitchPet:
				ExecSwitch(act, side);
				break;
			case TurnActionType.UseItem:
				ExecUseItem(act, side, myPet, youPet);
				break;
			case TurnActionType.Charge:
			default:
				GD.Print($"      → [聚能] {side} 进行聚能");
				break;
		}
	}

	// ───────────────────────────── 技能执行 ─────────────────────────

	private static void ExecSkill(TurnAction act, string side, InsFightPetData myPet, InsFightPetData youPet)
	{
		InsFightSkill skill = act.FightSkill;
		GD.Print($"      → [ExecSkill] {side} " + myPet.PetName + "使用技能" + skill.Skill.SkillName);
	}

	// ───────────────────────────── 换宠执行 ─────────────────────────

	private static void ExecSwitch(TurnAction act, string side)
	{
		GD.Print($"      → [ExecSwitch] {side} 切换宠物");
	}

	// ───────────────────────────── 道具执行 ─────────────────────────

	private static void ExecUseItem(TurnAction act, string side, InsFightPetData myPet, InsFightPetData youPet)
	{
		// TODO: 使用道具逻辑（回血、加buff等）
		GD.Print($"      → [道具] {side} 使用道具");
	}

	// ───────────────────────────── 回合结束效果 ─────────────────────

}