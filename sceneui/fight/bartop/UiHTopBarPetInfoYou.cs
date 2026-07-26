using Godot;

/// <summary>
/// 敌方顶部栏精灵信息（单例）
/// 提供敌方顶部栏精灵名称、等级、HP 信息的更新
/// 模仿 UiHTopBarPetInfoMy 的敌方版本
/// </summary>
public partial class UiHTopBarPetInfoYou : VBoxContainer
{
	private static UiHTopBarPetInfoYou _instance;
	public static UiHTopBarPetInfoYou Instance => _instance;

	// 子节点引用（在场景树中通过 _Ready 自动查找）
	private Label _hpLabel;
	private TextureProgressBar _hpBar;
	private Label _hpText;
	private Label _labelPetHealth;
	private Label _labelPetName;

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
	}

	/// <summary>
	/// 更新精灵信息展示
	/// 显示敌方宠物名称+等级、HP/MaxHp，同时同步血条
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