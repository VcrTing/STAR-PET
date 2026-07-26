using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 宠物类别分组设计
/// 宠物外观/主题类别：虫、怪物、动物、萌物、抽象、高级
/// </summary>
public static class PetCategoryGroupDesign
{

	/// <summary>
	/// 类别分组中文名称映射
	/// </summary>
	private static readonly Dictionary<int, string> _categoryGroupData = new()
	{
		{ (int)EnumPetCategoryGroup.Bug, "虫" },
		{ (int)EnumPetCategoryGroup.Monster, "怪物" },
		{ (int)EnumPetCategoryGroup.Animal, "动物" },
		{ (int)EnumPetCategoryGroup.Cute, "萌物" },
		{ (int)EnumPetCategoryGroup.Abstract, "抽象" },
		{ (int)EnumPetCategoryGroup.Advanced, "高级" },
	};

	/// <summary>
	/// 获取类别分组的中文名称
	/// </summary>
	public static string GetCategoryGroupName(int categoryGroupId)
	{
		if (_categoryGroupData.TryGetValue(categoryGroupId, out var name))
			return name;
		return $"Unknown({categoryGroupId})";
	}

	/// <summary>
	/// 获取所有类别分组 ID 列表
	/// </summary>
	public static List<int> GetAllCategoryGroupIds()
	{
		return new List<int>(_categoryGroupData.Keys);
	}
}