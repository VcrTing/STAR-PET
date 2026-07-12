using Godot;
using System.Collections.Generic;

/// <summary>
/// 世界全局状态
/// </summary>
public class InsWorldGlobalState
{
	public int GameTimeHour;       // 游戏内时间-时
	public int GameTimeMinute;     // 游戏内时间-分
	public int GameDay;            // 游戏内天数
	public string Weather = "晴天"; // 当前天气
	public string CurrentRegion;   // 当前地区ID
	public List<string> UnlockedRegions = new(); // 已解锁地区列表
}