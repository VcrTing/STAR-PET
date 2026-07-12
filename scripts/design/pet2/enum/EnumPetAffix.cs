using Godot;

/// <summary>
/// 宠物词条ID枚举
/// 词条是宠物拥有的特殊能力/特性，影响战斗、捕捉、采集、骑乘等
/// </summary>
public enum EnumPetAffix
{
	Surprise = 1,       // 奇袭
	Intimacy = 2,       // 亲密
	Dexterity = 3,      // 灵巧
	Swift = 4,          // 疾行
	RideTogether = 5,   // 同行/同乘
	Brave = 6,          // 勇敢
	Generous = 7,       // 爱分享
	Homebody = 8,       // 家里蹲
	Mentor = 9,         // 热心教
	Mercy = 10,         // 慈悲为怀/点到为止
}