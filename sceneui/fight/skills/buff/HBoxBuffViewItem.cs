using Godot;

/// <summary>
/// Buff 展示项
/// 显示单个 Buff 的名称和层数
/// </summary>
public partial class HBoxBuffViewItem : HBoxContainer
{
    /// <summary>
    /// 内部维护的 Buff 数据
    /// </summary>
    public InsFightBuff BuffData { get; private set; }

    /// <summary>
    /// 深层子节点：Buff 名称标签
    /// </summary>
    private Label _labelName;

    /// <summary>
    /// 深层子节点：Buff 层数值标签
    /// </summary>
    private Label _labelValue;

    public override void _Ready()
    {
        // 获取深层子节点
    }

    void Load ()
    {
        if (_labelName == null || _labelValue == null)
        {
            _labelName = (Label) GodotTool.FindChildByName(this, "LabelName");
            _labelValue = (Label) GodotTool.FindChildByName(this, "LabelValue");
        }
    }

    /// <summary>
    /// 更新 Buff 视图
    /// 设置内部 BuffData，并刷新 UI 显示
    /// </summary>
    /// <param name="buff">最新的 Buff 数据</param>
    public void UpdateBuffView(InsFightBuff buff)
    {
        Load();

        BuffData = buff;
        if (buff == null)
        {
            _labelName.Text = "";
            _labelValue.Text = "";
            return;
        }

        _labelValue.Text = DevBuffHelpTool.BuffToText(buff);
    }

}
