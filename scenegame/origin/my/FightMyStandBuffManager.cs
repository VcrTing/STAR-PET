using Godot;

public partial class FightMyStandBuffManager : Node2D
{
	/// <summary>
	/// 储存的 Buff 列表
	/// </summary>
	private InsFightBuff[] _buffs = System.Array.Empty<InsFightBuff>();

	/// <summary>
	/// 传入一组 Buff 加入到 buff 列表中
	/// </summary>
	/// <param name="newBuffs">要添加的 Buff 数组</param>
	public void AddBuffs(InsFightBuff[] newBuffs)
	{
		_buffs = DevBuffHelpTool.MergeBuffs(_buffs, newBuffs);

		// 更新 buff 视图
		if (VBoxViewBuffsContentMy.Instance != null)
		{
			VBoxViewBuffsContentMy.Instance.UpdateBuffs(_buffs);
		}
	}
}