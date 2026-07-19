using Godot;
using System;

/// <summary>
/// PVE 战斗执行器
/// 非 PVP 模式下，玩家选择技能后通过此类执行 PVE 逻辑
/// </summary>
public static class FightPveRunner
{
	/// <summary>
	/// 默认 PVE 敌方行动决策
	/// </summary>
	private static TurnAction DefaultEnemyAction(TurnAction playerAction)
	{
		int slotIndex = 0;
		if (playerAction?.FightSkill != null)
			slotIndex = playerAction.FightSkill.SlotIndex;

		GD.Print($"  └─ [敌方] AI思考... 模仿玩家选择 Slot[{slotIndex}] 技能");

		var enemySkills = FightLandYouStandPet.Instance?.FightPetData?.FightSkills;
		InsFightSkill enemySkill = null;
		if (enemySkills != null && slotIndex < enemySkills.Count)
			enemySkill = enemySkills[slotIndex];

		if (enemySkill != null)
		{
			GD.Print($"  └─ [敌方] 选择技能: {enemySkill.Skill?.SkillName ?? "未知"} (Slot={slotIndex})");
			return new TurnAction("enemy", enemySkill);
		}

		GD.Print($"  └─ [敌方] 无可用技能，执行无行动");
		return new TurnAction(TurnActionType.None, "enemy");
	}

	/// <summary>
	/// 执行 PVE 回合逻辑
	/// </summary>
	public static void RunPve(TurnAction playerAction)
	{
		GD.Print("  └─ [FightPveRunner] 执行 PVE 回合...");

		var mgr = FightCenterManger.Instance;
		if (mgr == null) return;

		mgr.EnemyTurnActs[4] = DefaultEnemyAction(playerAction);
		// PVE 结束
		mgr.SetPveActedAndExecute();
	}
}