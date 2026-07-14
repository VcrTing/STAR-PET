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
/// 注册到场景树后 _Ready 时自动执行 Init()
/// 
/// 【职责范围】
///   - 显示当前精灵信息
///   - 刷新技能 UI
///   - 播放入场动画（TODO）
///   - 显示"上吧！XX！"文字（TODO）
///   - 其他战斗开始时要做的初始化逻辑
/// </summary>
public partial class FightGameInit : Node2D
{
	private static FightGameInit _instance;
	public static FightGameInit Instance => _instance;

	public override void _EnterTree()
	{
		if (_instance != null) { QueueFree(); return; }
		_instance = this;
	}

	public override void _Ready()
	{
		// 节点就绪后自动执行战斗初始化
		Init();
	}

	public override void _ExitTree()
	{
		if (_instance == this) _instance = null;
	}

	/// <summary>
	/// 战斗初始化入口
	/// 由 _Ready 自动调用，执行所有开场前的准备工作
	/// 
	/// 当前实现：
	///   1. 获取当前上场精灵数据
	///   2. 打印精灵基本信息到控制台
	///   3. 刷新技能栏 UI
	///
	/// TODO 后续扩展：
	///   - 播放开场动画（可用 AnimationPlayer）
	///   - 显示"上吧！XX！"文本（可用 Label）
	///   - 精灵入场特效（可用 Tween）
	///   - 场地效果加载
	///   - BGM 切换（可用 AudioStreamPlayer）
	/// </summary>
	public void Init()
	{
		GD.Print("  └─ [FightGameInit.Init] 战斗初始化开始...");

		// 1. 获取当前上场精灵的战斗数据
		var pet = FightLandMyStandPet.Instance?.FightPetData;

		if (pet != null)
		{
			// 2. 打印精灵基本信息
			GD.Print($"      当前精灵: {pet.PetName}");
			GD.Print($"      血量: {pet.Hp}/{pet.MaxHp}");
			GD.Print($"      等级: {pet.Level}");
			GD.Print($"      系别: {string.Join(", ", pet.PetTypes)}");
			GD.Print($"      技能数: {pet.FightSkills?.Count ?? 0}");

			// 3. 刷新技能栏 UI（显示该精灵的可使用技能）
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

		// TODO: 入场动画（可用 AnimationPlayer 播放）
		// TODO: "上吧！{PetName}！" 文本显示
		// TODO: 场地效果
		// TODO: BGM 切换

		// 战场初始化完成 → 启动战斗状态机
		GD.Print("  └─ [FightGameInit.Init] 战场加载完成 → 启动 StartBattle()");
		FightCenterManger.Instance?.StartBattle();
	}
}
