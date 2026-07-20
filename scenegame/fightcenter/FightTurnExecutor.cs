// ════════════════════════════════════════════════════════════════
//  回合执行器（静态类）
//  职责：负责排序并执行双方行动
//  不控制游戏进程，只做"执行"这一件事
//  由 FightCenterManger 在 ExecuteTurn 状态时委托调用
// ════════════════════════════════════════════════════════════════

using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 回合执行器（静态类）
/// 
/// 【执行流程】
///   1. 收集双方 turnActs 数组中的有效行动
///   2. 按 Priority(先手值) → Speed(速度) 降序排序
///   3. 遍历排序后的行动列表，依次执行
///   4. 处理双方 endActs（回合结束持续效果）
///
/// 【伤害公式】
///   damage = max(1, round((ATK × 技能威力 / DEF) × 0.4))
/// </summary>
public static class FightTurnExecutor
{
	// ───────────────────────────── 委托回调 ─────────────────────────

	/// <summary>
	/// 伤害回调（由 FightCenterManger 注入）
	/// FightCenterManger 在 _EnterTree 中赋值，用来发射 Godot Signal
	/// 参数依次为：(目标方, 伤害值, 剩余HP)
	/// </summary>
	public static Action<string, int, int> OnDamageDealt;

	// ───────────────────────────── 主入口 ────────────────────────────

	/// <summary>
	/// 执行当前回合所有行动（主入口）
	/// </summary>
	/// <param name="myActs">我方当前回合行动队列（TurnAction[9]）</param>
	/// <param name="youActs">敌方当前回合行动队列（TurnAction[9]）</param>
	/// <param name="myEnd">我方回合结束效果队列（TurnAction[4]）</param>
	/// <param name="youEnd">敌方回合结束效果队列（TurnAction[4]）</param>
	public static void ExecuteTurn(TurnAction[] myActs, TurnAction[] youActs, TurnAction[] myEnd, TurnAction[] youEnd)
	{
		
	}

	// ───────────────────────────── 技能执行 ─────────────────────────

	/// <summary>
	/// 执行一次技能行动：计算伤害 → 扣血 → 回调通知
	/// </summary>
	/// <param name="act">行动实例（包含攻击者方、技能 ID）</param>
	private static void ExecSkill(TurnAction act)
	{
		
	}

	// ───────────────────────────── 换宠执行 ─────────────────────────

	/// <summary>
	/// 执行换宠行动：切换到指定精灵上场
	/// 目前仅实现玩家方换宠，敌方换宠待 TODO
	/// </summary>
	private static void ExecSwitch(TurnAction act)
	{
		
		// TODO: 敌方换宠逻辑
	}

	// ───────────────────────────── 回合结束效果 ─────────────────────

	/// <summary>
	/// 处理一方回合结束后的持续效果（中毒扣血、灼伤、持续回复等）
	/// 遍历 endActs 数组，执行效果后清空
	/// TODO: 实现具体效果逻辑
	/// </summary>
	private static void ProcessEnd(TurnAction[] endActs, string side)
	{
	}

}