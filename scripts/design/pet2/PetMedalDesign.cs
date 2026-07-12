using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵奖牌设计，照搬洛克王国世界的奖牌/勋章系统
/// 奖牌分为五类：天分资质类、地域时间类、活动挑战类、战斗获取类、特殊类
/// 每个奖牌附带探索回顾倍率或特殊说明
/// </summary>
public static class PetMedalDesign
{
	// const int 常量已提取到 EnumPetMedal 枚举，见 scripts/design/pet2/enum/EnumPetMedal.cs

	// 天分资质类
	public const int Flawless = (int)EnumPetMedal.Flawless;         // 完美无瑕
	public const int Tiny = (int)EnumPetMedal.Tiny;                 // 小不点
	public const int Giant = (int)EnumPetMedal.Giant;               // 大块头
	public const int RoughVoice = (int)EnumPetMedal.RoughVoice;     // 粗嗓门
	public const int SweetVoice = (int)EnumPetMedal.SweetVoice;     // 婉转声

	// 地域时间类
	public const int WindBreath = (int)EnumPetMedal.WindBreath;           // 轻风之息
	public const int KingdomSong = (int)EnumPetMedal.KingdomSong;         // 王国领歌
	public const int DawnBreaker = (int)EnumPetMedal.DawnBreaker;         // 晨光破晓
	public const int SunBlaze = (int)EnumPetMedal.SunBlaze;               // 灼日凌空
	public const int DuskRider = (int)EnumPetMedal.DuskRider;             // 暮色衔山
	public const int Starlight = (int)EnumPetMedal.Starlight;             // 星月交辉

	// 活动挑战类
	public const int DestinedHero = (int)EnumPetMedal.DestinedHero;       // 命定勇者
	public const int HeartCompanion = (int)EnumPetMedal.HeartCompanion;   // 同心相伴
	public const int StarterSpark = (int)EnumPetMedal.StarterSpark;       // 御三家奖牌

	// 战斗获取类
	public const int LastStand = (int)EnumPetMedal.LastStand;             // 背水一战
	public const int Veteran = (int)EnumPetMedal.Veteran;                 // 经验老道
	public const int SameMaster = (int)EnumPetMedal.SameMaster;           // 同门高手
	public const int ResistMaster = (int)EnumPetMedal.ResistMaster;       // 抵抗大师
	public const int EffectiveMaster = (int)EnumPetMedal.EffectiveMaster; // 克制大师
	public const int DodgeMaster = (int)EnumPetMedal.DodgeMaster;         // 躲避大师
	public const int Pioneer = (int)EnumPetMedal.Pioneer;                 // 先驱者
	public const int Reaper = (int)EnumPetMedal.Reaper;                   // 收割者
	public const int Underdog = (int)EnumPetMedal.Underdog;               // 下克上
	public const int DeathDefying = (int)EnumPetMedal.DeathDefying;       // 命悬一线
	public const int Overwhelming = (int)EnumPetMedal.Overwhelming;       // 摧枯拉朽

	// 特殊类
	public const int EndlessLearning = (int)EnumPetMedal.EndlessLearning; // 学无止境
	public const int WalkTogether = (int)EnumPetMedal.WalkTogether;       // 结伴同行
	public const int BestFriend = (int)EnumPetMedal.BestFriend;           // 最好的伙伴
	public const int MoonGlow = (int)EnumPetMedal.MoonGlow;               // 皎皎明月
	public const int StarShine = (int)EnumPetMedal.StarShine;             // 熠熠星辉
	public const int ArthurCrown = (int)EnumPetMedal.ArthurCrown;         // 亚瑟的王冠
	public const int DuckFire = (int)EnumPetMedal.DuckFire;               // 燃了鸭
	public const int FadingBond = (int)EnumPetMedal.FadingBond;           // 不褪色的羁绊

	// 以下方法体保持不变：_medalData 字典的 key 仍为 int，GetName/GetCategory/等方法也保持 int 参数不变

	// ==================== 奖牌数据 ====================

	/// <summary>
	/// 奖牌定义：名称、类别、描述、探索回顾倍率
	/// </summary>
	private class MedalDef
	{
		public string Name;
		public string Category;
		public string Description;
		public float ReviewMultiplier;  // 探索回顾倍率，无特殊倍率则为 1.0
		public string ExtraInfo;        // 额外说明（如活动时间、称号等）
	}

