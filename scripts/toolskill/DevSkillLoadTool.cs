using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发用技能加载工具
/// 根据技能ID数组，从技能配置中获取路径并加载技能数据
/// </summary>
public static class DevSkillLoadTool
{
	/// <summary>
	/// 根据技能标识符数组加载技能
	/// </summary>
	/// <param name="skillIds">技能标识符数组，格式 "{petType}_{skillType}_{skillCode}"，如 ["0_1_1", "0_1_2"]</param>
	/// <returns>加载完成的 InsSkill 列表</returns>
	public static List<InsSkill> LoadSkills(string[] skillIds)
	{
		var result = new List<InsSkill>();

		if (skillIds == null || skillIds.Length == 0)
			return result;

		foreach (string skillId in skillIds)
		{
			string path = DevSkillConf.GetSkillPath(skillId);
			if (string.IsNullOrEmpty(path))
			{
				GD.PrintErr($"[DevSkillLoadTool] 技能 {skillId} 路径获取失败");
				continue;
			}

			if (!ResourceLoader.Exists(path))
			{
				GD.PrintErr($"[DevSkillLoadTool] 技能文件不存在: {path}");
				continue;
			}

			// .gd 文件是 extends Resource 的 GDScript 脚本，需加载脚本后实例化
			var gdScript = GD.Load<GDScript>(path);
			if (gdScript == null)
			{
				GD.PrintErr($"[DevSkillLoadTool] 技能脚本加载失败: {path}");
				continue;
			}

			var res = (Resource)gdScript.New();
			if (res == null)
			{
				GD.PrintErr($"[DevSkillLoadTool] 技能脚本实例化失败: {path}");
				continue;
			}

			var skill = InsSkill.FromResource(res);
			if (skill != null)
			{
				result.Add(skill);
			}
		}

		return result;
	}
}