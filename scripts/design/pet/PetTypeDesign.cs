using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵属性/系别配置，类宝可梦
/// </summary>
public static class PetTypeDesign
{

	// ---- 攻击克制关系（攻击方 → 被克制的防御系别） ----
	// 普通系(Normal)不克制任何系别，均1倍输出
	private static readonly Dictionary<int, List<int>> _superEffective = new()
	{
		{ (int)EnumPetType.Water,   new() { (int)EnumPetType.Fire, (int)EnumPetType.Earth } },
		{ (int)EnumPetType.Fire,    new() { (int)EnumPetType.Grass, (int)EnumPetType.Gold, (int)EnumPetType.Ice } },
		{ (int)EnumPetType.Grass,   new() { (int)EnumPetType.Water, (int)EnumPetType.Earth } },
		{ (int)EnumPetType.Electric, new() { (int)EnumPetType.Water, (int)EnumPetType.Ghost } },
		{ (int)EnumPetType.Earth,   new() { (int)EnumPetType.Electric, (int)EnumPetType.Gold, (int)EnumPetType.Fire } },
		{ (int)EnumPetType.Gold,    new() { (int)EnumPetType.Ice, (int)EnumPetType.Fairy } },
		{ (int)EnumPetType.Ice,     new() { (int)EnumPetType.Grass, (int)EnumPetType.Dragon, (int)EnumPetType.Fighting } },
		{ (int)EnumPetType.Fighting, new() { (int)EnumPetType.Ice, (int)EnumPetType.Earth, (int)EnumPetType.Gold } },
		{ (int)EnumPetType.Fairy,   new() { (int)EnumPetType.Dragon, (int)EnumPetType.Fighting } },
		{ (int)EnumPetType.Ghost,   new() { (int)EnumPetType.Fairy, (int)EnumPetType.Fighting } },
		{ (int)EnumPetType.Dragon,  new() { (int)EnumPetType.Ghost, (int)EnumPetType.Fighting } },
	};

	// ---- 防御抵抗关系（防御方抵抗的攻击系别） ----
	private static readonly Dictionary<int, List<int>> _resisted = new()
	{
		{ (int)EnumPetType.Water,   new() { (int)EnumPetType.Fire, (int)EnumPetType.Earth } },
		{ (int)EnumPetType.Fire,    new() { (int)EnumPetType.Grass, (int)EnumPetType.Ice } },
		{ (int)EnumPetType.Grass,   new() { (int)EnumPetType.Water, (int)EnumPetType.Earth, (int)EnumPetType.Electric } },
		{ (int)EnumPetType.Electric, new() { (int)EnumPetType.Electric } },
		{ (int)EnumPetType.Earth,   new() { (int)EnumPetType.Fire, (int)EnumPetType.Gold } },
		{ (int)EnumPetType.Gold,    new() { (int)EnumPetType.Grass, (int)EnumPetType.Ice, (int)EnumPetType.Electric, (int)EnumPetType.Fairy } },
		{ (int)EnumPetType.Ice,     new() { (int)EnumPetType.Water, (int)EnumPetType.Ice } },
		{ (int)EnumPetType.Fighting, new() { (int)EnumPetType.Earth, (int)EnumPetType.Gold } },
		{ (int)EnumPetType.Fairy,   new() { (int)EnumPetType.Fighting } },
		{ (int)EnumPetType.Ghost,   new() { (int)EnumPetType.Fairy, (int)EnumPetType.Normal } },  // 灵系抵抗普通系
		{ (int)EnumPetType.Dragon,  new() { (int)EnumPetType.Water, (int)EnumPetType.Grass, (int)EnumPetType.Ghost } },
	};

	// ---- 防御免疫关系（防御方免疫的攻击系别） ----
	private static readonly Dictionary<int, List<int>> _immune = new()
	{
		{ (int)EnumPetType.Earth, new() { (int)EnumPetType.Electric } },
		{ (int)EnumPetType.Fairy, new() { (int)EnumPetType.Dragon } },
		{ (int)EnumPetType.Ghost, new() { (int)EnumPetType.Fighting } },
	};

	/// <summary>
	/// 计算伤害系数（单攻击方系别 → 单防御方系别）
	/// </summary>
	/// <param name="attackType">攻击方系别</param>
	/// <param name="defendType">防御方系别</param>
	/// <returns>2.0=克制, 0.5=抵抗, 0.0=免疫, 1.0=普通</returns>
	public static float GetDamageMultiplier(int attackType, int defendType)
	{
		// 1. 免疫 → 0.0
		if (_immune.TryGetValue(defendType, out var immuneList) && immuneList.Contains(attackType))
		{
			return 0f;
		}

		// 2. 克制 → 2.0
		if (_superEffective.TryGetValue(attackType, out var strongList) && strongList.Contains(defendType))
		{
			return 2f;
		}

		// 3. 抵抗 → 0.5
		if (_resisted.TryGetValue(defendType, out var resistList) && resistList.Contains(attackType))
		{
			return 0.5f;
		}

		// 4. 普通 → 1.0
		return 1f;
	}

