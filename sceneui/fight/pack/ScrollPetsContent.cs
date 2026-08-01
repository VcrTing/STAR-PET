using Godot;
using System;

public partial class ScrollPetsContent : ScrollContainer
{
	public static ScrollPetsContent Instance { get; private set; }

	private VBoxContainer _vBoxPetsContent;
	private PackedScene _itemScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		_vBoxPetsContent = GetNode<VBoxContainer>("VBoxPetsContent");
		_itemScene = GD.Load<PackedScene>("res://sceneui/fight/pets/btn_pack_pet_item.tscn");
		if (_itemScene == null)
		{
			GD.PrintErr("  ⚠ 加载 btn_pack_pet_item.tscn 失败");
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}

	/// <summary>
	/// 初始化宠物列表：清空 VBox 并重新从 pets 数组生成所有项
	/// </summary>
	public void InitPetItems(InsFightPetData[] pets)
	{
		if (_itemScene == null) return;
		if (pets == null) return;

		// 清空原有项
		foreach (Node child in _vBoxPetsContent.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var pet in pets)
		{
			Control item = _itemScene.Instantiate<Control>();
			if (item == null) continue;

			BtnPackPetItem petItem = item.FindChild("BtnPackPetItem", true, false) as BtnPackPetItem;
			if (petItem == null)
			{
				GD.PrintErr($"  ⚠ ScrollPetsContent InitPetItems 实例场景中未找到 BtnPackPetItem 子节点，使用 item 自身");
				petItem = item as BtnPackPetItem;
				if (petItem == null)
				{
					item.QueueFree();
					continue;
				}
			}
			petItem.SetPetData(pet);
			_vBoxPetsContent.AddChild(item);
		}
	}

	/// <summary>
	/// 异步刷新宠物列表：不销毁子节点，直接调用每个 BtnPackPetItem.SetPetData 刷新视图
	/// 数组长度必须与现有子节点数量一致
	/// </summary>
	public void AsyncPetItems(InsFightPetData[] pets)
	{
		if (pets == null) return;
		GD.Print("AsyncPetItems Running.");
		
		var children = _vBoxPetsContent.GetChildren();
		int count = Mathf.Min(children.Count, pets.Length);
		for (int i = 0; i < count; i++)
		{
			// 子节点是 btn_pack_pet_item.tscn 的根节点 MarginContainer，
			// BtnPackPetItem 是其子节点，需 FindChild 获取后刷新
			var container = children[i];
			if (container == null) continue;

			BtnPackPetItem item = container.FindChild("BtnPackPetItem", true, false) as BtnPackPetItem;
			if (item == null)
			{
				item = container as BtnPackPetItem;
				if (item == null) continue;
			}

			item.SetPetData(pets[i]);
		}
	}

	/// <summary>
	/// 刷新宠物列表：
	/// 子节点为空或宠物数量与子节点数量不一致时调用 InitPetItems 重新创建，
	/// 否则调用 AsyncPetItems 直接刷新数据
	/// </summary>
	public void RefreshPackPetItems(InsFightPetData[] pets)
	{
		var children = _vBoxPetsContent.GetChildren();
		if (children.Count == 0 || children.Count != pets.Length)
			InitPetItems(pets);
		else
		{
			AsyncPetItems(pets);
		}

		// 刷新详情面板：读取 pets 中血量大于 0 的第一个精灵
		VBoxPetMsgContent.Instance?.UpdatePetData(GetFirstAlivePet(pets));
	}

	/// <summary>
	/// 获取宠物数组中血量大于 0 的第一个存活精灵（未找到返回 null）
	/// </summary>
	/// <param name="pets">宠物数据数组</param>
	/// <returns>第一只存活精灵，无存活时返回 null</returns>
	private InsFightPetData GetFirstAlivePet(InsFightPetData[] pets)
	{
		if (pets == null) return null;

		for (int i = 0; i < pets.Length; i++)
		{
			if (pets[i] != null && pets[i].Hp > 0)
			{
				return pets[i];
			}
		}
		return null;
	}
}
