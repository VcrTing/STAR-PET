// ════════════════════════════════════════════════════════════════
//  战斗中心管理器（单例）
//  职责：控制战斗进程状态机
// ════════════════════════════════════════════════════════════════

using Godot;
using System.Collections.Generic;

public partial class FightCenterManger : Node2D
{
	// ─── 单例 ───
	private static FightCenterManger _instance;
	public static FightCenterManger Instance => _instance;

	// ─── 状态 ───
	private FightState _currentState = FightState.None;
	private bool _battleStarted = false;
	private int _turnNumber = 0;

	// ─── 4 个行动数组 ───
	public TurnAction[] MyTurnActs { get; private set; } = new TurnAction[9];
	public TurnAction[] MyEndActs { get; private set; } = new TurnAction[4];
	public TurnAction[] YouTurnActs { get; private set; } = new TurnAction[9];
	public TurnAction[] YouEndActs { get; private set; } = new TurnAction[4];

	private bool _playerActedThisTurn = false;
	private bool _youActedThisTurn = false;

	// ─── 信号 ───
	public const string SignalFightStateChanged = "OnFightStateChanged";
	public const string SignalDamageDealt = "OnDamageDealt";
	public const string SignalPetFainted = "OnPetFainted";
	public const string SignalBattleEnd = "OnBattleEnd";

	[Signal] public delegate void OnFightStateChangedEventHandler(FightState newState);
	[Signal] public delegate void OnDamageDealtEventHandler(string targetSide, int damage, int remainingHp);
	[Signal] public delegate void OnPetFaintedEventHandler(string side, int petIndex);
	[Signal] public delegate void OnBattleEndEventHandler(bool playerWin);

	// ─── 生命周期 ───
	public override void _EnterTree()
	{
		if (_instance != null) { QueueFree(); return; }
		_instance = this;
		FightTurnExecutor.OnDamageDealt = (s, d, r) => EmitSignal(SignalDamageDealt, s, d, r);
	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}

	// ══════════════════════════════════════════
	//  公开接口
	// ══════════════════════════════════════════

	public bool CanPlayerAct()
	{
		if (_currentState == FightState.PlayerSwitch) return true;
		return _currentState == FightState.PlayerTurn && !_playerActedThisTurn;
	}

	public bool CanUseSkill(InsFightSkill fightSkill) => true;

	public void StartBattle()
	{
		if (_battleStarted) return;
		_battleStarted = true;
		GD.Print("\n═══════════════════════════════════════\n  🎮 战斗开始！\n═══════════════════════════════════════");
		LabelGameStatus.SetText("🎮 战斗开始！");
		ClearAllQueues();
		_currentState = FightState.BattleStart;
		EmitSignal(SignalFightStateChanged, (int)_currentState);
		HandleBattleStart();
	}

	public void PlayerSelectSkill(InsFightSkill fightSkill)
	{
		if (_currentState != FightState.PlayerTurn) { GD.Print($"  ⚠ 当前不是玩家回合"); return; }
		if (fightSkill?.Skill == null) { GD.Print($"  ⚠ 技能数据无效"); return; }

		MyTurnActs[4] = new TurnAction("my", fightSkill);
		_playerActedThisTurn = true;

		GD.Print($"  └─ [玩家] 选择技能【{fightSkill.Skill.SkillName}】(先手={fightSkill.Skill.Priority}) → 等待敌方...");

		// 检查Pve
		PveRunning();
		// 检查下一步
		TryExecute();
	}

	public void PlayerSelectSwitch(int targetIndex)
	{
		if (_currentState != FightState.PlayerTurn) { GD.Print($"  ⚠ 当前不是玩家回合"); return; }
		var pets = PlayerLandMyStandPlayer.Instance.FightPets;
		if (targetIndex < 0 || targetIndex >= pets.Count || pets[targetIndex].Hp <= 0) { GD.Print($"  ⚠ 目标无效"); return; }

		int speed = FightCenterUtil.StatOrDefault(pets[targetIndex].FinalStats, EnumPetBaseStats.SPD, 50);
		MyTurnActs[4] = new TurnAction("my", targetIndex, speed);
		_playerActedThisTurn = true;
		GD.Print($"  └─ [玩家] 换宠 Index={targetIndex} ({pets[targetIndex].PetName}) → 等待敌方...");

		// 检查Pve
		PveRunning();
		// 检查下一步
		TryExecute();
	}

