using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发技能配置库（Dev Load Skill House）
/// 维护技能标识符 → [系别, 技能类型] 的映射，用于构建技能加载路径
/// 按 EnumPetType 拆分 Dictionary，最后统一整合到 SkillConfig 中
/// </summary>
public static class DevLoadSkillHouse
{
	/// <summary>
	/// 系统技能配置
	/// </summary>
	private static readonly Dictionary<string, int[]> SystemSkills = new()
	{
		{ "0_4_1", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.SYSTEM } },   // 选择切换宠物
	};

	
	// ─── 各系别技能字典 ───

	/// <summary>
	/// 普通系（Normal）技能配置
	/// </summary>
	private static readonly Dictionary<string, int[]> NormalSkills = new()
	{
		// 攻击
		{ "0_1_1", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 拍击
		{ "0_1_2", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 先发制人
		{ "0_1_3", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 后发制人
		{ "0_1_4", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 夹击
		{ "0_1_5", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 冲撞
		{ "0_1_6", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 冲撞
		{ "0_1_7", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 乘胜追击
		{ "0_1_8", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 当头棒喝
		{ "0_1_9", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 气势一击
		// 防御
		{ "0_2_1", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.DEFENSE } },  // 防御
		// 状态
		{ "0_3_1", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.STATUS } },   // 聚能
		{ "0_3_2", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.STATUS } },   // 加固
		{ "0_3_3", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.STATUS } },   // 力量增效
		{ "0_3_4", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.STATUS } },   // 魔法增效
	};

	/// <summary>
	/// 冰系（Ice）技能配置
	/// </summary>
	private static readonly Dictionary<string, int[]> IceSkills = new()
	{
		// 攻击
		{ "6_1_1", new[] { (int)EnumPetType.Ice, (int)EnumSkillType.ATTACK } },   // 极度冰点
		{ "6_1_2", new[] { (int)EnumPetType.Ice, (int)EnumSkillType.ATTACK } },   // 冰心
	};


	// ─── 整合字典 ───

	/// <summary>
	/// 技能配置字典（由各系别字典合并而成）
	/// k = 技能标识符，格式 "{petType}_{skillType}_{skillCode}"，例如 "0_1_1"
	/// v = 数组 [0] = petType（系别，对应 EnumPetType）, [1] = skillType（技能类型，对应 EnumSkillType）
	/// </summary>
	private static readonly Dictionary<string, int[]> SkillConfig;

	static DevLoadSkillHouse()
	{
		SkillConfig = new Dictionary<string, int[]>();

		// 按需添加各系别技能
		MergeDict(NormalSkills);
		MergeDict(IceSkills);
		MergeDict(SystemSkills);

		// 后续扩充示例：
		// MergeDict(WaterSkills);
		// MergeDict(FireSkills);
		// ...
	}

	/// <summary>
	/// 将子字典合并到 SkillConfig 中
	/// </summary>
	private static void MergeDict(Dictionary<string, int[]> source)
	{
		foreach (var kv in source)
		{
			SkillConfig[kv.Key] = kv.Value;
		}
	}

	/// <summary>
	/// 根据技能标识符获取技能文件的完整路径
	/// </summary>
	/// <param name="skillId">技能标识符，格式 "{petType}_{skillType}_{skillCode}"，如 "0_1_1"</param>
	/// <returns>技能文件路径（如 res://dataskill/Normal/attack/0_1_1.gd），未找到则返回 null</returns>
	public static string GetSkillPath(string skillId)
	{
		if (!SkillConfig.TryGetValue(skillId, out var config))
		{
			GD.PrintErr($"[DevLoadSkillHouse] 未找到技能 {skillId} 的配置");
			return null;
		}

		int petType = config[0];
		int skillType = config[1];

		string folderPath = SkillTypeDesign.GetSkillFolderPath(petType, skillType);
		return $"{folderPath}{skillId}.gd";
	}
}