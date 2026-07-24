using Godot;

public partial class FightMyStandBuffManager : Node2D
{
	private static FightMyStandBuffManager _instance;
	public static FightMyStandBuffManager Instance => _instance;

	/// <summary>
	/// 储存的 Buff 列表
	/// </summary>
	private InsFightBuff[] _buffs = System.Array.Empty<InsFightBuff>();

	public override void _EnterTree()
	{
		if (_instance != null) { QueueFree(); return; }
		_instance = this;
	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}

	/// <summary>
	/// 传入一组 Buff 加入到 buff 列表中
	/// </summary>
	/// <param name="newBuffs">要添加的 Buff 数组</param>
	public void AddBuffs(InsFightBuff[] newBuffs)
	{
		GD.Print($"[FightMyStandBuffManager] AddBuffs: {newBuffs?.Length ?? 0} 个 Buff 加入");
		_buffs = DevBuffHelpTool.MergeBuffs(_buffs, newBuffs);
		// 更新 buff 视图
		if (VBoxViewBuffsContentMy.Instance != null)
		{
			VBoxViewBuffsContentMy.Instance.UpdateBuffs(_buffs);
		}
	}
}