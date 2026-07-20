using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 行动执行器
/// 负责执行双方行动数组，处理技能、换宠、道具等行动，产生回合结束效果
/// </summary>
public static class FightExeAction
{
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
	/// 先执行我方行动，再执行敌方行动
	/// </summary>
	private static void ExecuteActionPair(
		TurnAction myAction,
		TurnAction youAction,
		TurnAction[] myEndActs,
		TurnAction[] youEndActs)
	{
		if (myAction == null)
		{
			
		}
		if (youAction == null)
		{
			
		}
		else
		{
			
		}
	}

}