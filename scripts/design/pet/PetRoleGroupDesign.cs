using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 宠物定位组设计
/// 宠物战斗定位/角色类型：速物攻、速魔攻、坦克、慢启动、增益辅助、清理辅助、低速物攻、低速魔攻、正常
/// </summary>
public static class PetRoleGroupDesign
{

	/// <summary>
	/// 定位组中文名称映射
	/// </summary>
	private static readonly Dictionary<int, string> _roleGroupData = new()
	{
		{ (int)EnumPetRoleGroup.FastPhysAtk, "速物攻" },
		{ (int)EnumPetRoleGroup.FastMagAtk, "速魔攻" },
		{ (int)EnumPetRoleGroup.Tank, "坦克" },
		{ (int)EnumPetRoleGroup.SlowStart, "慢启动" },
		{ (int)EnumPetRoleGroup.BuffSupport, "增益辅助" },
		{ (int)EnumPetRoleGroup.CleanSupport, "清理辅助" },
		{ (int)EnumPetRoleGroup.SlowPhysAtk, "低速物攻" },
		{ (int)EnumPetRoleGroup.SlowMagAtk, "低速魔攻" },
		{ (int)EnumPetRoleGroup.Normal, "正常" },
	};

	/// <summary>
	/// 获取定位组的中文名称
	/// </summary>
	public static string GetRoleGroupName(int roleGroupId)
	{
		if (_roleGroupData.TryGetValue(roleGroupId, out var name))
			return name;
		return $"Unknown({roleGroupId})";
	}

	/// <summary>
	/// 获取所有定位组 ID 列表
	/// </summary>
	public static List<int> GetAllRoleGroupIds()
	{
		return new List<int>(_roleGroupData.Keys);
	}
}