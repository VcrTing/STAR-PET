// ════════════════════════════════════════════════════════════════
//  战斗中心管理器（单例）
//  职责：控制战斗进程状态机
//  持有 4 个行动数组，玩家/敌方选择后放入，进入 ExecuteTurn 时委托
//  FightTurnExecutor 执行。执行完毕后自己负责检查濒死、换宠、回合切换。
// ════════════════════════════════════════════════════════════════

using Godot;
using System.Collections.Generic;

/// <summary>
/// 战斗中心管理器（单例 Node2D）
///
/// 【执行顺序 · 必看】
///   👉 StartBattle() 最先执行
///   👉 HandleTurnStart() 在第 0 回合执行系统初始化，然后进入第 1 回合
///
///   完整流程：
///     StartBattle → BattleStart(🎬) → TurnStart 🌅(第0回合·系统初始化)
///     → PlayerTurn(🧑第1回合) → EnemyTurn → ExecuteTurn ⚔️ → CheckFaint 💀
///     → NextTurn → TurnStart 🌅(第N回合初始化) → PlayerTurn(🧑第N回合)
///     → ... 循环 ... 或 → BattleEnd 🏁
///
/// 【启动初始化流程】
///   1. 场景加载 → Godot 自动调用 _EnterTree()
///      └── 注册单例 Instance
///      └── 注入委托 FightTurnExecutor.OnDamageDealt
///      └── 此时还不会开始战斗
///
///   2. Godot 自动调用 _Ready()
///      └── FightGameInit._Ready → Init()
///      └── 打印精灵信息 + 刷新技能UI
///      └── 末尾调用 FightCenterManger.Instance.StartBattle() ← 自动触发
///
///   3. StartBattle() 启动状态机
///      └── BattleStart(🎬) → PlayerTurn(🧑) → 开始第1回合
///
///   ⚠ 不再需要外部手动调用 StartBattle()，FightGameInit 初始化完成后自动触发
///
/// 【4 个行动数组】
///   - MyTurnActs[9]     我方当前回合行动队列（玩家选后放 Index=4）
///   - MyEndActs[4]      我方回合结束持续效果队列
///   - EnemyTurnActs[9]  敌方当前回合行动队列（AI 选后放 Index=4）
///   - EnemyEndActs[4]   敌方回合结束持续效果队列
///
/// 【如何用】
///   1. FightGameInit 初始化后自动调用 StartBattle()
///   2. 玩家回合调用 PlayerSelectSkill("技能ID") 或 PlayerSelectSwitch(index)
///   3. 自动等待敌方 AI 选择，双方都选完后进入回合执行
///   4. 濒死时调用 PlayerSwitchAfterFaint(index) 换宠
/// </summary>
public partial class FightCenterManger : Node2D
{
	// ───────────────────────────── 单例 ─────────────────────────────

	private static FightCenterManger _instance;
	public static FightCenterManger Instance => _instance;

	// ───────────────────────────── 状态 ─────────────────────────────

	/// <summary>当前战斗阶段（状态机当前所在状态）</summary>
	private FightState _currentState = FightState.None;

	/// <summary>当前是第几回合（从 1 开始计数）</summary>
	private int _turnNumber = 0;

	// ───────────────────────────── 4 个行动数组 ─────────────────────

	/// <summary>我方当前回合行动队列，长度 9，玩家选择后放入 Index=4</summary>
	public TurnAction[] MyTurnActs { get; private set; } = new TurnAction[9];

	/// <summary>我方回合结束后的持续效果队列（中毒、灼伤等），长度 4</summary>
	public TurnAction[] MyEndActs { get; private set; } = new TurnAction[4];

	/// <summary>敌方当前回合行动队列，长度 9，AI 选择后放入 Index=4</summary>
	public TurnAction[] EnemyTurnActs { get; private set; } = new TurnAction[9];

	/// <summary>敌方回合结束后的持续效果队列，长度 4</summary>
	public TurnAction[] EnemyEndActs { get; private set; } = new TurnAction[4];

	/// <summary>标记玩家是否已在当前回合做出选择</summary>
	private bool _playerActedThisTurn = false;

	/// <summary>标记敌方是否已在当前回合做出选择</summary>
	private bool _enemyActedThisTurn = false;

	// ───────────────────────────── 信号 ─────────────────────────────

