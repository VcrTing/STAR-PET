using Godot;
using System.Collections.Generic;

/// <summary>
/// 背包数据
/// </summary>
public class InsPackData
{
	public List<InsPackPetData> Pets = new();     // 背包中的精灵列表
	public int MaxCapacity = 60;                  // 背包最大容量
}