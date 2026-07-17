using Godot;
using System;
using System.Collections.Generic;

public static class GodotTool
{
	/// <summary>
	/// Get the first child of a specific type/interface from a parent node.
	/// </summary>
	public static T GetFirstChildOfType<T>(Node parent) where T : class
	{
		foreach (Node child in parent.GetChildren())
		{
			if (child is T typedChild)
			{
				return typedChild;
			}
		}
		return null;
	}

	/// <summary>
	/// Get all children of a specific type/interface from a parent node.
	/// </summary>
	public static List<T> GetChildrenOfType<T>(Node parent) where T : class
	{
		List<T> results = new List<T>();
		foreach (Node child in parent.GetChildren())
		{
			if (child is T typedChild)
			{
				results.Add(typedChild);
			}
		}
		return results;
	}

	/// <summary>
	/// 递归遍历所有子节点，根据节点名称匹配并返回第一个匹配的节点
	/// </summary>
	/// <param name="parent">父节点</param>
	/// <param name="nodeName">要查找的节点名称</param>
	/// <returns>匹配的节点，未找到返回 null</returns>
	public static Node FindChildByName(Node parent, string nodeName)
	{
		foreach (Node child in parent.GetChildren())
		{
			if (child.Name == nodeName)
				return child;

			Node found = FindChildByName(child, nodeName);
			if (found != null)
				return found;
		}
		return null;
	}

	/// <summary>
	/// 获取项目默认重力值 physics/2d/default_gravity
	/// </summary>
	public static float GetDefaultGravity()
	{
		return (float)ProjectSettings.GetSetting("physics/2d/default_gravity", 980f);
	}
}
