using Godot;

/// <summary>
/// 玩家任务数据
/// </summary>
public class InsQuestData
{
	public string QuestId;       // 任务ID
	public bool IsCompleted;     // 是否完成
	public int Progress;         // 当前进度
	public int Target;           // 目标进度
}