using Godot;

/// <summary>
/// PVE 模式枚举
/// </summary>
public enum PveMode
{
	WildMonster,  // 野怪
	Npc,          // NPC
	Elite,        // 精英
	Boss,         // BOSS
}

/// <summary>
/// PVE 战斗执行器
/// 非 PVP 模式下，玩家选择技能后通过此类执行 PVE 逻辑
/// 根据模式切换使用不同的 AI 大脑
/// </summary>
public static class FightPveRunner
{
	// ─── 4 个 AI 大脑实例 ───
	private static readonly IPveAiRunnerImpl AiBoss = new AiBoss();
	private static readonly IPveAiRunnerImpl AiWildMonster = new AiWildMonster();
	private static readonly IPveAiRunnerImpl AiNpc = new AiNpc();
	private static readonly IPveAiRunnerImpl AiElite = new AiElite();

	// ─── 当前模式（默认野怪） ───
	private static PveMode _currentMode = PveMode.WildMonster;

	/// <summary>
	/// 设置当前 PVE 模式
	/// </summary>
	public static void SetMode(PveMode mode)
	{
		_currentMode = mode;
		GD.Print($"  └─ [FightPveRunner] 切换为 {mode} 模式");
	}

	/// <summary>
	/// 根据当前模式获取对应的 AI 大脑
	/// </summary>
	private static IPveAiRunnerImpl GetCurrentAi()
	{
		return _currentMode switch
		{
			PveMode.Boss => AiBoss,
			PveMode.Npc => AiNpc,
			PveMode.Elite => AiElite,
			PveMode.WildMonster => AiWildMonster,
			_ => AiWildMonster,
		};
	}

	/// <summary>
	/// 执行 PVE 回合逻辑
	/// 使用当前模式对应的 AI 大脑决策
	/// </summary>
	public static void RunPve(TurnAction playerAction)
	{
		GD.Print($"  └─ [FightPveRunner] 执行 PVE 回合... (模式={_currentMode})");

		var mgr = FightCenterManger.Instance;
		if (mgr == null) return;

		// 使用当前模式对应的 AI 大脑获取行动
		var ai = GetCurrentAi();
		mgr.YouTurnActs[4] = ai.GetAction(playerAction);

		// PVE 结束
		mgr.SetPveActedAndExecute();
	}

	/// <summary>
	/// PVE 换宠逻辑：宠物死亡后，调用 AI 获取下一只上场的宠物
	/// </summary>
	/// <returns>下一只上场的精灵，无可用精灵返回 null</returns>
	public static InsFightPetData RunPveWhenPreDie()
	{
		GD.Print($"  └─ [FightPveRunner] 执行 PVE 换宠... (模式={_currentMode})");

		// 使用当前模式对应的 AI 大脑获取下一只宠物
		var ai = GetCurrentAi();
		return ai.GetNextPetWhenPreDie();
	}
}