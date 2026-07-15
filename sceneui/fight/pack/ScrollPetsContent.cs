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
	/// 加载/刷新宠物列表：清空 VBox 并重新从 pets 数组生成所有项
	/// </summary>
	public void LoadPetItems(InsFightPetData[] pets)
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
	/// 实时刷新：清空并重新加载，用于外部传入最新 InsFightPetData[] 后更新 UI
	/// </summary>
	public void RefreshPetItems(InsFightPetData[] pets)
	{
		LoadPetItems(pets);
	}
}
