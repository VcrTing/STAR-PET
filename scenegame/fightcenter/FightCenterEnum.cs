// ════════════════════════════════════════════════════════════════
//  战斗系统 - 枚举 & 数据类
// ════════════════════════════════════════════════════════════════

/// <summary>战斗状态枚举</summary>
public enum FightState
{
	None,           // 初始状态
	BattleStart,    // 🎬 战斗开始
	TurnStart,      // 🌅 回合开始
	PlayerTurn,     // 🧑 玩家回合
	EnemyTurn,      // 👹 敌方回合
	ExecuteTurn,    // ⚔️ 回合执行
	CheckFaint,     // 💀 濒死检查
	PlayerSwitch,   // 🔄 玩家换宠
	EnemySwitch,    // 🔄 敌方换宠
	BattleEnd       // 🏁 战斗结束
}

/// <summary>回合行动类型</summary>
public enum TurnActionType
{
	None,        // 无行动
	UseSkill,    // 使用技能
	SwitchPet,   // 切换精灵
}

/// <summary>单次回合行动实例</summary>
public class TurnAction
{
	public TurnActionType ActionType = TurnActionType.None;
	public string Side;                    // "player" / "enemy"
	public string SkillId;                 // 技能ID
	public InsFightSkill FightSkill;       // 战斗技能实例
	public int SwitchTargetIndex = -1;     // 换宠目标索引
	public int Speed;                      // 速度值
	public int Priority;                   // 先手值

	public TurnAction() {}

	public TurnAction(string side, InsFightSkill fightSkill)
	{
		ActionType = TurnActionType.UseSkill;
		Side = side;
		FightSkill = fightSkill;
		SkillId = fightSkill?.Skill?.SkillId;
		Speed = fightSkill?.Skill?.Priority ?? 0;
		Priority = fightSkill?.Skill?.Priority ?? 0;
	}

	public TurnAction(string side, int switchIndex, int speed)
	{
		ActionType = TurnActionType.SwitchPet;
		Side = side;
		SwitchTargetIndex = switchIndex;
		Speed = speed;
		Priority = 0;
	}

	public TurnAction(TurnActionType type, string side)
	{
		ActionType = type;
		Side = side;
	}

	public bool IsValid => ActionType != TurnActionType.None;
}