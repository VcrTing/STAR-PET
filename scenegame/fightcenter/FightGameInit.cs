// ════════════════════════════════════════════════════════════════
//  战斗初始化器（Node2D 单例）
//  职责：专门处理游戏开始时的初始化工作
//  注册到场景树后，_Ready 时自动执行 Init()
//  所有"战斗开始时要做什么"的逻辑都放在这里
//  继承 Node2D 以便使用动画播放器、Tween、音频节点等 Godot 功能
// ════════════════════════════════════════════════════════════════

using Godot;

/// <summary>
/// 战斗初始化器（Node2D，单例）
/// 专门负责游戏开始时的初始化处理
/// 注册到场景树后 _Ready 时自动开始分帧执行初始化
/// 
/// 【分帧执行顺序（每帧一个方法）】
///   1. 加载游戏数据（LoadGameData）
///   2. 加载场景（LoadScene）
///   3. 加载玩家背包（LoadPlayerPack）
///   4. 加载敌方（LoadEnemy）
///   执行完成后自动停止
/// </summary>
public partial class FightGameInit : Node2D
{
	private static FightGameInit _instance;
	public static FightGameInit Instance => _instance;

	/// <summary>当前执行到的步骤索引（0~3），-1 表示未开始</summary>
	private int _initStep = -1;

	/// <summary>标记全部初始化是否已完成，防止 _Process 重复调用 StartBattle</summary>
	private bool _initComplete = false;

	/// <summary>是否为 PVP 模式（默认 true）</summary>
	public bool IsPvp { get; set; } = true;

	/// <summary>我方战斗等级（PVP 时双方等级都取 60）</summary>
	public int MyFightLevel { get; set; } = 60;

	/// <summary>对方战斗等级（PVP 时双方等级都取 60）</summary>
	public int EnemyFightLevel { get; set; } = 60;

	private const int STEP_GAME_DATA = 0;
	private const int STEP_SCENE    = 1;
	private const int STEP_PLAYER   = 2;
	private const int STEP_ENEMY    = 3;
	private const int STEP_DONE     = 4;

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
			case STEP_GAME_DATA: LoadGameData(); break;
			case STEP_SCENE:     LoadScene();    break;
			case STEP_PLAYER:    LoadPlayerPack(); break;
			case STEP_ENEMY:     LoadEnemy();    break;
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

	/// <summary>
	/// 第 1 帧：加载游戏数据
	/// 初始化玩家上阵精灵列表 → 深拷贝为战斗数据 → 切换第一只精灵上场
	/// </summary>
	private void LoadGameData()
	{
		GD.Print("  ── [1/4] 加载游戏数据...");

		PlayerLandMyStandPlayer.Instance?.Init();
		PlayerLandMyStandPlayer.Instance?.InitFight(IsPvp, MyFightLevel);

		var fightPets = PlayerLandMyStandPlayer.Instance?.FightPets;
		if (fightPets != null && fightPets.Count > 0)
		{
			FightLandMyStandPet.Instance?.SwitchPet(fightPets[0]);
		}

		GD.Print("  ✔ 游戏数据加载完成");
	}

	/// <summary>
	/// 第 2 帧：加载场景（打印当前精灵信息 + 刷新技能UI）
	/// </summary>
	private void LoadScene()
	{
		GD.Print("  ── [2/4] 加载场景...");

		var pet = FightLandMyStandPet.Instance?.FightPetData;
		if (pet != null)
		{
			GD.Print($"      当前精灵: {pet.PetName}");
			GD.Print($"      血量: {pet.Hp}/{pet.MaxHp}");
			GD.Print($"      等级: {pet.Level}");
			GD.Print($"      系别: {string.Join(", ", pet.PetTypes)}");
			GD.Print($"      技能数: {pet.FightSkills?.Count ?? 0}");

			if (pet.FightSkills != null && pet.FightSkills.Count > 0)
			{
				UiHBoxSkillsManager.Instance?.SwitchSkills(pet.FightSkills);
				GD.Print($"      技能UI已刷新");
			}
			else
			{
				GD.Print($"      ⚠ 该精灵没有技能!");
			}
		}
		else
		{
			GD.Print($"      ⚠ 场上没有精灵数据!");
		}

		GD.Print("  ✔ 场景加载完成");
	}

	/// <summary>
	/// 第 3 帧：加载玩家背包
	/// 将玩家所有战斗精灵数据同步到背包面板 UI
	/// </summary>
	private void LoadPlayerPack()
	{
		GD.Print("  ── [3/4] 加载玩家背包...");

		var pets = PlayerLandMyStandPlayer.Instance?.FightPets;
		if (pets == null || pets.Count == 0)
		{
			GD.Print("      ⚠ 玩家没有战斗精灵数据，跳过背包同步");
		}
		else
		{
			VBoxFightPlayerPetsPack.Instance?.LoadPlayerPets(pets.ToArray());
			GD.Print($"      玩家背包已同步: {pets.Count} 只精灵");
		}

		GD.Print("  ✔ 玩家背包加载完成");
	}

	/// <summary>
	/// 第 4 帧：加载敌方（预留）
	/// </summary>
	private void LoadEnemy()
	{
		GD.Print("  ── [4/4] 加载敌方...");
		// TODO: 加载敌方数据
		GD.Print("  ✔ 敌方加载完成（预留）");
	}
}