	public const string SignalFightStateChanged = "OnFightStateChanged";
	public const string SignalDamageDealt = "OnDamageDealt";
	public const string SignalPetFainted = "OnPetFainted";
	public const string SignalBattleEnd = "OnBattleEnd";

	/// <summary>战斗状态变更时触发，参数：新状态</summary>
	[Signal] public delegate void OnFightStateChangedEventHandler(FightState newState);

	/// <summary>造成伤害时触发，参数：目标方("player"/"enemy")、伤害值、剩余HP</summary>
	[Signal] public delegate void OnDamageDealtEventHandler(string targetSide, int damage, int remainingHp);

	/// <summary>精灵濒死时触发，参数：方("player"/"enemy")、精灵索引</summary>
	[Signal] public delegate void OnPetFaintedEventHandler(string side, int petIndex);

	/// <summary>战斗结束时触发，参数：玩家是否胜利</summary>
	[Signal] public delegate void OnBattleEndEventHandler(bool playerWin);

	// ───────────────────────────── 生命周期 ─────────────────────────
	//
	// 【初始化流程解释】
	//
	// 当 FightCenterManger 所在场景被加载到场景树时，Godot 自动按以下顺序调用：
	//
	//   1. _EnterTree()
	//      └── 注册单例（静态 Instance）
	//      └── 注入 FightTurnExecutor 的伤害回调委托
	//      └── 此时场景树已挂载，可以 GetNode/FindChild
	//      └── 但此时精灵数据尚未准备好
	//
	//   2. _Ready()
	//      └── 本类 _Ready 为空，不做任何事情
	//      └── 但 FightGameInit._Ready 会执行 Init()，自动触发 StartBattle()
	//
	// 【状态机启动时刻】
	//   FightGameInit.Init() 末尾调用 FightCenterManger.Instance.StartBattle()
	//   因为 FightGameInit 要等所有初始化（精灵数据、技能UI）完成后才启动战斗
	//   StartBattle() 在场景加载 + 初始化完毕后自动运行，不再需要外部手动调用
	// ─────────────────────────────────────────────────────────────────

	public override void _EnterTree()
	{
		// 单例：已存在则销毁自己
		if (_instance != null) { QueueFree(); return; }
		_instance = this;

		// 注入委托：FightTurnExecutor 通过此回调发射 OnDamageDealt 信号
		FightTurnExecutor.OnDamageDealt = (s, d, r) => EmitSignal(SignalDamageDealt, s, d, r);
	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}

	// ════════════════════════════════════════════════════════════════
	//  公开接口
	// ════════════════════════════════════════════════════════════════

	/// <summary>
	/// 开始战斗：初始化状态，进入 BattleStart → PlayerTurn
	/// 由 FightGameInit.Init() 末尾自动调用
	/// </summary>
	public void StartBattle()
	{
		GD.Print("\n═══════════════════════════════════════\n  🎮 战斗开始！\n═══════════════════════════════════════");
		ClearAllQueues();
		_currentState = FightState.BattleStart;
		EmitSignal(SignalFightStateChanged, (int)_currentState);
		HandleBattleStart();
	}

	/// <summary>
	/// 玩家选择技能 → 放入 myTurnActs[4]
	/// 调用方：技能按钮点击时
	/// </summary>
	/// <param name="skillId">技能 ID，如 "0_1_1"</param>
	public void PlayerSelectSkill(string skillId)
	{
		// 校验：当前必须是玩家回合
		if (_currentState != FightState.PlayerTurn) { GD.Print($"  ⚠ 当前不是玩家回合"); return; }

		var pet = FightLandMyStandPet.Instance?.FightPetData;
		if (pet == null) { GD.Print($"  ⚠ 场上无精灵"); return; }

		var skill = FightCenterUtil.FindFightSkill(skillId);
		if (skill == null) { GD.Print($"  ⚠ 技能 {skillId} 不存在"); return; }

		// 获取速度值用于回合排序
		int speed = StatOrDefault(pet.FinalStats, EnumPetBaseStats.SPD, 50);

		// 放入第 5 位 (Index=4)
		MyTurnActs[4] = new TurnAction(TurnActionType.UseSkill, "player", skillId, -1, speed, skill.Skill?.Priority ?? 0);
		_playerActedThisTurn = true;

		GD.Print($"  └─ [玩家] 选择技能【{skill.Skill?.SkillName ?? skillId}】(先手={skill.Skill?.Priority ?? 0}, 速度={speed}) → 等待敌方...");

		// 如果敌方也选了，立刻进入回合执行
		TryExecute();
	}

