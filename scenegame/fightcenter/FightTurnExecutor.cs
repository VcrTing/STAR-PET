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
	/// <param name="enemyActs">敌方当前回合行动队列（TurnAction[9]）</param>
	/// <param name="myEnd">我方回合结束效果队列（TurnAction[4]）</param>
	/// <param name="enemyEnd">敌方回合结束效果队列（TurnAction[4]）</param>
	public static void ExecuteTurn(TurnAction[] myActs, TurnAction[] enemyActs, TurnAction[] myEnd, TurnAction[] enemyEnd)
	{
		// ── 步骤1：收集所有有效行动 ──
		var all = new List<TurnAction>();
		void Collect(TurnAction[] arr) { foreach (var a in arr) if (a?.IsValid == true) all.Add(a); }
		Collect(myActs);
		Collect(enemyActs);

		GD.Print($"    └─ [执行器] 共 {all.Count} 个行动");

		// ── 步骤2：按优先级→速度降序排序 ──
		//    Priority 高的先出手（技能先手值）
		//    同 Priority 时 Speed 高的先出手
		all.Sort((a, b) =>
		{
			int c = b.Priority.CompareTo(a.Priority);
			return c != 0 ? c : b.Speed.CompareTo(a.Speed);
		});

		// ── 步骤3：依次执行每个行动 ──
		for (int i = 0; i < all.Count; i++)
		{
			var a = all[i];
			string label = a.Side == "player" ? "🧑我方" : "👹敌方";
			GD.Print($"    └─ [执行器] #{i} ({label})...");
			switch (a.ActionType)
			{
				case TurnActionType.UseSkill:   ExecSkill(a); break;   // 使用技能
				case TurnActionType.SwitchPet:  ExecSwitch(a); break;  // 切换精灵
			}
		}

		// ── 步骤4：处理回合结束效果（中毒、灼伤、回复等） ──
		ProcessEnd(myEnd, "player");
		ProcessEnd(enemyEnd, "enemy");
		GD.Print($"    └─ [执行器] ✓");
	}

	// ───────────────────────────── 技能执行 ─────────────────────────

	/// <summary>
	/// 执行一次技能行动：计算伤害 → 扣血 → 回调通知
	/// </summary>
	/// <param name="act">行动实例（包含攻击者方、技能 ID）</param>
	private static void ExecSkill(TurnAction act)
	{
		// 安全检查
		if (act == null || string.IsNullOrEmpty(act.SkillId)) return;

		// 根据行动方确定攻击者和防御者
		var attacker = act.Side == "player" ? FightLandMyStandPet.Instance?.FightPetData : GetEnemy();
		var defender = act.Side == "player" ? GetEnemy() : FightLandMyStandPet.Instance?.FightPetData;
		if (attacker == null || defender == null) return;

		// 查找技能
		var skill = FindSkill(attacker, act.SkillId);
		if (skill == null) return;

		// 计算伤害
		int atk = StatVal(attacker.FinalStats, EnumPetBaseStats.ATK, 50);   // 攻击力
		int def = StatVal(defender.FinalStats, EnumPetBaseStats.DEF, 50);   // 防御力
		int baseP = skill.Skill?.AttackValue ?? 0;                          // 技能威力
		int dmg = Math.Max(1, Mathf.RoundToInt((atk * baseP) / (float)Math.Max(def, 1) * 0.4f));

		// 扣血（不低于 0）
		defender.Hp = Math.Max(0, defender.Hp - dmg);

		// 回调通知 FightCenterManger 发射信号
		string side = act.Side == "player" ? "enemy" : "player";
		OnDamageDealt?.Invoke(side, dmg, defender.Hp);

		GD.Print($"      → [技能] {attacker.PetName} 使用【{skill.Skill?.SkillName ?? act.SkillId}】 → 造成 {dmg} 伤害! ({defender.Hp}/{defender.MaxHp})");
	}

	// ───────────────────────────── 换宠执行 ─────────────────────────

	/// <summary>
	/// 执行换宠行动：切换到指定精灵上场
	/// 目前仅实现玩家方换宠，敌方换宠待 TODO
	/// </summary>
	private static void ExecSwitch(TurnAction act)
	{
		if (act.Side == "player")
		{
			var pets = PlayerLandMyStandPlayer.Instance.FightPets;
			if (act.SwitchTargetIndex < 0 || act.SwitchTargetIndex >= pets.Count || pets[act.SwitchTargetIndex].Hp <= 0) return;
			GD.Print($"      → [换宠] {pets[act.SwitchTargetIndex].PetName}");
			FightLandMyStandPet.Instance?.SwitchPet(pets[act.SwitchTargetIndex]);
		}
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
		int n = 0;
		for (int i = 0; i < endActs.Length; i++)
		{
			if (endActs[i]?.IsValid == true) { n++; endActs[i] = null; }
		}
		GD.Print($"      [{(side == "player" ? "🧑我方" : "👹敌方")}回合结束] {(n > 0 ? $"处理 {n} 个效果" : "无持续效果")}");
	}

	// ───────────────────────────── 内部工具 ─────────────────────────

	/// <summary>在精灵的战斗技能列表中按 ID 查找技能</summary>
	private static InsFightSkill FindSkill(InsFightPetData pet, string id)
	{
		if (pet?.FightSkills == null) return null;
		foreach (var f in pet.FightSkills) if (f.Skill?.SkillId == id) return f;
		return null;
	}

	/// <summary>从属性字典安全读取数值，不存在返回默认值</summary>
	private static int StatVal(Dictionary<EnumPetBaseStats, int> dict, EnumPetBaseStats key, int def)
	{
		if (dict != null && dict.TryGetValue(key, out int v)) return v;
		return def;
	}

	/// <summary>获取敌方当前上场精灵（TODO: 接入敌方管理器后实现）</summary>
	private static InsFightPetData GetEnemy() => null;
}