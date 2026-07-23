using Godot;

/// <summary>
/// 敌方 Buff 集中管理类（单例）
/// 负责展示当前敌方宠物身上的所有 Buff 以及技能信息
/// </summary>
public partial class VBoxViewBuffsContentYou : VBoxContainer
{
    /// <summary>
    /// 单例实例
    /// </summary>
    public static VBoxViewBuffsContentYou Instance { get; private set; }

    /// <summary>
    /// Buff 展示容器子节点
    /// </summary>
    private VBoxContainer _buffsContent;

    /// <summary>
    /// 技能展示容器子节点
    /// </summary>
    private VBoxContainer _skillContent;

    /// <summary>
    /// h_box_buff_view_item.tscn 场景资源（缓存）
    /// </summary>
    private PackedScene _buffItemScene;

    public override void _Ready()
    {
        // 注册单例
        Instance = this;

        // 获取子节点
        _buffsContent = GetNode<VBoxContainer>("BuffsContent");
        _skillContent = GetNode<VBoxContainer>("SkillContent");

        // 加载 Buff 展示项场景
        _buffItemScene = ResourceLoader.Load<PackedScene>("res://sceneui/fight/skills/buff/h_box_buff_view_item.tscn");
        if (_buffItemScene == null)
        {
            GD.PrintErr("❌ VBoxViewBuffsContentYou: h_box_buff_view_item.tscn 加载失败");
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

    /// <summary>
    /// 更新敌方技能列表视图
    /// 清空 SkillContent 内所有子节点，根据传入的技能数组生成 Label 展示技能名
    /// </summary>
    /// <param name="skills">敌方宠物的技能数组</param>
    public void UpdateSkills(InsFightSkill[] skills)
    {
        if (_skillContent == null)
            return;

        // 1. 删掉 SkillContent 内所有子节点
        foreach (Node child in _skillContent.GetChildren())
        {
            _skillContent.RemoveChild(child);
            child.QueueFree();
        }

        if (skills == null || skills.Length == 0)
            return;

        // 2. 循环 InsFightSkill[]，生成 Label
        for (int i = 0; i < skills.Length; i++)
        {
            InsFightSkill skill = skills[i];
            if (skill?.Skill == null)
                continue;

            Label label = new Label();
            label.Text = skill.Skill.SkillName;
            _skillContent.AddChild(label);
        }
    }
}