	/// <summary>
	/// 玩家选择换宠 → 放入 myTurnActs[4]
	/// 调用方：换宠按钮点击时
	/// </summary>
	/// <param name="targetIndex">目标精灵在 FightPets 中的索引</param>
	public void PlayerSelectSwitch(int targetIndex)
	{
		if (_currentState != FightState.PlayerTurn) { GD.Print($"  ⚠ 当前不是玩家回合"); return; }

		var pets = PlayerLandMyStandPlayer.Instance.FightPets;
		if (targetIndex < 0 || targetIndex >= pets.Count || pets[targetIndex].Hp <= 0) { GD.Print($"  ⚠ 目标无效"); return; }

		int speed = StatOrDefault(pets[targetIndex].FinalStats, EnumPetBaseStats.SPD, 50);

		MyTurnActs[4] = new TurnAction(TurnActionType.SwitchPet, "player", null, targetIndex, speed, 0);
		_playerActedThisTurn = true;

		GD.Print($"  └─ [玩家] 换宠 Index={targetIndex} ({pets[targetIndex].PetName}) → 等待敌方...");
		TryExecute();
	}

	/// <summary>
	/// 玩家选择替补上场（濒死换宠）
	/// 当前精灵濒死后调用此接口让玩家选下一只
	/// </summary>
	/// <param name="targetIndex">候补精灵在 FightPets 中的索引</param>
	public void PlayerSwitchAfterFaint(int targetIndex)
	{
		if (_currentState != FightState.PlayerSwitch) { GD.Print($"  ⚠ 当前不是换宠状态"); return; }

		var pets = PlayerLandMyStandPlayer.Instance.FightPets;
		if (targetIndex < 0 || targetIndex >= pets.Count || pets[targetIndex].Hp <= 0) { GD.Print($"  ⚠ 目标无效"); return; }

		GD.Print($"  └─ [玩家] 濒死后换宠 → {pets[targetIndex].PetName}");
		DoPlayerSwitch(targetIndex);
		TransitionTo(FightState.EnemyTurn);
	}

	// ════════════════════════════════════════════════════════════════
	//  状态机
	// ════════════════════════════════════════════════════════════════

	/// <summary>
	/// 切换到新状态：更新状态变量 → 发射信号 → 执行状态处理函数
	/// </summary>
	private void TransitionTo(FightState newState)
	{
		_currentState = newState;
		EmitSignal(SignalFightStateChanged, (int)_currentState);

		// BattleEnd 的 log 由 HandleBattleEnd 自己打印，避免重复
		if (newState != FightState.BattleEnd)
			GD.Print($"  └─ [状态机] → {StateName(newState)}");

		// 根据新状态调用对应的处理方法
		switch (newState)
		{
			case FightState.BattleStart:  HandleBattleStart();  break;
			case FightState.TurnStart:    HandleTurnStart();    break;
			case FightState.PlayerTurn:   HandlePlayerTurn();   break;
			case FightState.EnemyTurn:    HandleEnemyTurn();    break;
			case FightState.ExecuteTurn:  HandleExecuteTurn();  break;
			case FightState.CheckFaint:   HandleCheckFaint();   break;
			case FightState.PlayerSwitch: HandlePlayerSwitch(); break;
			case FightState.EnemySwitch:  HandleEnemySwitch();  break;
			case FightState.BattleEnd:    HandleBattleEnd();    break;
		}
	}

	/// <summary>
	/// 双方都已选择行动时 → 进入回合执行状态
	/// 在 PlayerSelectSkill / PlayerSelectSwitch / HandleEnemyTurn 末尾调用
	/// </summary>
	private void TryExecute()
	{
		if (_playerActedThisTurn && _enemyActedThisTurn)
			TransitionTo(FightState.ExecuteTurn);
	}

	// ════════════════════════════════════════════════════════════════
	//  各状态处理方法
	// ════════════════════════════════════════════════════════════════

	/// <summary>
	/// 🎬 战斗开始：转入 TurnStart（第 0 回合·系统初始化）
	/// 由 FightGameInit.Init() 末尾调用 StartBattle() 触发
	/// </summary>
	private void HandleBattleStart()
	{
		// 进入第 0 回合初始化阶段，系统在这里做一些准备工作
		TransitionTo(FightState.TurnStart);
	}

