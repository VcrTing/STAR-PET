// ════════════════════════════════════════════════════════════════
//  战斗系统 - 枚举 & 数据类
//  定义战斗状态、行动类型、回合行动实例
// ════════════════════════════════════════════════════════════════

/// <summary>
/// 战斗状态枚举 —— 控制整个战斗流程的阶段切换
/// 每个值对应 FightCenterManger 状态机中的一个阶段
/// </summary>
public enum FightState
{
	/// <summary>初始状态，战斗尚未开始</summary>
	None,
	/// <summary>🎬 战斗开始：播放入场动画、显示"上吧！XX！"</summary>
	BattleStart,
	/// <summary>🌅 回合开始：每回合最开始的初始化，如触发持续效果、检查状态</summary>
	TurnStart,
	/// <summary>🧑 玩家回合：等待玩家选择技能或切换精灵</summary>
	PlayerTurn,
	/// <summary>👹 敌方回合：AI 自动选择行动</summary>
	EnemyTurn,
	/// <summary>⚔️ 回合执行：委托 FightTurnExecutor 排序并执行双方行动</summary>
	ExecuteTurn,
	/// <summary>💀 濒死检查：判断是否有精灵血量归零，决定是否换宠或结束战斗</summary>
	CheckFaint,
	/// <summary>🔄 玩家换宠：当前精灵濒死，玩家需选择下一只上场</summary>
	PlayerSwitch,
	/// <summary>🔄 敌方换宠：敌方精灵濒死，AI 选择下一只上场</summary>
	EnemySwitch,
	/// <summary>🏁 战斗结束：胜负判定，发送 SignalBattleEnd 信号</summary>
	BattleEnd
}

/// <summary>
/// 回合行动类型 —— 标识 TurnAction 的具体行动内容
/// </summary>
public enum TurnActionType
{
	/// <summary>无行动 / 占位</summary>
	None,
	/// <summary>使用技能攻击</summary>
	UseSkill,
	/// <summary>切换上场精灵</summary>
	SwitchPet,
}

/// <summary>
/// 单次回合行动实例 —— 玩家或敌方在本回合要执行的一个动作
/// 存放在 myTurnActs[9] / enemyTurnActs[9] 数组中
/// 放入第 5 位 (Index=4)，回合执行时统一排序处理
/// </summary>
public class TurnAction
{
	/// <summary>行动类型：UseSkill（用技能）或 SwitchPet（换宠）</summary>
	public TurnActionType ActionType = TurnActionType.None;

	/// <summary>行动方："player" 表示我方，"enemy" 表示敌方</summary>
	public string Side;

	/// <summary>技能 ID（仅 ActionType=UseSkill 时有意义）</summary>
	public string SkillId;

	/// <summary>换宠目标索引（仅 ActionType=SwitchPet 时有意义，对应 FightPets 列表中的下标）</summary>
	public int SwitchTargetIndex;

	/// <summary>该行动时的速度值（用于排序：同优先级下速度快的先手）</summary>
	public int Speed;

	/// <summary>先手优先级（数值越大越先执行，技能自带 Priority 属性）</summary>
	public int Priority;

	public TurnAction() {}

	/// <summary>
	/// 构造一个完整的回合行动
	/// </summary>
	/// <param name="type">行动类型</param>
	/// <param name="side">"player" 或 "enemy"</param>
	/// <param name="skillId">技能 ID（选技能时传入）</param>
	/// <param name="switchIndex">换宠目标索引（换宠时传入）</param>
	/// <param name="speed">速度值</param>
	/// <param name="priority">先手值</param>
	public TurnAction(TurnActionType type, string side, string skillId = null, int switchIndex = -1, int speed = 0, int priority = 0)
	{
		ActionType = type;
		Side = side;
		SkillId = skillId;
		SwitchTargetIndex = switchIndex;
		Speed = speed;
		Priority = priority;
	}

	/// <summary>是否为有效行动（ActionType 不为 None）</summary>
	public bool IsValid => ActionType != TurnActionType.None;
}