// ════════════════════════════════════════════════════════════════
//  战斗中心管理器（单例）
//  职责：控制战斗进程状态机
//  状态流：None → BattleStart → TurnStart → PlayerTurn → YouTurn →
//          ExecuteTurn → (有死亡→DoingDie换宠→...) 
//                       → (无死亡→TurnStart) → ... → BattleEnd
//  ⚠ PlayerTurn 即包含正常选行动也包含濒死后强制换宠
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
	public TurnAction[] YouTurnActs { get; private set; } = new TurnAction[9];

	private bool _playerActedThisTurn = false;
	private bool _youActedThisTurn = false;

	/// <summary>玩家当前上场精灵濒死，需强制选择替补上场</summary>
	private bool _needPlayerFaintSwitch = false;

	/// <summary>是否处于强制换宠状态（My 精灵濒死需强制选择替补）</summary>
	public bool NeedPlayerFaintSwitch => _needPlayerFaintSwitch;

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

	/// <summary>玩家是否可以正常行动（选技能）</summary>
	public bool CanPlayerAct()
	{
		if (_needPlayerFaintSwitch) return false;
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
		if (_needPlayerFaintSwitch) { GD.Print($"  ⚠ 当前需要换宠，不能选择技能"); return; }
		if (_currentState != FightState.PlayerTurn) { GD.Print($"  ⚠ 当前不是玩家回合"); return; }
		if (fightSkill?.Skill == null) { GD.Print($"  ⚠ 技能数据无效"); return; }

		MyTurnActs[4] = new TurnAction(EnumWho.My, fightSkill);
		_playerActedThisTurn = true;

		GD.Print($"  └─ [玩家] 选择技能【{fightSkill.Skill.SkillName}】(先手={fightSkill.Skill.Priority}) → 等待敌方...");

		// 检查Pve
		PveRunning();
		// 检查下一步
		TryExecute();
	}

	public void PlayerSelectSwitch(int targetIndex)
	{
		var allPets = PlayerLandMyStandPlayer.Instance.FightPets;
		if (targetIndex < 0 || targetIndex >= allPets.Count || allPets[targetIndex].Hp <= 0)
		{
			GD.Print($"  ⚠ 目标无效");
			return;
		}

		// ── 濒死强制换宠 ──
		if (_needPlayerFaintSwitch)
		{
			// 先解除强制换宠状态，再执行换宠（SwitchPet 内部会调用 Pan.Close()，
			// 若此时仍处于强制换宠状态，Close 会被拦截无法关闭）
			_needPlayerFaintSwitch = false;
			DoPlayerSwitch(targetIndex);
			GD.Print($"  └─ [玩家] 濒死后换宠 → {allPets[targetIndex].PetName}");

			// 把换宠作为玩家行动记录（加载系统换宠技能 0_4_1），
			// 确保状态机正常流转（TryExecute 能检测玩家已行动）
			InsSkill faintSwitchSkill = DevSkillLoadTool.LoadSwitchPetSkill();
			if (faintSwitchSkill != null)
			{
				var faintFightSkill = InsFightSkill.FromInsSkill(faintSwitchSkill);
				var faintAction = new TurnAction(EnumWho.My, faintFightSkill);
				faintAction.SwitchTargetIndex = targetIndex;
				MyTurnActs[4] = faintAction;
				_playerActedThisTurn = true;
			}

			// 换宠成功后，直接进入新回合（TurnStart）
			TransitionTo(FightState.TurnStart);
			return;
		}

		// ── 正常情况：玩家自愿换宠（作为行动） ──
		if (_currentState != FightState.PlayerTurn) { GD.Print($"  ⚠ 当前不是玩家回合"); return; }

		// 加载系统换宠技能 0_4_1 并作为 UseSkill 行动
		InsSkill switchSkill = DevSkillLoadTool.LoadSwitchPetSkill();
		if (switchSkill == null)
		{
			GD.PrintErr($"  ⚠ 加载换宠技能失败");
			return;
		}
		var fightSkill = InsFightSkill.FromInsSkill(switchSkill);
		var action = new TurnAction(EnumWho.My, fightSkill);
		action.SwitchTargetIndex = targetIndex;
		MyTurnActs[4] = action;
		_playerActedThisTurn = true;
		GD.Print($"  └─ [玩家] 换宠 Index={targetIndex} ({allPets[targetIndex].PetName}) → 使用技能【{switchSkill.SkillName}】等待敌方...");

		// 检查Pve
		PveRunning();
		// 检查下一步
		TryExecute();
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
			case FightState.YouTurn:      HandleEnemyTurn();    break;
			case FightState.ExecuteTurn:  HandleExecuteTurn();  break;
			case FightState.DoingDie:     HandleDoingDie();     break;
			case FightState.BattleEnd:    HandleBattleEnd();    break;
		}
	}
	// ─── 各状态处理方法 ───

	private void HandleBattleStart() => TransitionTo(FightState.TurnStart);

	private void HandlePlayerTurn()
	{
		// ── 濒死换宠模式：不重置标记，仅提示玩家选择替补 ──
		if (_needPlayerFaintSwitch)
		{
			LabelGameStatus.SetText("💀 我方精灵濒死，请选择替补上场\nPlayerSelectSwitch(idx)");
			GD.Print("  ▶ 我方精灵濒死，请调用 PlayerSelectSwitch(idx) 选择替补");
				
			// 回合开始时先更新 UI 显示
			FightUiUpdateTool.UpdateMyUi();
			FightUiUpdateTool.UpdateYouUi();

			// 打开切换宠物的 Pan（强制换宠状态，不允许关闭）
			PanFightPlayerPack.Instance?.OpenForLimit(PlayerLandMyStandPlayer.Instance.GetCanSiwtchFightPets(false));
			return;
		}

		// ── 正常回合 ──
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
		// 如果敌人因濒死需要换宠，在这里做自动换宠
		// （暂未实现敌方多精灵，后续扩展）

		GD.Print($"  └─ [敌方] AI思考...");
		LabelGameStatus.SetText($"👹 敌方行动中...");
		YouTurnActs[4] = new TurnAction(TurnActionType.Charge, EnumWho.You);
		_youActedThisTurn = true;
		TryExecute();
	}

	private void HandleExecuteTurn()
	{
		GD.Print($"─────────────────\n  ⚔️ 第 {_turnNumber} 回合执行\n─────────────────");

		// 防御校验：双方场上精灵数据必须完整，否则跳过本回合执行，
		// 避免精灵死亡/换宠流程中残留行动标记导致二次执行时访问 null
		if (FightLandMyStandPet.Instance?.FightPetData == null ||
			FightLandYouStandPet.Instance?.FightPetData == null)
		{
			GD.Print("  ⚠ [HandleExecuteTurn] 场上精灵数据不完整，跳过本回合执行");
			NextTurn();
			return;
		}

		// 执行双方行动，获取本回合死亡精灵列表
		var newDiePets = FightExeAction.ExecuteActions(MyTurnActs, YouTurnActs);

		if (newDiePets.Count > 0)
		{
			// 有精灵死亡 → 进入死亡检查流程
			GD.Print($"  💀 [HandleExecuteTurn] 本回合有 {newDiePets.Count} 只精灵死亡，进入 DoingDie");
			TransitionTo(FightState.DoingDie);
		}
		else
		{
			// 无精灵死亡 → 直接进入下一回合
			GD.Print("  ✅ [HandleExecuteTurn] 本回合无精灵死亡，直接进入下一回合");
			NextTurn();
		}
	}

	private void HandleDoingDie()
	{
		GD.Print($"  💀 [DoingDie] 处理死亡：我方死亡 {FightAliveHouse.MyDiePets.Count} 只，敌方死亡 {FightAliveHouse.YouDiePets.Count} 只");

		// ── 胜负判定：根据 AliveHouse 的心数判断 ──
		if (!FightAliveHouse.Alive(EnumWho.My))
		{
			GD.Print("  ❌ 我方全灭！战败！");
			LabelGameStatus.SetText("❌ 我方全灭！战败！");
			TransitionTo(FightState.BattleEnd);
			return;
		}
		if (!FightAliveHouse.Alive(EnumWho.You))
		{
			GD.Print("  ✅ 敌方全灭！胜利！");
			LabelGameStatus.SetText("✅ 敌方全灭！胜利！");
			TransitionTo(FightState.BattleEnd);
			return;
		}

		// ── 我方精灵死亡 → 触发换宠信号 ──
		if (FightAliveHouse.MyDiePets.Count > 0)
		{
			var myPet = FightLandMyStandPet.Instance?.FightPetData;
			if (myPet == null || myPet.Hp <= 0)
			{
				GD.Print("  💀 [DoingDie] 我方精灵死亡，请求玩家换宠");
				EmitSignal(SignalPetFainted, EnumWho.My.ToString(), FightCenterUtil.GetCurrentPlayerPetIndex());
				_needPlayerFaintSwitch = true;
				// 重置双方行动标记，避免上一回合残留的 _playerActedThisTurn=true
				// 导致 TryExecute 在换宠流程中误判"双方已行动"而二次触发回合执行
				_playerActedThisTurn = false;
				_youActedThisTurn = false;
				TransitionTo(FightState.PlayerTurn);
				return;
			}
		}

		// ── 敌方精灵死亡 → PVE 自动换宠 ──
		if (FightAliveHouse.YouDiePets.Count > 0)
		{
			if (FightGameInit.Instance != null && !FightGameInit.Instance.IsPvp)
			{
				var nextPet = FightPveRunner.RunPveWhenPreDie();
				if (nextPet != null)
				{
					GD.Print($"  💀 [DoingDie] 敌方换宠 → {nextPet.PetName}");
					FightLandYouStandPet.Instance?.SwitchPet(nextPet);
				}
			}
			else
			{
				GD.Print("  💀 [DoingDie] PVP 敌方换宠暂未实现");
			}
		}

		// 死亡处理完毕，进入下一回合
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

		// 刷新技能
		FightSkillAsyncTool.SyncAllSkills(EnumWho.My, FightLandMyStandPet.Instance.FightPetData);
		FightSkillAsyncTool.SyncAllSkills(EnumWho.You, FightLandYouStandPet.Instance.FightPetData);

		// 回合开始时先更新 UI 显示
		FightUiUpdateTool.UpdateMyUi();
		FightUiUpdateTool.UpdateYouUi();
		
		var pet = FightLandMyStandPet.Instance?.FightPetData;
		string info = pet != null ? $"{pet.PetName} (HP={pet.Hp}/{pet.MaxHp})" : "无精灵";

		if (_turnNumber == 0)
		{
			GD.Print("╔══════════════════════════════════════╗\n" +
			         "║      🌅 第 0 回合 · 系统初始化        ║\n" +
			        $"║  {info,-34}║\n" +
			         "║       ⏳ 战场准备中...                 ║\n" +
			         "╚══════════════════════════════════════╝");
			LabelGameStatus.SetText("🌅 战场准备中...");
			_turnNumber = 1;
		}
		else
		{
			GD.Print("╔══════════════════════════════════════╗\n" +
			        $"║{DispCenter($"第 {_turnNumber} 回合  🌅 回合开始", 36)}║\n" +
			        $"║{DispCenter(info, 36)}║\n" +
			         "╚══════════════════════════════════════╝");
			LabelGameStatus.SetText($"🌅 第 {_turnNumber} 回合开始\n{info}");
		}

		TransitionTo(FightState.PlayerTurn);
	}

	/// <summary>
	/// 中英混排居中辅助方法（中文按2个英文字符宽度计算）
	/// </summary>
	private static string DispCenter(string text, int totalWidth)
	{
		int len = 0;
		foreach (char c in text)
			len += (c > 127) ? 2 : 1;
		int pad = totalWidth - len;
		if (pad <= 0) return text;
		int left = pad / 2;
		return new string(' ', left) + text + new string(' ', pad - left);
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
		FightCenterUtil.ClearActionQueue(YouTurnActs);
	}
}