	/// <summary>
	/// 🧑 玩家回合：重置标记，清空行动队列，等待玩家选择
	/// 玩家通过 PlayerSelectSkill / PlayerSelectSwitch 响应
	/// </summary>
	private void HandlePlayerTurn()
	{
		_playerActedThisTurn = false;
		_enemyActedThisTurn = false;
		FightCenterUtil.ClearActionQueue(MyTurnActs);
		FightCenterUtil.ClearActionQueue(EnemyTurnActs);

		var pet = FightLandMyStandPet.Instance?.FightPetData;
		string info = pet != null ? $"{pet.PetName} (HP={pet.Hp}/{pet.MaxHp})" : "无精灵";
		GD.Print($"━━━ 第 {_turnNumber} 回合 · 玩家回合 · {info} ━━━");
		GD.Print("  ▶ 请选择: PlayerSelectSkill(id) 或 PlayerSelectSwitch(idx)");
	}

	/// <summary>
	/// 👹 敌方回合：AI 选择行动
	/// 目前为占位逻辑，后续实现敌方 AI 决策
	/// </summary>
	private void HandleEnemyTurn()
	{
		GD.Print($"  └─ [敌方] AI思考...");
		EnemyTurnActs[4] = new TurnAction(TurnActionType.None, "enemy");
		_enemyActedThisTurn = true;
		TryExecute();
	}

	/// <summary>
	/// ⚔️ 回合执行：打印行动队列 → 委托 FightTurnExecutor 执行 → 转入濒死检查
	/// </summary>
	private void HandleExecuteTurn()
	{
		GD.Print($"─────────────────\n  ⚔️ 第 {_turnNumber} 回合执行\n─────────────────");
		FightCenterUtil.PrintQueueStatus(MyTurnActs, "我方");
		FightCenterUtil.PrintQueueStatus(EnemyTurnActs, "敌方");

		// 关键：把 4 个数组传给执行器，让它排序+执行
		FightTurnExecutor.ExecuteTurn(MyTurnActs, EnemyTurnActs, MyEndActs, EnemyEndActs);

		TransitionTo(FightState.CheckFaint);
	}

	/// <summary>
	/// 💀 濒死检查：检查双方精灵血量，决定战斗结束、换宠或继续下一回合
	/// </summary>
	private void HandleCheckFaint()
	{
		var playerPet = FightLandMyStandPet.Instance?.FightPetData;
		var enemyPet = FightCenterUtil.GetEnemyActivePet();

		bool playerDead = playerPet != null && playerPet.Hp <= 0;
		bool enemyDead = enemyPet != null && enemyPet.Hp <= 0;

		// 统计我方存活精灵数
		int alive = 0;
		foreach (var p in PlayerLandMyStandPlayer.Instance.FightPets)
			if (p.Hp > 0) alive++;

		// 胜负判定优先级：全灭 > 单只濒死
		if (alive == 0) { GD.Print("  ❌ 我方全灭！战败！"); TransitionTo(FightState.BattleEnd); return; }
		if (enemyDead) { GD.Print("  ✅ 敌方全灭！胜利！"); TransitionTo(FightState.BattleEnd); return; }
		if (playerDead) { EmitSignal(SignalPetFainted, "player", FightCenterUtil.GetCurrentPlayerPetIndex()); TransitionTo(FightState.PlayerSwitch); return; }
		if (enemyDead) { EmitSignal(SignalPetFainted, "enemy", 0); TransitionTo(FightState.EnemySwitch); return; }

		// 都活着 → 下一回合
		NextTurn();
	}

	/// <summary>
	/// 🔄 玩家换宠：提示玩家用 PlayerSwitchAfterFaint(idx) 选择
	/// </summary>
	private void HandlePlayerSwitch() => GD.Print("  ▶ 请调用 PlayerSwitchAfterFaint(idx)");

	/// <summary>
	/// 🔄 敌方换宠：AI 换完后直接进入下一回合
	/// </summary>
	private void HandleEnemySwitch() { GD.Print("  └─ [敌方] 换宠完毕"); NextTurn(); }

	/// <summary>
	/// 🏁 战斗结束：发送胜负信号，打印总结
	/// </summary>
	private void HandleBattleEnd()
	{
		bool win = FightCenterUtil.GetEnemyActivePet()?.Hp <= 0;
		EmitSignal(SignalBattleEnd, win);
		GD.Print($"\n═══════════════════════════════════════\n  🏆 战斗结束! 玩家{(win ? "胜利🎉" : "战败💀")}!\n  ⏱ 共 {_turnNumber} 回合\n═══════════════════════════════════════");
	}

