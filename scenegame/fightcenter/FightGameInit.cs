// ════════════════════════════════════════════════════════════════
//  战斗初始化器（Node2D 单例）
// ════════════════════════════════════════════════════════════════

using Godot;

public partial class FightGameInit : Node2D
{
	private static FightGameInit _instance;
	public static FightGameInit Instance => _instance;

	private int _initStep = -1;
	private bool _initComplete = false;

	public bool IsPvp { get; set; } = false;
	public bool IsBalanceMode { get; set; } = true;
	public int MyFightLevel { get; set; } = 60;
	public int YouFightLevel { get; set; } = 60;

	private const int STEP_INIT     = 0;
	private const int STEP_MY_DATA  = 1;
	private const int STEP_YOU_DATA = 2;
	private const int STEP_SCENE    = 3;
	private const int STEP_MY       = 4;
	private const int STEP_YOU      = 5;
	private const int STEP_DONE     = 6;

	public override void _EnterTree()
	{
		if (_instance != null) { QueueFree(); return; }
		_instance = this;
	}

	public override void _Ready()
	{
		GD.Print("  └─ [FightGameInit] 开始分帧初始化...");
		_initStep = 0;
		SetProcess(true);
	}

	public override void _Process(double delta)
	{
		if (_initStep < 0 || _initStep >= STEP_DONE || _initComplete) return;

		switch (_initStep)
		{
			case STEP_INIT:     Init();        break;
			case STEP_MY_DATA:  LoadMyData();  break;
			case STEP_YOU_DATA: LoadYouData(); break;
			case STEP_SCENE:    LoadScene();   break;
			case STEP_MY:       LoadMy();      break;
			case STEP_YOU:      LoadYou();     break;
		}

		_initStep++;
		if (_initStep >= STEP_DONE)
		{
			_initComplete = true;
			SetProcess(false);
			GD.Print("  └─ [FightGameInit] 所有初始化完成 → 启动 StartBattle()");
			FightCenterManger.Instance?.StartBattle();
		}
	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}

	private void Init()
	{
		GD.Print("  ── [0/6] 全局初始化...");
		InitPvpMode();
		GD.Print("  ✔ 全局初始化完成");
	}

	private void InitPvpMode()
	{
		IsBalanceMode = true;
		MyFightLevel = 60;
		YouFightLevel = 60;
	}

	private void LoadMyData()
	{
		GD.Print("  ── [1/6] 加载我方数据...");
		PlayerLandMyStandPlayer.Instance?.Init();
		PlayerLandMyStandPlayer.Instance?.InitFight(IsBalanceMode, MyFightLevel);
		var fightPets = PlayerLandMyStandPlayer.Instance?.FightPets;
		if (fightPets != null && fightPets.Count > 0)
			FightLandMyStandPet.Instance?.SwitchPet(fightPets[0]);
		GD.Print("  ✔ 我方数据加载完成");
	}

	private void LoadYouData()
	{
		GD.Print("  ── [2/6] 加载敌方数据...");
		PlayerLandYouStandPlayer.Instance?.Init();
		PlayerLandYouStandPlayer.Instance?.InitFight(IsBalanceMode, YouFightLevel);
		var fightPets = PlayerLandYouStandPlayer.Instance?.FightPets;
		if (fightPets != null && fightPets.Count > 0)
			FightLandYouStandPet.Instance?.SwitchPet(fightPets[0]);
		GD.Print("  ✔ 敌方数据加载完成");
	}

	private void LoadScene()
	{
		GD.Print("  ── [3/6] 加载场景...");
	}

	private void LoadMy()
	{
		GD.Print("  ── [4/6] 加载我方背包...");
		var pets = PlayerLandMyStandPlayer.Instance?.FightPets;
		if (pets == null || pets.Count == 0)
			GD.Print("      ⚠ 玩家没有战斗精灵数据，跳过背包同步");
		else
		{
			VBoxFightPlayerPetsPack.Instance?.LoadPlayerPets(pets.ToArray());
			GD.Print($"      玩家背包已同步: {pets.Count} 只精灵");
		}
	}

	private void LoadYou()
	{
		GD.Print("  ── [5/6] 加载敌方");
	}
}