	/// <summary>
	/// 计算伤害系数（单攻击方系别 → 多防御方系别）
	/// 对每个防御方系别分别取得系数，按以下规则复合：
	///   - 双 0.5 → 0.25（乘积）
	///   - 双 2.0 → 3.0（上限，非 4）
	///   - 含免疫(0.0)时：取另一系数 × 0.5，最低 0.5
	///   - 其余情况：乘积
	/// </summary>
	/// <param name="attackType">攻击方系别</param>
	/// <param name="defendTypes">防御方系别数组（双属性时传入两个系别）</param>
	/// <returns>克制系数乘积（0.0 表示免疫）</returns>
	public static float GetDamageMultipliers(int attackType, IEnumerable<int> defendTypes)
	{
		if (defendTypes == null)
			return 1.0f;

		// 收集所有系数
		float m1 = 1.0f, m2 = 1.0f;
		int count = 0;
		foreach (int def in defendTypes)
		{
			float mul = GetDamageMultiplier(attackType, def);
			if (count == 0) m1 = mul;
			else if (count == 1) m2 = mul;
			count++;
		}

		if (count == 0) return 1.0f;
		if (count == 1) return m1;

		return CombineMultipliers(m1, m2);
	}

	/// <summary>
	/// 计算伤害系数（单攻击方系别 → 多防御方系别）
	/// 对每个防御方系别分别取得系数，按以下规则复合：
	///   - 双 0.5 → 0.25（乘积）
	///   - 双 2.0 → 3.0（上限，非 4）
	///   - 含免疫(0.0)时：取另一系数 × 0.5，最低 0.5
	///   - 其余情况：乘积
	/// </summary>
	/// <param name="attackType">攻击方系别（int 值）</param>
	/// <param name="defendTypes">防御方系别列表（如 List<EnumPetType>）</param>
	/// <returns>克制系数乘积（0.0 表示免疫）</returns>
	public static float GetDamageMultipliers(int attackType, List<EnumPetType> defendTypes)
	{
		if (defendTypes == null || defendTypes.Count == 0)
			return 1.0f;
		if (defendTypes.Count == 1)
			return GetDamageMultiplier(attackType, (int)defendTypes[0]);

		float m1 = GetDamageMultiplier(attackType, (int)defendTypes[0]);
		float m2 = GetDamageMultiplier(attackType, (int)defendTypes[1]);

		return CombineMultipliers(m1, m2);
	}

	/// <summary>
	/// 复合两个单属性系数为双属性结果
	/// </summary>
	private static float CombineMultipliers(float m1, float m2)
	{
		// 双 2.0 → 3.0（上限）
		if (m1 >= 2.0f && m2 >= 2.0f)
			return 3.0f;

		// 含免疫(0.0)：取另一系数 × 0.5，最低 0.5
		if (m1 <= 0.0f || m2 <= 0.0f)
		{
			float other = (m1 <= 0.0f) ? m2 : m1;
			return Math.Max(other * 0.5f, 0.5f);
		}

		// 其余情况：乘积
		return m1 * m2;
	}

	/// <summary>
	/// 获取系别中文名称
	/// </summary>
	public static string GetName(int type)
	{
		return type switch
		{
			(int)EnumPetType.Normal => "普通",
			(int)EnumPetType.Water => "水",
			(int)EnumPetType.Fire => "火",
			(int)EnumPetType.Grass => "草",
			(int)EnumPetType.Earth => "土",
			(int)EnumPetType.Gold => "金",
			(int)EnumPetType.Ice => "冰",
			(int)EnumPetType.Electric => "电",
			(int)EnumPetType.Fairy => "妖精",
			(int)EnumPetType.Dragon => "龙",
			(int)EnumPetType.Ghost => "灵",
			(int)EnumPetType.Fighting => "斗",
			_ => $"未知({type})"
		};
	}

	/// <summary>
	/// 根据 EnumPetType 获取对应的 datapet/ 子文件夹名
	/// 用于定位精灵数据文件：res://datapet/{folder}/pet_{id:0000}.gd
	/// </summary>
	public static string GetDataFolderPath(EnumPetType type)
	{
		return type switch
		{
			EnumPetType.Normal => "Normal",
			EnumPetType.Water => "Water",
			EnumPetType.Fire => "Fire",
			EnumPetType.Grass => "Grass",
			EnumPetType.Earth => "Earth",
			EnumPetType.Gold => "Gold",
			EnumPetType.Ice => "Ice",
			EnumPetType.Electric => "Electric",
			EnumPetType.Fairy => "Fairy",
			EnumPetType.Dragon => "Dragon",
			EnumPetType.Ghost => "Ghost",
			EnumPetType.Fighting => "Fighting",
			_ => "Unknown"
		};
	}

	/// <summary>
	/// 获取系别图标路径，如 res://IMG/uigame/pet_type/ui_pet_type_1.png
	/// </summary>
	public static string GetIconPath(int type)
	{
		return $"res://IMG/uigame/pet_type/ui_pet_type_{type}.png";
	}

	/// <summary>
	/// 加载系别图标纹理
	/// </summary>
	public static Texture2D GetIconTexture(int type)
	{
		return GD.Load<Texture2D>(GetIconPath(type));
	}
}