using Godot;

/// <summary>
/// 精灵奖牌ID枚举
/// 奖牌分为五类：天分资质类、地域时间类、活动挑战类、战斗获取类、特殊类
/// </summary>
public enum EnumPetMedal
{
	// 天分资质类
	Flawless = 1,         // 完美无瑕
	Tiny = 2,             // 小不点
	Giant = 3,            // 大块头
	RoughVoice = 4,       // 粗嗓门
	SweetVoice = 5,       // 婉转声

	// 地域时间类
	WindBreath = 6,       // 轻风之息
	KingdomSong = 7,      // 王国领歌
	DawnBreaker = 8,      // 晨光破晓
	SunBlaze = 9,         // 灼日凌空
	DuskRider = 10,       // 暮色衔山
	Starlight = 11,       // 星月交辉

	// 活动挑战类
	DestinedHero = 12,    // 命定勇者
	HeartCompanion = 13,  // 同心相伴
	StarterSpark = 14,    // 御三家奖牌

	// 战斗获取类
	LastStand = 15,       // 背水一战
	Veteran = 16,         // 经验老道
	SameMaster = 17,      // 同门高手
	ResistMaster = 18,    // 抵抗大师
	EffectiveMaster = 19, // 克制大师
	DodgeMaster = 20,     // 躲避大师
	Pioneer = 21,         // 先驱者
	Reaper = 22,          // 收割者
	Underdog = 23,        // 下克上
	DeathDefying = 24,    // 命悬一线
	Overwhelming = 25,    // 摧枯拉朽

	// 特殊类
	EndlessLearning = 26, // 学无止境
	WalkTogether = 27,    // 结伴同行
	BestFriend = 28,      // 最好的伙伴
	MoonGlow = 29,        // 皎皎明月
	StarShine = 30,       // 熠熠星辉
	ArthurCrown = 31,     // 亚瑟的王冠
	DuckFire = 32,        // 燃了鸭
	FadingBond = 33,      // 不褪色的羁绊
}