using Godot;

/// <summary>
/// 我方 Buff 集中管理类（单例）
/// 负责展示当前我方宠物身上的所有 Buff
/// </summary>
public partial class VBoxViewBuffsContentMy : VBoxContainer
{
    /// <summary>
    /// 单例实例
    /// </summary>
    public static VBoxViewBuffsContentMy Instance { get; private set; }

    /// <summary>
    /// Buff 展示容器子节点
    /// </summary>
    private VBoxContainer _buffsContent;

    /// <summary>
    /// h_box_buff_view_item.tscn 场景资源（缓存）
    /// </summary>
    private PackedScene _buffItemScene;

    public override void _Ready()
    {
        // 注册单例
        Instance = this;

        // 获取 BuffsContent 子节点
        _buffsContent = GetNode<VBoxContainer>("BuffsContent");

        // 加载 Buff 展示项场景
        _buffItemScene = ResourceLoader.Load<PackedScene>("res://sceneui/fight/skills/buff/h_box_buff_view_item.tscn");
        if (_buffItemScene == null)
        {
            GD.PrintErr("❌ VBoxViewBuffsContentMy: h_box_buff_view_item.tscn 加载失败");
        }
    }

    /// <summary>
    /// 更新 Buff 列表视图
    /// 清空原有内容，根据传入的 Buff 数组重新生成展示项
    /// </summary>
    /// <param name="buffs">要展示的 Buff 数组</param>
    public void UpdateBuffs(InsFightBuff[] buffs)
    {
        if (_buffsContent == null || _buffItemScene == null)
            return;

        // 1. 删掉 BuffsContent 内所有子节点
        foreach (Node child in _buffsContent.GetChildren())
        {
            _buffsContent.RemoveChild(child);
            child.QueueFree();
        }

        if (buffs == null || buffs.Length == 0)
            return;

        // 2. 循环 InsFightBuff[]，生成展示项
        for (int i = 0; i < buffs.Length; i++)
        {
            InsFightBuff buff = buffs[i];
            if (buff == null)
                continue;

            // 实例化场景
            HBoxBuffViewItem item = _buffItemScene.Instantiate<HBoxBuffViewItem>();
            if (item == null)
                continue;

            // 更新视图
            item.UpdateBuffView(buff);

            // 添加到 BuffsContent 中展示
            _buffsContent.AddChild(item);
        }
    }
}