using Godot;

/// <summary>
/// AiBoss大脑 - BOSS 级别敌方AI
/// 策略：优先使用高伤害技能，必要时使用战术技能
/// </summary>
public class AiBoss : IPveAiRunnerImpl
{
	public TurnAction GetAction(TurnAction playerAction)
	{
		int slotIndex = 0;
		if (playerAction?.FightSkill != null)
			slotIndex = playerAction.FightSkill.SlotIndex;

		GD.Print($"    └─ [AiBoss] BOSS思考... 分析玩家 Slot[{slotIndex}]");

		var enemySkills = FightLandYouStandPet.Instance?.FightPetData?.FightSkills;
		InsFightSkill enemySkill = null;

		// BOSS 策略：优先选择高优先级/高伤害技能
		if (enemySkills != null && enemySkills.Count > 0)
		{
			// 按优先级降序排列，选最高优先级技能
			enemySkills.Sort((a, b) => (b.Skill?.Priority ?? 0).CompareTo(a.Skill?.Priority ?? 0));
			enemySkill = enemySkills[0];
		}

		if (enemySkill != null)
		{
			GD.Print($"    └─ [AiBoss] BOSS 选择技能: {enemySkill.Skill?.SkillName ?? "未知"} (优先级={enemySkill.Skill?.Priority})");
			return new TurnAction("you", enemySkill);
		}

		GD.Print($"    └─ [AiBoss] BOSS 无可用技能，执行无行动");
		return new TurnAction(TurnActionType.Charge, "you");
	}
}