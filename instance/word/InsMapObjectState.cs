using Godot;
using System.Collections.Generic;

/// <summary>
/// 地图状态（可交互对象的状态变化）
/// </summary>
public class InsMapObjectState
{
	public string ObjectId;        // 地图对象ID
	public bool IsActive = true;   // 是否激活
	public bool IsCollected;       // 是否已被采集/拾取
	public Dictionary<string, string> CustomStates = new(); // 自定义状态键值对
}