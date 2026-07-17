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

		// 清空原有项
		foreach (Node child in _vBoxPetsContent.GetChildren())
		{
			child.QueueFree();
		}

		foreach (var pet in pets)
		{
			var item = _itemScene.Instantiate<BtnPackPetItem>();
			item.SetPetData(pet);
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

		var children = _vBoxPetsContent.GetChildren();
		int count = Mathf.Min(children.Count, pets.Length);
		for (int i = 0; i < count; i++)
		{
			if (children[i] is BtnPackPetItem item)
			{
				item.SetPetData(pets[i]);
			}
		}
	}

	/// <summary>
	/// 刷新宠物列表：
	/// 子节点为空或宠物数量与子节点数量不一致时调用 InitPetItems 重新创建，
	/// 否则调用 AsyncPetItems 直接刷新数据
	/// </summary>
	public void RefreshPetItems(InsFightPetData[] pets)
	{
		var children = _vBoxPetsContent.GetChildren();
		if (children.Count == 0 || children.Count != pets.Length)
			InitPetItems(pets);
		else
			AsyncPetItems(pets);
	}
}
