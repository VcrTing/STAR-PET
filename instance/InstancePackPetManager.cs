using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 精灵背包单例管理器
/// 管理玩家背包中精灵的动态数据（等级、经验、个体值、技能、道具等）
/// 负责动态数据的存储、加载、序列化/反序列化
/// </summary>
public partial class InstancePackPetManager : Node
{
	#region 单例
	private static InstancePackPetManager _instance;
	public static InstancePackPetManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new InstancePackPetManager();
				_instance.Name = "InstancePackPetManager";
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
			_instance = new InstancePackPetManager();
			_instance.Name = "InstancePackPetManager";
			parent.AddChild(_instance);
		}
	}
	#endregion

	private InsPackData _packData = new();

	#region 公开方法
	/// <summary>
	/// 获取背包数据
	/// </summary>
	public InsPackData GetPackData() => _packData;

	/// <summary>
	/// 获取背包精灵列表
	/// </summary>
	public List<InsPackPetData> GetPets() => _packData.Pets;

	/// <summary>
	/// 添加精灵到背包
	/// </summary>
	public void AddPet(InsPackPetData pet)
	{
		if (_packData.Pets.Count >= _packData.MaxCapacity)
		{
			GD.PrintErr("背包已满，无法添加精灵");
			return;
		}
		_packData.Pets.Add(pet);
	}

	/// <summary>
	/// 从背包移除精灵
	/// </summary>
	public void RemovePet(string petUuid)
	{
		_packData.Pets.RemoveAll(p => p.PetUuid == petUuid);
	}

	/// <summary>
	/// 根据UUID获取精灵
	/// </summary>
	public InsPackPetData GetPetByUuid(string petUuid)
	{
		return _packData.Pets.Find(p => p.PetUuid == petUuid);
	}

	/// <summary>
	/// 根据索引获取精灵
	/// </summary>
	public InsPackPetData GetPetByIndex(int index)
	{
		if (index < 0 || index >= _packData.Pets.Count)
			return null;
		return _packData.Pets[index];
	}

	/// <summary>
	/// 获取背包精灵数量
	/// </summary>
	public int GetPetCount() => _packData.Pets.Count;
	#endregion

	#region 存档
	/// <summary>
	/// 保存背包数据到文件
	/// </summary>
	public void Save(string filePath = "user://pack_pet_data.save")
	{
		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Write);
		if (file == null)
		{
			GD.PrintErr("无法打开存档文件: " + filePath);
			return;
		}

		var data = new Godot.Collections.Dictionary
		{
			["pets"] = SerializePackPets(_packData.Pets),
			["max_capacity"] = _packData.MaxCapacity
		};
		file.StoreString(Json.Stringify(data));
		GD.Print("背包精灵数据已保存");
	}

	/// <summary>
	/// 从文件加载背包数据
	/// </summary>
	public void Load(string filePath = "user://pack_pet_data.save")
	{
		if (!FileAccess.FileExists(filePath))
		{
			GD.Print("存档文件不存在，使用默认数据");
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
		_packData.Pets = DeserializePackPets(data["pets"].AsGodotArray());
		_packData.MaxCapacity = (int)data["max_capacity"];
		GD.Print("背包精灵数据已加载");
	}

	private Godot.Collections.Array SerializePackPets(List<InsPackPetData> pets)
	{
		var arr = new Godot.Collections.Array();
		foreach (var pet in pets)
		{
			arr.Add(new Godot.Collections.Dictionary
			{
				["pet_uuid"] = pet.PetUuid ?? "",
				["pet_id"] = pet.PetId,
				["pet_name"] = pet.PetName ?? "",
				["pet_types"] = EnumListToIntArray(pet.PetTypes),
				["nickname"] = pet.Nickname ?? "",
				["level"] = pet.Level,
				["exp"] = pet.Exp,
				["hp"] = pet.Hp,
				["max_hp"] = pet.MaxHp,
				["iv"] = EnumDictToIntDict(pet.Iv),
				["ev"] = EnumDictToIntDict(pet.Talent),
				["skills"] = StringListToGodotArray(pet.Skills),
				["nature"] = (int)pet.Nature,
				["gender"] = (int)pet.Gender,
				["ball_type"] = pet.BallType,
				["pet_fly"] = EnumListToIntArray(pet.PetFly),
				["pet_big"] = (int)pet.PetBig,
				["pet_ability"] = (int)pet.PetAbility,
				["is_shiny"] = pet.IsShiny,
				["hatch_counter"] = pet.HatchCounter,
				["intimacy"] = pet.Intimacy,
				["is_locked"] = pet.IsLocked,
				["is_special"] = pet.IsSpecial,
				["obtained_date"] = pet.ObtainedDate ?? "",
				["obtained_method"] = pet.ObtainedMethod ?? "",
				["obtained_location"] = pet.ObtainedLocation ?? "",
				["medals"] = EnumListToIntArray(pet.Medals),
				["final_stats"] = EnumDictToIntDict(pet.FinalStats)
			});
		}
		return arr;
	}

	private List<InsPackPetData> DeserializePackPets(Godot.Collections.Array arr)
	{
		var list = new List<InsPackPetData>();
		foreach (var item in arr)
		{
			var dict = item.AsGodotDictionary();
			var pet = new InsPackPetData
			{
				PetUuid = dict["pet_uuid"].AsString(),
				PetId = dict["pet_id"].AsString(),
				PetName = dict.ContainsKey("pet_name") ? dict["pet_name"].AsString() : "",
				PetTypes = IntArrayToEnumList<EnumPetType>(dict["pet_types"].AsGodotArray()),
				Nickname = dict["nickname"].AsString(),
				Level = (int)dict["level"],
				Exp = (int)dict["exp"],
				Hp = (int)dict["hp"],
				MaxHp = (int)dict["max_hp"],
				Iv = IntDictToEnumDict<EnumPetBaseStats>(dict["iv"].AsGodotDictionary()),
				Talent = IntDictToEnumDict<EnumPetBaseStats>(dict["ev"].AsGodotDictionary()),
				Skills = GodotArrayToStringList(dict["skills"].AsGodotArray()),
				Nature = (EnumPetNature)(int)dict["nature"],
				Gender = (EnumPetGender)(int)dict["gender"],
				BallType = (int)dict["ball_type"],
				PetFly = dict.ContainsKey("pet_fly")
					? IntArrayToEnumList<EnumPetFly>(dict["pet_fly"].AsGodotArray())
					: new List<EnumPetFly> { EnumPetFly.Walk },
				PetBig = dict.ContainsKey("pet_big") ? (EnumPetBig)(int)dict["pet_big"] : EnumPetBig.Normal,
				PetAbility = dict.ContainsKey("pet_ability") ? (EnumPetAbility)(int)dict["pet_ability"] : EnumPetAbility.None,
				IsShiny = (bool)dict["is_shiny"],
				HatchCounter = (int)dict["hatch_counter"],
				Intimacy = (int)dict["intimacy"],
				IsLocked = (bool)dict["is_locked"],
				IsSpecial = (bool)dict["is_special"],
				ObtainedDate = dict["obtained_date"].AsString(),
				ObtainedMethod = dict["obtained_method"].AsString(),
				ObtainedLocation = dict["obtained_location"].AsString(),
				Medals = IntArrayToEnumList<EnumPetMedal>(dict["medals"].AsGodotArray()),
				FinalStats = dict.ContainsKey("final_stats")
					? IntDictToEnumDict<EnumPetBaseStats>(dict["final_stats"].AsGodotDictionary())
					: new Dictionary<EnumPetBaseStats, int>()
			};
			list.Add(pet);
		}
		return list;
	}

	// ---- 序列化辅助 ----

	private static Godot.Collections.Dictionary EnumDictToIntDict<T>(Dictionary<T, int> source) where T : struct, Enum
	{
		var result = new Godot.Collections.Dictionary();
		foreach (var kv in source)
			result[Convert.ToInt32(kv.Key).ToString()] = kv.Value;
		return result;
	}

	private static Dictionary<T, int> IntDictToEnumDict<T>(Godot.Collections.Dictionary source) where T : struct, Enum
	{
		var result = new Dictionary<T, int>();
		foreach (var key in source.Keys)
		{
			int intVal = int.Parse(key.AsString());
			result[(T)(object)intVal] = (int)source[key];
		}
		return result;
	}

	private static Godot.Collections.Array EnumListToIntArray<T>(List<T> source) where T : struct, Enum
	{
		var arr = new Godot.Collections.Array();
		foreach (var item in source)
			arr.Add(Convert.ToInt32(item));
		return arr;
	}

	private static List<T> IntArrayToEnumList<T>(Godot.Collections.Array source) where T : struct, Enum
	{
		var list = new List<T>();
		foreach (var item in source)
			list.Add((T)(object)(int)item);
		return list;
	}

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