	// ════════════════════════════════════════════════════════════════
	//  内部辅助方法
	// ════════════════════════════════════════════════════════════════

	/// <summary>增加回合数，进入回合开始初始化</summary>
	private void NextTurn() { _turnNumber++; TransitionTo(FightState.TurnStart); }

	/// <summary>
	/// 🌅 回合开始：每回合最开始的初始化
	/// 
	/// 【第 0 回合（战斗开始后第一次进入）】
	///   系统在这里做一些准备工作（如重置某些状态、触发开场效果）
	///   做完后将 _turnNumber 设为 1，然后进入 PlayerTurn（第 1 回合）
	///
	/// 【第 N 回合（NextTurn 进入）】
	///   处理回合开始时的效果触发、检查异常状态、刷新 UI 等
	/// </summary>
	private void HandleTurnStart()
	{
		// TODO: 回合初始化逻辑
		// 例如：触发持续效果、检查异常状态、刷新 UI 等

		var pet = FightLandMyStandPet.Instance?.FightPetData;
		string info = pet != null ? $"{pet.PetName} (HP={pet.Hp}/{pet.MaxHp})" : "无精灵";

		if (_turnNumber == 0)
		{
			// 第 0 回合：系统初始化
			GD.Print("");
			GD.Print($"╔══════════════════════════════════════╗");
			GD.Print($"║      🌅 第 0 回合 · 系统初始化       ║");
			GD.Print($"║      {info,-28} ║");
			GD.Print($"║      ⏳ 战场准备中...                ║");
			GD.Print($"╚══════════════════════════════════════╝");
			// TODO: 开场系统逻辑
			// 例如：重置状态、加载场地效果、触发开场事件等
			GD.Print($"  └─ [系统初始化] 第 0 回合初始化完成，进入第 1 回合");
			_turnNumber = 1;
		}
		else
		{
			// 第 N 回合（N >= 2）：每回合常规初始化
			GD.Print("");
			GD.Print($"╔══════════════════════════════════════╗");
			GD.Print($"║      第 {_turnNumber,2} 回合   🌅 回合开始      ║");
			GD.Print($"║      {info,-28} ║");
			GD.Print($"╚══════════════════════════════════════╝");
		}

		TransitionTo(FightState.PlayerTurn);
	}

	/// <summary>
	/// 执行玩家换宠操作：切换到指定精灵上场
	/// </summary>
	private void DoPlayerSwitch(int idx)
	{
		var pets = PlayerLandMyStandPlayer.Instance.FightPets;
		if (idx >= 0 && idx < pets.Count && pets[idx].Hp > 0)
		{
			GD.Print($"    → 换宠: {pets[idx].PetName}");
			FightLandMyStandPet.Instance?.SwitchPet(pets[idx]);
		}
	}

	/// <summary>清空所有 4 个行动数组（开始新战斗时调用）</summary>
	private void ClearAllQueues()
	{
		FightCenterUtil.ClearActionQueue(MyTurnActs);
		FightCenterUtil.ClearActionQueue(MyEndActs);
		FightCenterUtil.ClearActionQueue(EnemyTurnActs);
		FightCenterUtil.ClearActionQueue(EnemyEndActs);
	}

	/// <summary>从字典安全读取属性值，不存在则返回默认值</summary>
	private static int StatOrDefault(Dictionary<EnumPetBaseStats, int> dict, EnumPetBaseStats key, int def)
	{
		if (dict != null && dict.TryGetValue(key, out int val)) return val;
		return def;
	}

	/// <summary>获取状态的 Emoji 名称（用于控制台输出）</summary>
	private static string StateName(FightState s) => s switch
	{
		FightState.BattleStart => "🎬 战斗开始",
		FightState.TurnStart => "🌅 回合开始",
		FightState.PlayerTurn => "🧑 玩家回合",
		FightState.EnemyTurn => "👹 敌方回合",
		FightState.ExecuteTurn => "⚔️ 回合执行",
		FightState.CheckFaint => "💀 濒死检查",
		FightState.PlayerSwitch => "🔄 玩家换宠",
		FightState.EnemySwitch => "🔄 敌方换宠",
		_ => "❓ " + s.ToString()
	};
}