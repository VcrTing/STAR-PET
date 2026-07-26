using Godot;

/// <summary>
/// 我方顶部栏精灵信息（单例）
/// 提供我方顶部栏精灵名称、等级、HP、精灵心数信息的更新
/// </summary>
public partial class UiHTopBarPetInfoMy : VBoxContainer
{
	private static UiHTopBarPetInfoMy _instance;
	public static UiHTopBarPetInfoMy Instance => _instance;

	// 子节点引用（在场景树中通过 _Ready 自动查找）
	private Label _hpLabel;
	private TextureProgressBar _hpBar;
	private Label _hpText;
	private Label _labelPetHealth;
	private Label _labelPetName;
	private HBoxContainer _heartsContent;
	private PackedScene _heartItemScene;

	public override void _EnterTree()
	{
		if (_instance != null)
		{
			QueueFree();
			return;
		}
		_instance = this;
	}

	public override void _Ready()
	{
		// 查找子节点：期望场景中包含名为 "HpBar" 的 TextureProgressBar 和 "HpLabel" 的 Label
		_hpBar = FindChild("HpBar", true, false) as TextureProgressBar;
		_hpText = FindChild("HpLabel", true, false) as Label;
		if (_hpText == null)
		{
			// 如果没有 HpLabel，尝试找通用标签
			_hpLabel = FindChild("Label", true, false) as Label;
		}

		// 缓存标签
		_labelPetHealth = FindChild("LabelPetHealth", true, false) as Label;
		_labelPetName = FindChild("LabelPetName", true, false) as Label;

		// 缓存心数容器 HBoxHeartsContent
		_heartsContent = FindChild("HBoxHeartsContent", true, false) as HBoxContainer;

		// 缓存心数场景
		_heartItemScene = ResourceLoader.Load<PackedScene>("res://sceneui/fight/players/alive/ui_fight_heart_item.tscn");
		if (_heartItemScene == null)
		{
			GD.PrintErr("  ⚠ UiHTopBarPetInfoMy 加载 ui_fight_heart_item.tscn 失败");
		}
	}

	/// <summary>
	/// 更新精灵心数显示
	/// 清除旧的心数展示项，根据 heartCount 生成新的 heart 展示项
	/// </summary>
	/// <param name="heartCount">心数数量</param>
	public void UpdateHearts(int heartCount)
	{
		if (_heartsContent == null)
		{
			_heartsContent = FindChild("HBoxHeartsContent", true, false) as HBoxContainer;
			if (_heartsContent == null) return;
		}

		if (_heartItemScene == null)
		{
			_heartItemScene = ResourceLoader.Load<PackedScene>("res://sceneui/fight/players/alive/ui_fight_heart_item.tscn");
			if (_heartItemScene == null) return;
		}

		// 清空旧的心数项
		foreach (Node child in _heartsContent.GetChildren())
		{
			_heartsContent.RemoveChild(child);
			child.QueueFree();
		}

		if (heartCount <= 0) return;

		// 生成新的 heart 展示项
		for (int i = 0; i < heartCount; i++)
		{
			Node heartItem = _heartItemScene.Instantiate();
			if (heartItem != null)
			{
				_heartsContent.AddChild(heartItem);
			}
		}
	}

	/// <summary>
	/// 更新精灵信息展示
	/// 显示宠物名称+等级、HP/MaxHp，同时同步血条
	/// </summary>
	/// <param name="petData">精灵数据</param>
	public void UpdatePetInfo(InsFightPetData petData)
	{
		if (petData == null) return;

		// 懒加载 LabelPetHealth
		if (_labelPetHealth == null)
		{
			_labelPetHealth = FindChild("LabelPetHealth", true, false) as Label;
		}

		// 懒加载 LabelPetName
		if (_labelPetName == null)
		{
			_labelPetName = FindChild("LabelPetName", true, false) as Label;
		}

		int hp = petData.Hp;
		int maxHp = petData.MaxHp;
		if (maxHp <= 0) maxHp = 1;

		// 更新血量和血条
		if (_labelPetHealth != null)
		{
			_labelPetHealth.Text = $"{hp} / {maxHp}";
		}

		if (_hpBar != null)
		{
			_hpBar.MaxValue = maxHp;
			_hpBar.Value = hp;
		}

		// 更新名称和等级
		if (_labelPetName != null)
		{
			_labelPetName.Text = $"{petData.PetName} Lv.{petData.Level}";
		}
	}

	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}
}