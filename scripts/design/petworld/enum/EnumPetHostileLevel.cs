using Godot;

/// <summary>
/// 精灵对人类攻击等级（野外遭遇行为）
/// </summary>
public enum EnumPetHostileLevel
{
	Flee = -1,      // 逃避（见人就跑）
	None = 0,       // 不攻击（完全无视玩家）
	Passive = 1,    // 不主动攻击（被攻击后反击）
	Aggressive = 2, // 近处攻击（靠近一定范围主动攻击）
}