	private static readonly Dictionary<int, MedalDef> _medalData = new()
	{
		// ---- 天分资质类 ----
		{ Flawless, new MedalDef { Name = "完美无瑕", Category = "天分资质类", Description = "精灵天分达到\"了不起\"，且六项个体值全满。", ReviewMultiplier = 3f } },
		{ Tiny, new MedalDef { Name = "小不点", Category = "天分资质类", Description = "捕捉到同种群体型最小的精灵。", ReviewMultiplier = 3f } },
		{ Giant, new MedalDef { Name = "大块头", Category = "天分资质类", Description = "捕捉到同种群体型最大的精灵。", ReviewMultiplier = 3f } },
		{ RoughVoice, new MedalDef { Name = "粗嗓门", Category = "天分资质类", Description = "种群内声音最低沉的精灵。", ReviewMultiplier = 3f } },
		{ SweetVoice, new MedalDef { Name = "婉转声", Category = "天分资质类", Description = "种群内声音最高昂的精灵。", ReviewMultiplier = 3f } },

		// ---- 地域时间类 ----
		{ WindBreath, new MedalDef { Name = "轻风之息", Category = "地域时间类", Description = "在风眼省捕捉或孵化精灵。", ReviewMultiplier = 2f } },
		{ KingdomSong, new MedalDef { Name = "王国领歌", Category = "地域时间类", Description = "在洛克里安省捕捉或孵化精灵。", ReviewMultiplier = 2f } },
		{ DawnBreaker, new MedalDef { Name = "晨光破晓", Category = "地域时间类", Description = "在清晨捕捉或孵化精灵。", ReviewMultiplier = 2f } },
		{ SunBlaze, new MedalDef { Name = "灼日凌空", Category = "地域时间类", Description = "在白天捕捉或孵化精灵。", ReviewMultiplier = 2f } },
		{ DuskRider, new MedalDef { Name = "暮色衔山", Category = "地域时间类", Description = "在傍晚捕捉或孵化精灵。", ReviewMultiplier = 2f } },
		{ Starlight, new MedalDef { Name = "星月交辉", Category = "地域时间类", Description = "在夜晚捕捉或孵化精灵。", ReviewMultiplier = 2f } },

		// ---- 活动挑战类 ----
		{ DestinedHero, new MedalDef { Name = "命定勇者", Category = "活动挑战类", Description = "单人、仅用一只精灵、全程不换宠且不败，击败\"命定花种\"活动BOSS。需魔法等级40级并晋升\"精灵学士\"。", ReviewMultiplier = 1f, ExtraInfo = "周期性轮换活动" } },
		{ HeartCompanion, new MedalDef { Name = "同心相伴", Category = "活动挑战类", Description = "在\"为相伴加冕\"活动中，选择一只满亲密度的精灵接受祝福。需魔法等级30级并晋升\"精灵研究员\"。", ReviewMultiplier = 1f, ExtraInfo = "S2赛季限时活动（2026.05.21-07.16）" } },
		{ StarterSpark, new MedalDef { Name = "御三家奖牌", Category = "活动挑战类", Description = "参与\"不褪色的羁绊\"活动。老玩家对应页游首宠，新玩家随机获得一款。", ReviewMultiplier = 1f, ExtraInfo = "称号：我有我的三小只" } },

		// ---- 战斗获取类 ----
		{ LastStand, new MedalDef { Name = "背水一战", Category = "战斗获取类", Description = "双方魔力剩1时，使用逆属性技能击败对手。" } },
		{ Veteran, new MedalDef { Name = "经验老道", Category = "战斗获取类", Description = "总计使用该精灵击败20种不同精灵。" } },
		{ SameMaster, new MedalDef { Name = "同门高手", Category = "战斗获取类", Description = "PVP中击败相同精灵。" } },
		{ ResistMaster, new MedalDef { Name = "抵抗大师", Category = "战斗获取类", Description = "PVP中使用该精灵连续三次抵抗敌方技能伤害。" } },
		{ EffectiveMaster, new MedalDef { Name = "克制大师", Category = "战斗获取类", Description = "PVP中使用该精灵连续三次造成克制伤害。" } },
		{ DodgeMaster, new MedalDef { Name = "躲避大师", Category = "战斗获取类", Description = "PVP中使用该精灵连续三次让对方技能未应对成功。" } },
		{ Pioneer, new MedalDef { Name = "先驱者", Category = "战斗获取类", Description = "首回合击败对手。" } },
		{ Reaper, new MedalDef { Name = "收割者", Category = "战斗获取类", Description = "剩1魔力时，击败4只精灵。" } },
		{ Underdog, new MedalDef { Name = "下克上", Category = "战斗获取类", Description = "击败比自己高级的精灵。" } },
		{ DeathDefying, new MedalDef { Name = "命悬一线", Category = "战斗获取类", Description = "战斗胜利时，血量10%以下，能量清空。" } },
		{ Overwhelming, new MedalDef { Name = "摧枯拉朽", Category = "战斗获取类", Description = "该精灵造成的伤害超过80%的精灵。" } },

		// ---- 特殊类 ----
		{ EndlessLearning, new MedalDef { Name = "学无止境", Category = "特殊类", Description = "通过使用技能石，让精灵学习10个技能。" } },
		{ WalkTogether, new MedalDef { Name = "结伴同行", Category = "特殊类", Description = "与精灵达到满好感度获得。" } },
		{ BestFriend, new MedalDef { Name = "最好的伙伴", Category = "特殊类", Description = "与迪莫重逢获得。" } },
		{ MoonGlow, new MedalDef { Name = "皎皎明月", Category = "特殊类", Description = "疾光千兽15级通行证获得。" } },
		{ StarShine, new MedalDef { Name = "熠熠星辉", Category = "特殊类", Description = "绒仙子15级通行证获得。" } },
		{ ArthurCrown, new MedalDef { Name = "亚瑟的王冠", Category = "特殊类", Description = "通过任务获得。" } },
		{ DuckFire, new MedalDef { Name = "燃了鸭", Category = "特殊类", Description = "通过预约获得。" } },
		{ FadingBond, new MedalDef { Name = "不褪色的羁绊", Category = "特殊类", Description = "参与\"不褪色的羁绊\"活动获得。" } },
	};

