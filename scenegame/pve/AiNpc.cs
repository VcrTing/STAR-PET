using Godot;
using System;

/// <summary>
/// AiNpc大脑 - NPC 敌方AI
/// 策略：随机选择技能，模拟 NPC 的不可预测性
/// </summary>
public class AiNpc : IPveAiRunnerImpl
{
	private readonly System.Random _random = new();

	public TurnAction GetAction(TurnAction playerAction)
	{
		int slotIndex = 0;
		if (playerAction?.FightSkill != null)
			slotIndex = playerAction.FightSkill.SlotIndex;

		GD.Print($"    └─ [AiNpc] NPC思考... 随机决策中(玩家Slot[{slotIndex}])");

		var enemySkills = FightLandYouStandPet.Instance?.FightPetData?.FightSkills;
		InsFightSkill enemySkill = null;

		// NPC 策略：随机选择技能
		if (enemySkills != null && enemySkills.Count > 0)
		{
			int randomIndex = _random.Next(0, enemySkills.Count);
			enemySkill = enemySkills[randomIndex];
		}

		if (enemySkill != null)
		{
			GD.Print($"    └─ [AiNpc] NPC 选择技能: {enemySkill.Skill?.SkillName ?? "未知"} (随机)");
			return new TurnAction(EnumWho.You, enemySkill);
		}

		GD.Print($"    └─ [AiNpc] NPC 无可用技能，执行无行动");
		return new TurnAction(TurnActionType.Charge, EnumWho.You);
	}
}