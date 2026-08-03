using Godot;

/// <summary>
/// Power 展示项
/// 显示单个 Power 的名称和层数
/// </summary>
public partial class HBoxPowerViewItem : HBoxContainer
{
    /// <summary>
    /// 内部维护的 Power 数据
    /// </summary>
    public InsFightPower PowerData { get; private set; }

    /// <summary>
    /// 深层子节点：Power 名称标签
    /// </summary>
    private Label _labelName;

    /// <summary>
    /// 深层子节点：Power 层数值标签
    /// </summary>
    private Label _labelValue;

    public override void _Ready()
    {
        // 获取深层子节点
    }

    void Load()
    {
        if (_labelName == null || _labelValue == null)
        {
            _labelName = (Label)GodotTool.FindChildByName(this, "LabelName");
            _labelValue = (Label)GodotTool.FindChildByName(this, "LabelValue");
        }
    }

    /// <summary>
    /// 更新 Power 视图
    /// 设置内部 PowerData，并刷新 UI 显示
    /// </summary>
    /// <param name="power">最新的 Power 数据</param>
    public void UpdatePowerView(InsFightPower power)
    {
        Load();

        PowerData = power;
        if (power == null)
        {
            _labelName.Text = "";
            _labelValue.Text = "";
            return;
        }

        _labelValue.Text = DevPowerDesign.PowerToText(power);
    }
}