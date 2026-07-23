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
        _labelName = GetNode<Label>("LabelName");
        _labelValue = GetNode<Label>("LabelValue");
    }

    /// <summary>
    /// 更新 Buff 视图
    /// 设置内部 BuffData，并刷新 UI 显示
    /// </summary>
    /// <param name="buff">最新的 Buff 数据</param>
    public void UpdateBuffView(InsFightBuff buff)
    {
        BuffData = buff;
        if (buff == null)
        {
            _labelName.Text = "";
            _labelValue.Text = "";
            return;
        }

        // LabelName 展示 Stat（属性名），如 "物攻"、"物防"
        _labelName.Text = PetBaseStatsDesign.GetName((int)buff.Stat);

        // LabelValue 展示 Layer（层数），如 "+1"、"+2"、"-1"
        string sign = buff.Layer >= 0 ? "+" : "";
        _labelValue.Text = $"{sign}{buff.Layer}";
    }

}
