using Godot;
using System.Collections.Generic;

/// <summary>
/// 玩家基本信息
/// </summary>
public class InsPlayerInfo
{
	public string PlayerName = "训练家";         // 玩家名称
	public int Level = 1;                         // 训练家等级
	public int Exp;                               // 当前经验
	public int Gold;                              // 金币
	public int Diamond;                           // 钻石/点券
	public int BadgeCount;                        // 徽章数量
	public List<string> Badges = new();           // 已获得的徽章ID列表
	public int PlayTimeSeconds;                   // 总游戏时长（秒）
}