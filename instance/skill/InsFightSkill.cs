using Godot;
using System;

/// <summary>
/// 战斗中的技能实例数据
/// 包装 InsSkill 并附加战斗运行时状态
/// </summary>
public class InsFightSkill
{
	/// <summary>
	/// 技能基础数据
	/// </summary>
	public InsSkill Skill { get; private set; }

	/// <summary>
	/// 技能槽位索引（0=第一个技能，1=第二个，依此类推）
	/// </summary>
	public int SlotIndex { get; set; }

	/// <summary>
	/// 实际能耗（战斗修正后的PP消耗）
	/// </summary>
	public int ActualPpCost { get; set; }

	/// <summary>
	/// 实际威力（战斗修正后的攻击数值）
	/// </summary>
	public int ActualAttackValue { get; set; }

	/// <summary>
	/// 显示威力（UI展示用的威力值，可能受隐藏效果影响）
	/// </summary>
	public int DisplayAttackValue { get; set; }

	/// <summary>
	/// 是否冻结（冻结状态下该技能不可使用）
	/// </summary>
	public bool IsFrozen { get; set; }

	/// <summary>
	/// 冷却回合数（0=无冷却，>0=还需等待的回合数）
	/// </summary>
	public int CooldownTurns { get; set; }

	/// <summary>
	/// 实际系别（默认取 InsSkill.PetType，战斗中可被特性/道具改变）
	/// </summary>
	public int ActualPetType { get; set; }

	/// <summary>
	/// 判断技能是否为使用者的本系技能（STAB）
	/// 即技能的 ActualPetType 是否存在于 PetData 的 PetTypes 中
	/// </summary>
	/// <param name="petData">使用该技能的宠物数据</param>
	/// <returns>true=本系技能</returns>
	public bool IsSameType(InsFightPetData petData)
	{
		if (petData?.PetTypes == null)
			return false;

		return petData.PetTypes.Contains((EnumPetType)ActualPetType);
	}

	/// <summary>
	/// 从 InsSkill 创建战斗技能实例
	/// </summary>
	/// <param name="skill">技能基础数据</param>
	/// <param name="slotIndex">技能槽位索引</param>
	/// <returns>战斗技能实例</returns>
	public static InsFightSkill FromInsSkill(InsSkill skill, int slotIndex = 0)
	{
		if (skill == null)
			return null;

		return new InsFightSkill
		{
			Skill = skill,
			SlotIndex = slotIndex,
			ActualPpCost = skill.PpCost,
			ActualAttackValue = skill.AttackValue,
			DisplayAttackValue = skill.AttackValue,
			IsFrozen = false,
			CooldownTurns = 0,
			ActualPetType = skill.PetType,
		};
	}
}