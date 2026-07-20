// ════════════════════════════════════════════════════════════════
//  战斗中心 - 通用工具方法
//  提供给 FightCenterManger 使用的静态辅助函数
// ════════════════════════════════════════════════════════════════

using Godot;
using System.Collections.Generic;

/// <summary>
/// 战斗中心通用工具方法（静态类）
/// 包含技能查找、队列打印、数组清空等辅助功能
/// </summary>
public static class FightCenterUtil
{
	// ───────────────────────────── 技能查找 ─────────────────────────

	/// <summary>
	/// 通过技能 ID 在当前上场精灵的战斗技能列表中查找
	/// </summary>
	/// <param name="skillId">技能 ID，如 "0_1_1"（格式：{petType}_{skillType}_{skillCode}）</param>
	/// <returns>找到的 InsFightSkill 实例，未找到返回 null</returns>
	public static InsFightSkill FindFightSkill(string skillId)
	{
		// 从 FightLandMyStandPet 获取当前上场精灵的战斗数据
		var activePet = FightLandMyStandPet.Instance?.FightPetData;
		if (activePet?.FightSkills == null)
			return null;

		// 遍历技能列表匹配 ID
		foreach (var fs in activePet.FightSkills)
		{
			if (fs.Skill?.SkillId == skillId)
				return fs;
		}
		return null;
	}

	// ───────────────────────────── 敌方获取 ─────────────────────────

	/// <summary>
	/// 获取敌方当前上场精灵
	/// TODO: 接入敌方管理器后替换为真实数据
	/// </summary>
	/// <returns>敌方上场精灵的战斗数据，暂无实现返回 null</returns>
	public static InsFightPetData GetEnemyActivePet()
	{
		return null;
	}

	// ───────────────────────────── 索引查找 ─────────────────────────

	/// <summary>
	/// 获取玩家当前上场精灵在 FightPets 列表中的索引
	/// 用于濒死信号 EmitSignal(SignalPetFainted, "player", index)
	/// </summary>
	/// <returns>索引值（0-based），未找到返回 -1</returns>
	public static int GetCurrentPlayerPetIndex()
	{
		var activePet = FightLandMyStandPet.Instance?.FightPetData;
		if (activePet == null)
			return -1;

		var pets = PlayerLandMyStandPlayer.Instance.FightPets;
		for (int i = 0; i < pets.Count; i++)
		{
			if (pets[i].PetUuid == activePet.PetUuid)
				return i;
		}
		return -1;
	}

	// ───────────────────────────── 调试打印 ─────────────────────────

	/// <summary>
	/// 打印行动队列状态（调试用）
	/// 在回合执行时输出双方本回合有哪些有效行动
	/// </summary>
	/// <param name="acts">行动数组（TurnAction[9]）</param>
	/// <param name="sideLabel">显示标签，如 "我方" / "敌方"</param>
	public static void PrintQueueStatus(TurnAction[] acts, string sideLabel)
	{
	}

	// ───────────────────────────── 数组管理 ─────────────────────────

	/// <summary>
	/// 清空行动数组的所有元素（全部置为 null）
	/// 在新回合开始或清空队列时调用
	/// </summary>
	/// <param name="arr">要清空的行动数组</param>
	public static void ClearActionQueue(TurnAction[] arr)
	{
		for (int i = 0; i < arr.Length; i++)
			arr[i] = null;
	}

	// ════════════════════════════════════════════════════════════════
	//  以下方法从 FightCenterManger 提取
	// ════════════════════════════════════════════════════════════════

	/// <summary>
	/// 从属性字典安全读取数值，不存在返回默认值
	/// </summary>
	/// <param name="dict">属性字典</param>
	/// <param name="key">属性键</param>
	/// <param name="def">默认值</param>
	/// <returns>属性值或默认值</returns>
	public static int StatOrDefault(Dictionary<EnumPetBaseStats, int> dict, EnumPetBaseStats key, int def)
	{
		if (dict != null && dict.TryGetValue(key, out int val)) return val;
		return def;
	}

	/// <summary>
	/// 获取战斗状态的中文名称
	/// </summary>
	/// <param name="s">战斗状态枚举值</param>
	/// <returns>中文状态名</returns>
	public static string StateName(FightState s) => s switch
	{
		FightState.BattleStart => "🎬 战斗开始",
		FightState.TurnStart => "🌅 回合开始",
		FightState.PlayerTurn => "🧑 玩家回合",
		FightState.EnemyTurn => "👹 敌方回合",
		FightState.ExecuteTurn => "⚔️ 回合执行",
		FightState.CheckFaint => "💀 濒死检查",
		FightState.PlayerSwitch => "🔄 玩家换宠",
		FightState.EnemySwitch => "🔄 敌方换宠",
		_ => "❓ " + s.ToString()
	};
}