	/// <summary>
	/// 获取奖牌名称
	/// </summary>
	public static string GetName(int medalId)
	{
		if (_medalData.TryGetValue(medalId, out var def))
			return def.Name;
		return $"未知奖牌({medalId})";
	}

	/// <summary>
	/// 获取奖牌类别
	/// </summary>
	public static string GetCategory(int medalId)
	{
		if (_medalData.TryGetValue(medalId, out var def))
			return def.Category;
		return "";
	}

	/// <summary>
	/// 获取奖牌描述
	/// </summary>
	public static string GetDescription(int medalId)
	{
		if (_medalData.TryGetValue(medalId, out var def))
			return def.Description;
		return "";
	}

	/// <summary>
	/// 获取探索回顾倍率
	/// </summary>
	public static float GetReviewMultiplier(int medalId)
	{
		if (_medalData.TryGetValue(medalId, out var def))
			return def.ReviewMultiplier;
		return 1f;
	}

	/// <summary>
	/// 获取额外信息
	/// </summary>
	public static string GetExtraInfo(int medalId)
	{
		if (_medalData.TryGetValue(medalId, out var def))
			return def.ExtraInfo ?? "";
		return "";
	}

	/// <summary>
	/// 获取奖牌完整展示文本
	/// </summary>
	public static string GetDisplayText(int medalId)
	{
		if (!_medalData.TryGetValue(medalId, out var def))
			return "";

		string text = $"【{def.Category}】{def.Name}\n{def.Description}";
		if (def.ReviewMultiplier > 1f)
			text += $"\n探索回顾倍率 ×{def.ReviewMultiplier}";
		if (!string.IsNullOrEmpty(def.ExtraInfo))
			text += $"\n{def.ExtraInfo}";
		return text;
	}

	/// <summary>
	/// 获取所有奖牌 ID 列表
	/// </summary>
	public static List<int> GetAllMedalIds()
	{
		return new List<int>(_medalData.Keys);
	}

	/// <summary>
	/// 按类别获取奖牌 ID 列表
	/// </summary>
	public static List<int> GetMedalIdsByCategory(int medalId)
	{
		if (!_medalData.TryGetValue(medalId, out var def))
			return new List<int>();

		string category = def.Category;
		var result = new List<int>();
		foreach (var kvp in _medalData)
		{
			if (kvp.Value.Category == category)
				result.Add(kvp.Key);
		}
		return result;
	}
}