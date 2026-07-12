using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 宠物词条设计，照搬洛克王国世界的功能型词条
/// 词条是宠物拥有的特殊能力/特性，影响战斗、捕捉、采集、骑乘等
/// </summary>
public static class PetAffixDesign
{
	// const int 常量已提取到 EnumPetAffix 枚举，见 scripts/design/pet2/enum/EnumPetAffix.cs
	public const int Surprise = (int)EnumPetAffix.Surprise;         // 奇袭
	public const int Intimacy = (int)EnumPetAffix.Intimacy;         // 亲密
	public const int Dexterity = (int)EnumPetAffix.Dexterity;       // 灵巧
	public const int Swift = (int)EnumPetAffix.Swift;               // 疾行
	public const int RideTogether = (int)EnumPetAffix.RideTogether; // 同行/同乘
	public const int Brave = (int)EnumPetAffix.Brave;               // 勇敢
	public const int Generous = (int)EnumPetAffix.Generous;         // 爱分享
	public const int Homebody = (int)EnumPetAffix.Homebody;         // 家里蹲
	public const int Mentor = (int)EnumPetAffix.Mentor;             // 热心教
	public const int Mercy = (int)EnumPetAffix.Mercy;               // 慈悲为怀/点到为止

	// ==================== 词条数据 ====================

	/// <summary>
	/// 所有词条的定义
	/// </summary>
	private static readonly Dictionary<int, AffixDef> _affixData = new()
	{
		{ (int)EnumPetAffix.Surprise, new AffixDef { Name = "奇袭", Description = "对战时，投掷精灵可使对手背对2回合。提高捕捉成功率。" } },
		{ (int)EnumPetAffix.Intimacy, new AffixDef { Name = "亲密", Description = "大幅度提升与精灵的亲密度。快速刷亲密度。" } },
		{ (int)EnumPetAffix.Dexterity, new AffixDef { Name = "灵巧", Description = "精灵的采集范围变大。跑图采集。" } },
		{ (int)EnumPetAffix.Swift, new AffixDef { Name = "疾行", Description = "骑乘该精灵时，移动速度变快。提升跑图效率。" } },
		{ (int)EnumPetAffix.RideTogether, new AffixDef { Name = "同行/同乘", Description = "可以和好友一起骑乘该精灵。双人跑图、社交。" } },
		{ (int)EnumPetAffix.Brave, new AffixDef { Name = "勇敢", Description = "对战时，同种族触发抵触环境加成。特定环境战斗。" } },
		{ (int)EnumPetAffix.Generous, new AffixDef { Name = "爱分享", Description = "采集时有概率获得额外的大量产物。资源收集，每只提升约15%产量。" } },
		{ (int)EnumPetAffix.Homebody, new AffixDef { Name = "家里蹲", Description = "放在家园里，可额外获得打造灵感和家园经验。家园挂机发育。" } },
		{ (int)EnumPetAffix.Mentor, new AffixDef { Name = "热心教", Description = "战斗和捕获中获得的经验显著增加。带低等级精灵升级。" } },
		{ (int)EnumPetAffix.Mercy, new AffixDef { Name = "慈悲为怀/点到为止", Description = "攻击精灵时，必定保留对方1点生命值，不会打死。抓宠神技。" } },
	};

	/// <summary>
	/// 获取词条名称
	/// </summary>
	public static string GetName(int affixId)
	{
		if (_affixData.TryGetValue(affixId, out var def))
			return def.Name;
		return $"未知词条({affixId})";
	}

	/// <summary>
	/// 获取词条描述
	/// </summary>
	public static string GetDescription(int affixId)
	{
		if (_affixData.TryGetValue(affixId, out var def))
			return def.Description;
		return "";
	}

	/// <summary>
	/// 获取词条完整展示文本
	/// </summary>
	public static string GetDisplayText(int affixId)
	{
		if (_affixData.TryGetValue(affixId, out var def))
			return $"{def.Name}：{def.Description}";
		return "";
	}

	/// <summary>
	/// 获取所有词条 ID 列表
	/// </summary>
	public static List<int> GetAllAffixIds()
	{
		return new List<int>(_affixData.Keys);
	}

	/// <summary>
	/// 词条内部定义结构
	/// </summary>
	private class AffixDef
	{
		public string Name { get; set; }
		public string Description { get; set; }
	}
}