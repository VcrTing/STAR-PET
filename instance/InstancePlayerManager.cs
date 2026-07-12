using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 玩家单例管理器
/// 管理玩家的动态数据（基本信息、背包道具、任务进度、成就等）
/// 负责动态数据的存储、加载、序列化/反序列化
/// </summary>
public partial class InstancePlayerManager : Node
{
	#region 单例
	private static InstancePlayerManager _instance;
	public static InstancePlayerManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new InstancePlayerManager();
				_instance.Name = "InstancePlayerManager";
			}
			return _instance;
		}
	}

	/// <summary>
	/// 初始化单例并挂载到指定父节点
	/// </summary>
	public static void Init(Node parent)
	{
		if (_instance == null)
		{
			_instance = new InstancePlayerManager();
			_instance.Name = "InstancePlayerManager";
			parent.AddChild(_instance);
		}
	}
	#endregion

	private InsPlayerInfo _playerInfo = new();
	private List<InsItemData> _items = new();
	private List<InsQuestData> _quests = new();
	private InsPlayerStats _stats = new();

	#region 公开方法
	/// <summary>
	/// 获取玩家信息
	/// </summary>
	public InsPlayerInfo GetPlayerInfo() => _playerInfo;

	/// <summary>
	/// 获取道具列表
	/// </summary>
	public List<InsItemData> GetItems() => _items;

	/// <summary>
	/// 获取任务列表
	/// </summary>
	public List<InsQuestData> GetQuests() => _quests;

	/// <summary>
	/// 获取玩家统计数据
	/// </summary>
	public InsPlayerStats GetStats() => _stats;

	/// <summary>
	/// 获取指定道具数量
	/// </summary>
	public int GetItemCount(string itemId)
	{
		var item = _items.Find(i => i.ItemId == itemId);
		return item?.Count ?? 0;
	}

	/// <summary>
	/// 添加道具
	/// </summary>
	public void AddItem(string itemId, int count = 1)
	{
		var item = _items.Find(i => i.ItemId == itemId);
		if (item != null)
		{
			item.Count += count;
		}
		else
		{
			_items.Add(new InsItemData { ItemId = itemId, Count = count });
		}
	}

	/// <summary>
	/// 移除道具
	/// </summary>
	public bool RemoveItem(string itemId, int count = 1)
	{
		var item = _items.Find(i => i.ItemId == itemId);
		if (item == null || item.Count < count)
			return false;

		item.Count -= count;
		if (item.Count <= 0)
			_items.Remove(item);
		return true;
	}

	/// <summary>
	/// 增加金币
	/// </summary>
	public void AddGold(int amount) => _playerInfo.Gold += amount;

	/// <summary>
	/// 减少金币（不足返回false）
	/// </summary>
	public bool SpendGold(int amount)
	{
		if (_playerInfo.Gold < amount)
			return false;
		_playerInfo.Gold -= amount;
		return true;
	}

	/// <summary>
	/// 增加经验
	/// </summary>
	public void AddExp(int amount) => _playerInfo.Exp += amount;
	#endregion

	#region 存档
	/// <summary>
	/// 保存玩家数据到文件
	/// </summary>
	public void Save(string filePath = "user://player_data.save")
	{
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
		if (file == null)
		{
			GD.PrintErr("无法打开存档文件: " + filePath);
			return;
		}

		var data = new Godot.Collections.Dictionary
		{
			["player_name"] = _playerInfo.PlayerName,
			["level"] = _playerInfo.Level,
			["exp"] = _playerInfo.Exp,
			["gold"] = _playerInfo.Gold,
			["diamond"] = _playerInfo.Diamond,
			["badge_count"] = _playerInfo.BadgeCount,
			["badges"] = StringListToGodotArray(_playerInfo.Badges),
			["play_time"] = _playerInfo.PlayTimeSeconds,
			["items"] = SerializeItems(_items),
			["quests"] = SerializeQuests(_quests),
			["total_battles"] = _stats.TotalBattles,
			["total_wins"] = _stats.TotalWins,
			["total_captures"] = _stats.TotalCaptures,
			["total_defeated"] = _stats.TotalDefeated,
			["total_steps"] = _stats.TotalSteps
		};
		file.StoreString(Json.Stringify(data));
		GD.Print("玩家数据已保存");
	}

	/// <summary>
	/// 从文件加载玩家数据
	/// </summary>
	public void Load(string filePath = "user://player_data.save")
	{
		if (!FileAccess.FileExists(filePath))
		{
			GD.Print("存档文件不存在，使用默认玩家数据");
			return;
		}

		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			GD.PrintErr("无法读取存档文件: " + filePath);
			return;
		}

		string json = file.GetAsText();
		var data = Json.ParseString(json).AsGodotDictionary();

		_playerInfo.PlayerName = data["player_name"].AsString();
		_playerInfo.Level = (int)data["level"];
		_playerInfo.Exp = (int)data["exp"];
		_playerInfo.Gold = (int)data["gold"];
		_playerInfo.Diamond = (int)data["diamond"];
		_playerInfo.BadgeCount = (int)data["badge_count"];
		_playerInfo.Badges = GodotArrayToStringList(data["badges"].AsGodotArray());
		_playerInfo.PlayTimeSeconds = (int)data["play_time"];

		_items = DeserializeItems(data["items"].AsGodotArray());
		_quests = DeserializeQuests(data["quests"].AsGodotArray());

		_stats.TotalBattles = (int)data["total_battles"];
		_stats.TotalWins = (int)data["total_wins"];
		_stats.TotalCaptures = (int)data["total_captures"];
		_stats.TotalDefeated = (int)data["total_defeated"];
		_stats.TotalSteps = (int)data["total_steps"];

		GD.Print("玩家数据已加载");
	}

	private Godot.Collections.Array SerializeItems(List<InsItemData> items)
	{
		var arr = new Godot.Collections.Array();
		foreach (var item in items)
		{
			arr.Add(new Godot.Collections.Dictionary
			{
				["item_id"] = item.ItemId,
				["count"] = item.Count
			});
		}
		return arr;
	}

	private List<InsItemData> DeserializeItems(Godot.Collections.Array arr)
	{
		var list = new List<InsItemData>();
		foreach (var item in arr)
		{
			var dict = item.AsGodotDictionary();
			list.Add(new InsItemData
			{
				ItemId = dict["item_id"].AsString(),
				Count = (int)dict["count"]
			});
		}
		return list;
	}

	private Godot.Collections.Array SerializeQuests(List<InsQuestData> quests)
	{
		var arr = new Godot.Collections.Array();
		foreach (var q in quests)
		{
			arr.Add(new Godot.Collections.Dictionary
			{
				["quest_id"] = q.QuestId,
				["is_completed"] = q.IsCompleted,
				["progress"] = q.Progress,
				["target"] = q.Target
			});
		}
		return arr;
	}

	private List<InsQuestData> DeserializeQuests(Godot.Collections.Array arr)
	{
		var list = new List<InsQuestData>();
		foreach (var item in arr)
		{
			var dict = item.AsGodotDictionary();
			list.Add(new InsQuestData
			{
				QuestId = dict["quest_id"].AsString(),
				IsCompleted = (bool)dict["is_completed"],
				Progress = (int)dict["progress"],
				Target = (int)dict["target"]
			});
		}
		return list;
	}

	// ---- 序列化辅助 ----

	private static Godot.Collections.Array StringListToGodotArray(List<string> source)
	{
		var arr = new Godot.Collections.Array();
		foreach (var s in source)
			arr.Add(s);
		return arr;
	}

	private static List<string> GodotArrayToStringList(Godot.Collections.Array source)
	{
		var list = new List<string>();
		foreach (var item in source)
			list.Add(item.AsString());
		return list;
	}
	#endregion
}