	public void PlayerSwitchAfterFaint(int targetIndex)
	{
		if (_currentState != FightState.PlayerSwitch) { GD.Print($"  ⚠ 当前不是换宠状态"); return; }
		var pets = PlayerLandMyStandPlayer.Instance.FightPets;
		if (targetIndex < 0 || targetIndex >= pets.Count || pets[targetIndex].Hp <= 0) { GD.Print($"  ⚠ 目标无效"); return; }
		GD.Print($"  └─ [玩家] 濒死后换宠 → {pets[targetIndex].PetName}");
		DoPlayerSwitch(targetIndex);
		TransitionTo(FightState.EnemyTurn);
	}

	public void SetPveActedAndExecute()
	{
		_youActedThisTurn = true;
		TransitionTo(FightState.ExecuteTurn);
	}
	// Pve 执行
	public void PveRunning()
	{
		if (FightGameInit.Instance != null && !FightGameInit.Instance.IsPvp)
		{
			FightPveRunner.RunPve(MyTurnActs[4]);
			return;
		}
	}

	private void TryExecute()
	{
		if (!_playerActedThisTurn || !_youActedThisTurn) return;
		TransitionTo(FightState.ExecuteTurn);
	}

	// ─── 状态机 ───

	private void TransitionTo(FightState newState)
	{
		_currentState = newState;
		EmitSignal(SignalFightStateChanged, (int)_currentState);
		if (newState != FightState.BattleEnd)
			GD.Print($"  └─ [状态机] → {FightCenterUtil.StateName(newState)}");

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
	// ─── 各状态处理方法 ───

	private void HandleBattleStart() => TransitionTo(FightState.TurnStart);

	private void HandlePlayerTurn()
	{
		_playerActedThisTurn = false;
		_youActedThisTurn = false;
		FightCenterUtil.ClearActionQueue(MyTurnActs);
		FightCenterUtil.ClearActionQueue(YouTurnActs);

		var pet = FightLandMyStandPet.Instance?.FightPetData;
		string info = pet != null ? $"{pet.PetName} (HP={pet.Hp}/{pet.MaxHp})" : "无精灵";
		GD.Print($"━━━ 第 {_turnNumber} 回合 · 玩家回合 · {info} ━━━");
		LabelGameStatus.SetText($"🧑 第 {_turnNumber} 回合 · 请选择行动\n{info}");
	}

	private void HandleEnemyTurn()
	{
		GD.Print($"  └─ [敌方] AI思考...");
		LabelGameStatus.SetText($"👹 敌方行动中...");
		YouTurnActs[4] = new TurnAction(TurnActionType.Charge, "you");
		_youActedThisTurn = true;
		TryExecute();
	}

	private void HandleExecuteTurn()
	{
		GD.Print($"─────────────────\n  ⚔️ 第 {_turnNumber} 回合执行\n─────────────────");

		// 执行双方行动，接收回合结束效果
		FightExeAction.ExecuteActions(MyTurnActs, YouTurnActs, out var newMyEndActs, out var newYouEndActs);
		MyEndActs = newMyEndActs;
		YouEndActs = newYouEndActs;

		TransitionTo(FightState.CheckFaint);
	}

	private void HandleCheckFaint()
	{
		var playerPet = FightLandMyStandPet.Instance?.FightPetData;
		var enemyPet = FightCenterUtil.GetEnemyActivePet();
		bool playerDead = playerPet != null && playerPet.Hp <= 0;
		bool enemyDead = enemyPet != null && enemyPet.Hp <= 0;

		int alive = 0;
		foreach (var p in PlayerLandMyStandPlayer.Instance.FightPets)
			if (p.Hp > 0) alive++;

		if (alive == 0) { GD.Print("  ❌ 我方全灭！战败！"); LabelGameStatus.SetText("❌ 我方全灭！战败！"); TransitionTo(FightState.BattleEnd); return; }
		if (enemyDead) { GD.Print("  ✅ 敌方全灭！胜利！"); LabelGameStatus.SetText("✅ 敌方全灭！胜利！"); TransitionTo(FightState.BattleEnd); return; }
		if (playerDead) { LabelGameStatus.SetText("💀 我方精灵濒死，请选择替补上场"); EmitSignal(SignalPetFainted, "my", FightCenterUtil.GetCurrentPlayerPetIndex()); TransitionTo(FightState.PlayerSwitch); return; }
		if (enemyDead) { LabelGameStatus.SetText("💀 敌方精灵濒死，敌方准备换宠..."); EmitSignal(SignalPetFainted, "you", 0); TransitionTo(FightState.EnemySwitch); return; }
		NextTurn();
	}

	private void HandlePlayerSwitch()
	{
		GD.Print("  ▶ 请调用 PlayerSwitchAfterFaint(idx)");
		LabelGameStatus.SetText("🔄 请选择替补精灵上场\nPlayerSwitchAfterFaint(idx)");
	}

	private void HandleEnemySwitch()
	{
		GD.Print("  └─ [敌方] 换宠完毕");
		LabelGameStatus.SetText("🔄 敌方换宠完毕");
		NextTurn();
	}

	private void HandleBattleEnd()
	{
		bool win = FightCenterUtil.GetEnemyActivePet()?.Hp <= 0;
		EmitSignal(SignalBattleEnd, win);
		GD.Print($"\n═══════════════════════════════════════\n  🏆 战斗结束! 玩家{(win ? "胜利🎉" : "战败💀")}!\n  ⏱ 共 {_turnNumber} 回合\n═══════════════════════════════════════");
		LabelGameStatus.SetText($"🏁 战斗结束! 玩家{(win ? "胜利 🎉" : "战败 💀")}\n共 {_turnNumber} 回合");
	}

	// ─── 内部辅助 ───

	private void NextTurn() { _turnNumber++; TransitionTo(FightState.TurnStart); }

	private void HandleTurnStart()
	{
		var pet = FightLandMyStandPet.Instance?.FightPetData;
		string info = pet != null ? $"{pet.PetName} (HP={pet.Hp}/{pet.MaxHp})" : "无精灵";

		if (_turnNumber == 0)
		{
			GD.Print($"╔══════════════════════════════════════╗\n║      🌅 第 0 回合 · 系统初始化       ║\n║      {info,-28} ║\n║      ⏳ 战场准备中...                ║\n╚══════════════════════════════════════╝");
			LabelGameStatus.SetText("🌅 战场准备中...");
			_turnNumber = 1;
		}
		else
		{
			GD.Print($"╔══════════════════════════════════════╗\n║      第 {_turnNumber,2} 回合   🌅 回合开始      ║\n║      {info,-28} ║\n╚══════════════════════════════════════╝");
			LabelGameStatus.SetText($"🌅 第 {_turnNumber} 回合开始\n{info}");
		}
		TransitionTo(FightState.PlayerTurn);
	}

	private void DoPlayerSwitch(int idx)
	{
		var pets = PlayerLandMyStandPlayer.Instance.FightPets;
		if (idx >= 0 && idx < pets.Count && pets[idx].Hp > 0)
		{
			GD.Print($"    → 换宠: {pets[idx].PetName}");
			FightLandMyStandPet.Instance?.SwitchPet(pets[idx]);
		}
	}

	private void ClearAllQueues()
	{
		FightCenterUtil.ClearActionQueue(MyTurnActs);
		FightCenterUtil.ClearActionQueue(MyEndActs);
		FightCenterUtil.ClearActionQueue(YouTurnActs);
		FightCenterUtil.ClearActionQueue(YouEndActs);
	}

}