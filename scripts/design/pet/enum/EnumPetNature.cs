using Godot;

/// <summary>
/// 精灵性格枚举（25种，完全照搬宝可梦）
/// </summary>
public enum EnumPetNature
{
	Hardy = 1,     // 勤奋 — 无影响
	Lonely = 2,    // 孤独 — +物攻 -物防
	Brave = 3,     // 勇敢 — +物攻 -速度
	Adamant = 4,   // 固执 — +物攻 -魔攻
	Naughty = 5,   // 调皮 — +物攻 -魔防
	Bold = 6,      // 大胆 — +物防 -物攻
	Docile = 7,    // 坦率 — 无影响
	Relaxed = 8,   // 悠闲 — +物防 -速度
	Impish = 9,    // 淘气 — +物防 -魔攻
	Lax = 10,      // 乐天 — +物防 -魔防
	Timid = 11,    // 胆小 — +速度 -物攻
	Hasty = 12,    // 急躁 — +速度 -物防
	Serious = 13,  // 认真 — 无影响
	Jolly = 14,    // 爽朗 — +速度 -魔攻
	Naive = 15,    // 天真 — +速度 -魔防
	Modest = 16,   // 内敛 — +魔攻 -物攻
	Mild = 17,     // 冷静 — +魔攻 -物防
	Quiet = 18,    // 慢吞吞 — +魔攻 -速度
	Rash = 19,     // 马虎 — +魔攻 -魔防
	Calm = 20,     // 温和 — +魔防 -物攻
	Gentle = 21,   // 温顺 — +魔防 -物防
	Careful = 22,  // 慎重 — +魔防 -魔攻
	Quirky = 23,   // 浮躁 — 无影响
	Sassy = 24,    // 自大 — +魔防 -速度
	Bashful = 25,  // 害羞 — 无影响
}