using Godot;

/// <summary>
/// AiWildMonster大脑 - 野怪敌方AI
/// 策略：模仿玩家选择相同槽位的技能（基础行为）
/// </summary>
public class AiWildMonster : IPveAiRunnerImpl
{
	InsFightSkill GetSkill(int slotIndex)
	{
		var enemySkills = FightLandYouStandPet.Instance?.FightPetData?.FightSkills;
		InsFightSkill enemySkill = null;
		if (enemySkills != null && slotIndex < enemySkills.Count)
			enemySkill = enemySkills[slotIndex];
		if (enemySkill == null)
		{
			enemySkill = DevFightSkillLoadTool.LoadChargeSkill();
		}
		return enemySkill;
	}

	public TurnAction GetAction(TurnAction playerAction)
	{
		int slotIndex = 0;
		if (playerAction?.FightSkill != null)
			slotIndex = playerAction.FightSkill.SlotIndex;

		InsFightSkill enemySkill = GetSkill(slotIndex);
		if (enemySkill != null)
		{
			GD.Print($"    └─ [AiWildMonster] 野怪 选择技能: {enemySkill.Skill?.SkillName ?? "未知"} (Slot={slotIndex})");
			return new TurnAction("you", enemySkill);
		}

		GD.Print($"    └─ [AiWildMonster] 野怪 无可用技能，执行无行动");
		return new TurnAction(TurnActionType.Charge, "you");
	}
}