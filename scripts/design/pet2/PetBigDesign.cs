using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵体型档位设计
/// 分三档：小个体、普通个体、大个体
/// </summary>
public static class PetBigDesign
{
	public const int Small = (int)EnumPetBig.Small;   // 小个体
	public const int Normal = (int)EnumPetBig.Normal; // 普通个体
	public const int Large = (int)EnumPetBig.Large;   // 大个体

	/// <summary>
	/// 体型档位中文名称映射
	/// </summary>
	private static readonly Dictionary<int, string> _bigData = new()
	{
		{ (int)EnumPetBig.Small, "小个体" },
		{ (int)EnumPetBig.Normal, "普通个体" },
		{ (int)EnumPetBig.Large, "大个体" },
	};

	/// <summary>
	/// 获取体型档位的中文名称
	/// </summary>
	public static string GetBigName(int bigId)
	{
		if (_bigData.TryGetValue(bigId, out var name))
			return name;
		return $"Unknown({bigId})";
	}

	/// <summary>
	/// 获取所有体型档位 ID 列表
	/// </summary>
	public static List<int> GetAllBigIds()
	{
		return new List<int>(_bigData.Keys);
	}

	/// <summary>
	/// 无参随机返回体型档位，按概率：
	/// 小个体 20%，普通个体 40%，大个体 40%
	/// </summary>
	public static EnumPetBig GetRandomBig()
	{
		var rand = Random.Shared.NextDouble();
		return rand switch
		{
			< 0.2 => EnumPetBig.Small,
			< 0.6 => EnumPetBig.Normal,
			_     => EnumPetBig.Large,
		};
	}
}
