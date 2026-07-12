using Godot;
using System.Collections.Generic;

/// <summary>
/// NPC状态
/// </summary>
public class InsNpcState
{
	public string NpcId;           // NPC ID
	public bool IsDefeated;        // 是否已被击败
	public bool IsDialogTriggered; // 对话是否已触发
	public int DialogPhase;        // 当前对话阶段
	public Dictionary<string, string> CustomStates = new();
}