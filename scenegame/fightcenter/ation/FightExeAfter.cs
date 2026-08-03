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

		// 步骤1：从原始数组中提取 center 行动（通常位于索引 4）
		TurnAction myCenter = null;
		TurnAction youCenter = null;
		for (int i = 0; i < myActs.Length; i++)
		{
			if (myActs[i] != null) { myCenter = myActs[i]; break; }
		}
		for (int i = 0; i < youActs.Length; i++)
		{
			if (youActs[i] != null) { youCenter = youActs[i]; break; }
		}

		// 步骤2：通过 CreateMyActsFromCenter / CreateYouActsFromCenter 生成按优先级排好 slot 的数组
		sortedMyActs = CreateMyActsFromCenter(myCenter);
		sortedYouActs = CreateYouActsFromCenter(youCenter);

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

			// 与自身先手值加减
			slotIndex += FightLandMyStandPet.Instance?.GetRoundPriorityIntervene() ?? 0;

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
			// 与自身先手值加减
			slotIndex += FightLandYouStandPet.Instance?.GetRoundPriorityIntervene() ?? 0;

			acts[slotIndex] = youCenterAction;
			GD.Print($"  └─ [FightExeAfter] 敌方行动={youCenterAction.ActionType} → 放入 Slot[{slotIndex}]");
		}
		return acts;
	}

	/// <summary>
	/// 执行技能的 RebuildTurn 鸭子方法，重构指定方（side）的 TurnAction 数组
	/// 委托 DuckSkillLoader.ExecuteRebuildTurn 调用技能实现类的 RebuildTurn 方法
	/// </summary>
	/// <param name="implCsFilePath">技能实现脚本路径（res://define/dataskill/.../DuckSkillXxx.cs）</param>
	/// <param name="myTurnActions">我方行动数组</param>
	/// <param name="youTurnActions">敌方行动数组</param>
	/// <param name="side">要返回并重构的行动方</param>
	/// <returns>重构后的 side 行动数组；脚本不存在/类找不到/异常时返回 null</returns>
	public static TurnAction[] ExecuteRebuildTurn(string implCsFilePath, TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
	{
		return DuckSkillLoader.ExecuteRebuildTurn(implCsFilePath, myTurnActions, youTurnActions, side);
	}

	/// <summary>
	/// 根据彼此的技能，再次调整行动顺序
	/// 分别拿出我方/敌方中心行动，若为技能行动（UseSkill），
	/// 则通过 ExecuteRebuildTurn 调用技能实现类的 RebuildTurn 鸭子方法，
	/// 让技能根据对方行动重构自己的 TurnAction[]，完成后重新赋值 sortedMy + sortedYou。
	/// 调用失败（脚本缺失/非技能行动/返回 null）时保持原数组不变。
	/// </summary>
	/// <param name="sortedMy">我方行动数组（引用，可能被技能重构替换）</param>
	/// <param name="sortedYou">敌方行动数组（引用，可能被技能重构替换）</param>
	public static void RebuildTurnBySkills(ref TurnAction[] sortedMy, ref TurnAction[] sortedYou)
	{
		// 提取双方中心行动
		TurnAction myCenter = GetCenterAction(sortedMy);
		TurnAction youCenter = GetCenterAction(sortedYou);

		// 我方技能重构
		if (myCenter?.ActionType == TurnActionType.UseSkill && myCenter.FightSkill?.Skill != null)
		{
			TurnAction[] rebuiltMy = ExecuteRebuildTurn(
				myCenter.FightSkill.ImplClass, sortedMy, sortedYou, EnumWho.My);
			if (rebuiltMy != null)
			{
				GD.Print($"  └─ [FightExeAfter] RebuildTurnBySkills | 我方技能【{myCenter.FightSkill.Skill.SkillName}】重构行动完成");
				sortedMy = rebuiltMy;
			}
		}

		// 敌方技能重构（此时 sortedMy 可能已被替换，传入最新值）
		if (youCenter?.ActionType == TurnActionType.UseSkill && youCenter.FightSkill?.Skill != null)
		{
			TurnAction[] rebuiltYou = ExecuteRebuildTurn(
				youCenter.FightSkill.ImplClass, sortedMy, sortedYou, EnumWho.You);
			if (rebuiltYou != null)
			{
				GD.Print($"  └─ [FightExeAfter] RebuildTurnBySkills | 敌方技能【{youCenter.FightSkill.Skill.SkillName}】重构行动完成");
				sortedYou = rebuiltYou;
			}
		}
	}

	/// <summary>
	/// 从行动数组中提取第一个非空行动（中心行动）
	/// </summary>
	private static TurnAction GetCenterAction(TurnAction[] actions)
	{
		if (actions == null) return null;
		for (int i = 0; i < actions.Length; i++)
		{
			if (actions[i] != null)
				return actions[i];
		}
		return null;
	}
}
