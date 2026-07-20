using Godot;
using System;

/// <summary>
/// 行动执行后处理类
/// 负责对比双方行动先后顺序，按优先级/速度排序后返回
/// </summary>
public static class FightExeAfter
{
	/// <summary>
	/// 对比双方行动数组，按 Priority(先手值) → Speed(速度) 降序排列
	/// 同一索引位置双方行动对比，优先级/速度高的方先行
	/// 若某一方在某索引无有效行动，则不处理该位置
	/// </summary>
	/// <param name="myActs">我方行动数组 (TurnAction[9])</param>
	/// <param name="youActs">敌方行动数组 (TurnAction[9])</param>
	/// <param name="sortedMyActs">排序后的我方行动数组</param>
	/// <param name="sortedYouActs">排序后的敌方行动数组</param>
	public static void SortActionsByPriority(
		TurnAction[] myActs,
		TurnAction[] youActs,
		out TurnAction[] sortedMyActs,
		out TurnAction[] sortedYouActs)
	{
		if (myActs == null || youActs == null)
		{
			sortedMyActs = myActs;
			sortedYouActs = youActs;
			return;
		}

		int len = Math.Max(myActs.Length, youActs.Length);
		sortedMyActs = new TurnAction[len];
		sortedYouActs = new TurnAction[len];

		// 浅拷贝原始数据
		Array.Copy(myActs, sortedMyActs, myActs.Length);
		Array.Copy(youActs, sortedYouActs, youActs.Length);
	}

	/// <summary>
	/// 根据中心行动的行动类型和优先级，计算其在数组中的放置位置
	/// </summary>
	private static int ResolveSlotIndex(TurnAction centerAction)
	{
		if (centerAction == null)
			return 4; // 默认索引

		switch (centerAction.ActionType)
		{
			case TurnActionType.UseItem:
				return 2; // 使用道具 → 第2位

			case TurnActionType.UseSkill:
				// 根据技能优先级决定位置
				if (centerAction.Priority > 0)
					return 3; // 优先级+1（高优先级）→ 第3位
				else if (centerAction.Priority == 0)
					return 5; // 无优先级/普通 → 第5位
				else
					return 7; // 优先级-1（低优先级）→ 第7位

			case TurnActionType.SwitchPet:
				return 4; // 更换精灵 → 第4位

			case TurnActionType.Charge:
			default:
				return 5; // 聚能/默认 → 第5位
		}
	}

	/// <summary>
	/// 传入我方中心行动，根据行动类型和优先级计算放置位置，生成完整行动数组（长度9）
	/// </summary>
	/// <param name="myCenterAction">我方中心行动</param>
	/// <returns>包含中心行动的我方行动数组，长度为9</returns>
	public static TurnAction[] CreateMyActsFromCenter(TurnAction myCenterAction)
	{
		var acts = new TurnAction[9];
		if (myCenterAction != null)
		{
			int slotIndex = ResolveSlotIndex(myCenterAction);
			acts[slotIndex] = myCenterAction;
			GD.Print($"  └─ [FightExeAfter] 我方行动={myCenterAction.ActionType} → 放入 Slot[{slotIndex}]");
		}
		return acts;
	}

	/// <summary>
	/// 传入敌方中心行动，根据行动类型和优先级计算放置位置，生成完整行动数组（长度9）
	/// </summary>
	/// <param name="youCenterAction">敌方中心行动</param>
	/// <returns>包含中心行动的敌方行动数组，长度为9</returns>
	public static TurnAction[] CreateYouActsFromCenter(TurnAction youCenterAction)
	{
		var acts = new TurnAction[9];
		if (youCenterAction != null)
		{
			int slotIndex = ResolveSlotIndex(youCenterAction);
			acts[slotIndex] = youCenterAction;
			GD.Print($"  └─ [FightExeAfter] 敌方行动={youCenterAction.ActionType} → 放入 Slot[{slotIndex}]");
		}
		return acts;
	}
}