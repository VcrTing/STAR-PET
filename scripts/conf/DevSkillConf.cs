using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用技能配置
/// 维护技能标识符 → [系别, 技能类型] 的映射，用于构建技能加载路径
/// </summary>
public static class DevSkillConf
{
	/// <summary>
	/// 技能配置字典
	/// k = 技能标识符，格式 "{petType}_{skillType}_{skillCode}"，例如 "0_1_1"
	/// v = 数组 [0] = petType（系别，对应 EnumPetType）, [1] = skillType（技能类型，对应 EnumSkillType）
	/// </summary>
	private static readonly Dictionary<string, int[]> SkillConfig = new()
	{
		// 普通系 - 攻击
		{ "0_1_1", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 拍击
		{ "0_1_2", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 先发制人
		{ "0_1_3", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.ATTACK } },   // 后发制人
		// 普通系 - 防御
		{ "0_2_1", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.DEFENSE } },  // 防御
		// 普通系 - 状态
		{ "0_3_1", new[] { (int)EnumPetType.Normal, (int)EnumSkillType.STATUS } },   // 聚能
	};

	/// <summary>
	/// 根据技能标识符获取技能文件的完整路径
	/// </summary>
	/// <param name="skillId">技能标识符，格式 "{petType}_{skillType}_{skillCode}"，如 "0_1_1"</param>
	/// <returns>技能文件路径（如 res://dataskill/Normal/attack/0_1_1.gd），未找到则返回 null</returns>
	public static string GetSkillPath(string skillId)
	{
		if (!SkillConfig.TryGetValue(skillId, out var config))
		{
			GD.PrintErr($"[DevSkillConf] 未找到技能 {skillId} 的配置");
			return null;
		}

		int petType = config[0];
		int skillType = config[1];

		string folderPath = SkillTypeDesign.GetSkillFolderPath(petType, skillType);
		return $"{folderPath}{skillId}.gd";
	}
}
