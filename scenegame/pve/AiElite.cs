using Godot;
using System;

/// <summary>
/// AiElite大脑 - 精英怪敌方AI
/// 策略：70%概率选择与玩家同slot技能，30%随机选择
/// </summary>
public class AiElite : IPveAiRunnerImpl
{
	private readonly System.Random _random = new();

	public TurnAction GetAction(TurnAction playerAction)
	{
		int slotIndex = 0;
		InsFightSkill playerSkill = playerAction?.FightSkill;
		if (playerSkill != null)
			slotIndex = playerSkill.SlotIndex;

		GD.Print($"    └─ [AiElite] 精英怪思考... 分析玩家 Slot[{slotIndex}]");

		var enemySkills = FightLandYouStandPet.Instance?.FightPetData?.FightSkills;
		InsFightSkill enemySkill = null;

		if (enemySkills != null && enemySkills.Count > 0)
		{
			// 精英策略：优先选择与玩家同 slot 的技能，否则随机
			if (slotIndex < enemySkills.Count && _random.NextDouble() > 0.3)
			{
				enemySkill = enemySkills[slotIndex];
			}
			else
			{
				int randomIndex = _random.Next(0, enemySkills.Count);
				enemySkill = enemySkills[randomIndex];
			}
		}

		if (enemySkill != null)
		{
			GD.Print($"    └─ [AiElite] 精英怪 选择技能: {enemySkill.Skill?.SkillName ?? "未知"} (战术决策)");
			return new TurnAction(EnumWho.You, enemySkill);
		}

		GD.Print($"    └─ [AiElite] 精英怪 无可用技能，执行无行动");
		return new TurnAction(TurnActionType.Charge, EnumWho.You);
	}
}