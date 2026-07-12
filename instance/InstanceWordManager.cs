using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 世界单例管理器
/// 管理游戏世界/场景的动态数据（地图状态、NPC状态、场景切换、天气、时间等）
/// 负责动态数据的存储、加载、序列化/反序列化
/// </summary>
public partial class InstanceWordManager : Node
{
	#region 单例
	private static InstanceWordManager _instance;
	public static InstanceWordManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new InstanceWordManager();
				_instance.Name = "InstanceWordManager";
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
			_instance = new InstanceWordManager();
			_instance.Name = "InstanceWordManager";
			parent.AddChild(_instance);
		}
	}
	#endregion

	private InsPlayerPosition _playerPosition = new();
	private List<InsMapObjectState> _mapObjects = new();
	private List<InsNpcState> _npcs = new();
	private InsWorldGlobalState _worldState = new();

	#region 公开方法
	/// <summary>
	/// 获取玩家位置
	/// </summary>
	public InsPlayerPosition GetPlayerPosition() => _playerPosition;

	/// <summary>
	/// 设置玩家位置
	/// </summary>
	public void SetPlayerPosition(string scenePath, float x, float y, float z = 0)
	{
		_playerPosition.ScenePath = scenePath;
		_playerPosition.X = x;
		_playerPosition.Y = y;
		_playerPosition.Z = z;
	}

	/// <summary>
	/// 获取世界全局状态
	/// </summary>
	public InsWorldGlobalState GetWorldState() => _worldState;

	/// <summary>
	/// 获取地图对象列表
	/// </summary>
	public List<InsMapObjectState> GetMapObjects() => _mapObjects;

	/// <summary>
	/// 获取或创建地图对象状态
	/// </summary>
	public InsMapObjectState GetOrCreateMapObject(string objectId)
	{
		var obj = _mapObjects.Find(o => o.ObjectId == objectId);
		if (obj == null)
		{
			obj = new InsMapObjectState { ObjectId = objectId };
			_mapObjects.Add(obj);
		}
		return obj;
	}

	/// <summary>
	/// 标记地图对象为已采集
	/// </summary>
	public void CollectMapObject(string objectId)
	{
		var obj = GetOrCreateMapObject(objectId);
		obj.IsCollected = true;
		obj.IsActive = false;
	}

	/// <summary>
	/// 获取NPC状态列表
	/// </summary>
	public List<InsNpcState> GetNpcs() => _npcs;

	/// <summary>
	/// 获取或创建NPC状态
	/// </summary>
	public InsNpcState GetOrCreateNpc(string npcId)
	{
		var npc = _npcs.Find(n => n.NpcId == npcId);
		if (npc == null)
		{
			npc = new InsNpcState { NpcId = npcId };
			_npcs.Add(npc);
		}
		return npc;
	}

	/// <summary>
	/// 标记NPC为已击败
	/// </summary>
	public void DefeatNpc(string npcId)
	{
		var npc = GetOrCreateNpc(npcId);
		npc.IsDefeated = true;
	}

	/// <summary>
	/// 推进游戏时间
	/// </summary>
	public void AdvanceTime(int hours = 0, int minutes = 0)
	{
		_worldState.GameTimeMinute += minutes;
		_worldState.GameTimeHour += hours + _worldState.GameTimeMinute / 60;
		_worldState.GameTimeMinute %= 60;
		_worldState.GameTimeHour %= 24;
		if (_worldState.GameTimeHour == 0 && _worldState.GameTimeMinute == 0)
			_worldState.GameDay++;
	}

	/// <summary>
	/// 设置天气
	/// </summary>
	public void SetWeather(string weather) => _worldState.Weather = weather;

	/// <summary>
	/// 解锁新地区
	/// </summary>
	public void UnlockRegion(string regionId)
	{
		if (!_worldState.UnlockedRegions.Contains(regionId))
			_worldState.UnlockedRegions.Add(regionId);
	}
	#endregion

	#region 存档
	/// <summary>
	/// 保存世界数据到文件
	/// </summary>
	public void Save(string filePath = "user://world_data.save")
	{
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
		if (file == null)
		{
			GD.PrintErr("无法打开存档文件: " + filePath);
			return;
		}

		var data = new Godot.Collections.Dictionary
		{
			["player_scene"] = _playerPosition.ScenePath ?? "",
			["player_x"] = _playerPosition.X,
			["player_y"] = _playerPosition.Y,
			["player_z"] = _playerPosition.Z,
			["game_hour"] = _worldState.GameTimeHour,
			["game_minute"] = _worldState.GameTimeMinute,
			["game_day"] = _worldState.GameDay,
			["weather"] = _worldState.Weather,
			["current_region"] = _worldState.CurrentRegion ?? "",
			["unlocked_regions"] = StringListToGodotArray(_worldState.UnlockedRegions),
			["map_objects"] = SerializeMapObjects(_mapObjects),
			["npcs"] = SerializeNpcs(_npcs)
		};
		file.StoreString(Json.Stringify(data));
		GD.Print("世界数据已保存");
	}

	/// <summary>
	/// 从文件加载世界数据
	/// </summary>
	public void Load(string filePath = "user://world_data.save")
	{
		if (!FileAccess.FileExists(filePath))
		{
			GD.Print("存档文件不存在，使用默认世界数据");
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

		_playerPosition.ScenePath = data["player_scene"].AsString();
		_playerPosition.X = (float)data["player_x"];
		_playerPosition.Y = (float)data["player_y"];
		_playerPosition.Z = (float)data["player_z"];

		_worldState.GameTimeHour = (int)data["game_hour"];
		_worldState.GameTimeMinute = (int)data["game_minute"];
		_worldState.GameDay = (int)data["game_day"];
		_worldState.Weather = data["weather"].AsString();
		_worldState.CurrentRegion = data["current_region"].AsString();
		_worldState.UnlockedRegions = GodotArrayToStringList(data["unlocked_regions"].AsGodotArray());

		_mapObjects = DeserializeMapObjects(data["map_objects"].AsGodotArray());
		_npcs = DeserializeNpcs(data["npcs"].AsGodotArray());

		GD.Print("世界数据已加载");
	}

	private Godot.Collections.Array SerializeMapObjects(List<InsMapObjectState> objects)
	{
		var arr = new Godot.Collections.Array();
		foreach (var obj in objects)
		{
			arr.Add(new Godot.Collections.Dictionary
			{
				["object_id"] = obj.ObjectId,
				["is_active"] = obj.IsActive,
				["is_collected"] = obj.IsCollected,
				["custom_states"] = StringDictToGodotDict(obj.CustomStates)
			});
		}
		return arr;
	}

	private List<InsMapObjectState> DeserializeMapObjects(Godot.Collections.Array arr)
	{
		var list = new List<InsMapObjectState>();
		foreach (var item in arr)
		{
			var dict = item.AsGodotDictionary();
			list.Add(new InsMapObjectState
			{
				ObjectId = dict["object_id"].AsString(),
				IsActive = (bool)dict["is_active"],
				IsCollected = (bool)dict["is_collected"],
				CustomStates = GodotDictToStringDict(dict["custom_states"].AsGodotDictionary())
			});
		}
		return list;
	}

	private Godot.Collections.Array SerializeNpcs(List<InsNpcState> npcs)
	{
		var arr = new Godot.Collections.Array();
		foreach (var npc in npcs)
		{
			arr.Add(new Godot.Collections.Dictionary
			{
				["npc_id"] = npc.NpcId,
				["is_defeated"] = npc.IsDefeated,
				["is_dialog_triggered"] = npc.IsDialogTriggered,
				["dialog_phase"] = npc.DialogPhase,
				["custom_states"] = StringDictToGodotDict(npc.CustomStates)
			});
		}
		return arr;
	}

	private List<InsNpcState> DeserializeNpcs(Godot.Collections.Array arr)
	{
		var list = new List<InsNpcState>();
		foreach (var item in arr)
		{
			var dict = item.AsGodotDictionary();
			list.Add(new InsNpcState
			{
				NpcId = dict["npc_id"].AsString(),
				IsDefeated = (bool)dict["is_defeated"],
				IsDialogTriggered = (bool)dict["is_dialog_triggered"],
				DialogPhase = (int)dict["dialog_phase"],
				CustomStates = GodotDictToStringDict(dict["custom_states"].AsGodotDictionary())
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

	private static Godot.Collections.Dictionary StringDictToGodotDict(Dictionary<string, string> source)
	{
		var result = new Godot.Collections.Dictionary();
		foreach (var kv in source)
			result[kv.Key] = kv.Value;
		return result;
	}

	private static Dictionary<string, string> GodotDictToStringDict(Godot.Collections.Dictionary source)
	{
		var result = new Dictionary<string, string>();
		foreach (var key in source.Keys)
			result[key.AsString()] = source[key].AsString();
		return result;
	}
	#endregion
}