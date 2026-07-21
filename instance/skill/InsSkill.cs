using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 技能动态数据（战斗中的技能实例）
/// </summary>
public class InsSkill
{
	/// <summary>
	/// 技能唯一标识符，格式：{petType}_{skillType}_{skillCode}
	/// 例如 "0_1_1" = 普通系攻击拍击
	/// </summary>
	public string SkillId;                                       // 技能唯一标识符
	public int PetType;                                          // 系别（对应 EnumPetType）
	public int SkillType;                                        // 技能类型（1=攻击,2=防御,3=状态）
	public int SkillCode;                                        // 技能编号
	public string SkillName;                                     // 技能名
	public int AttackValue;                                      // 攻击数值/威力
	public int AttackType;                                       // 攻击类型（2=物攻,3=魔攻,0=固伤）
	public int PpCost;                                           // PP能耗
	public string IconPath;                                      // 图标路径
	public float HitRate;                                        // 命中率
	public int Priority;                                         // 先手值
	public int HiddenPriority;                                   // 隐藏先手判断
	public float InstantKillRate;                                // 秒杀概率
	public int TurnEndSpecialId;                                 // 回合结束特殊处理ID
	public int BeforeActionSpecialId;                            // 释放前特殊处理ID
	public int BingoSkillType;                                   // 应对类型（1=应对攻击, 0=无应对）
	public int GainEnergy;                                       // 获得能量
	public int GainHp;                                           // 获得血量
	public Godot.Collections.Array GainIv = new();               // 获得个体值量
	public Godot.Collections.Array Marks = new();                 // 印记
	public Godot.Collections.Array StatusEffects = new();        // 异常状态
	public Godot.Collections.Array SoundEffects = new();         // 音效
	public Godot.Collections.Array ParticleEffects = new();      // 特效
	public Godot.Collections.Array PetActions = new();           // 宠物动作
	public Godot.Collections.Array MainDescription = new();      // 主描述
	public Godot.Collections.Array AuxiliaryDescription = new(); // 辅助描述

	/// <summary>
	/// 从资源文件加载技能数据
	/// </summary>
	public static InsSkill FromResource(Resource res)
	{
		if (res == null) return null;

		int petType = res.Get("pet_type").AsInt32();
		int skillType = res.Get("skill_type").AsInt32();
		int skillCode = res.Get("skill_code").AsInt32();

		return new InsSkill
		{
			SkillId = $"{petType}_{skillType}_{skillCode}",
			PetType = petType,
			SkillType = res.Get("skill_type").AsInt32(),
			SkillCode = res.Get("skill_code").AsInt32(),
			SkillName = res.Get("skill_name").AsString(),
			AttackValue = res.Get("attack_value").AsInt32(),
			AttackType = res.Get("attack_type").AsInt32(),
			PpCost = res.Get("pp_cost").AsInt32(),
			IconPath = res.Get("icon_path").AsString(),
			HitRate = (float)res.Get("hit_rate").AsDouble(),
			Priority = res.Get("priority").AsInt32(),
			HiddenPriority = res.Get("hidden_priority").AsInt32(),
			InstantKillRate = (float)res.Get("instant_kill_rate").AsDouble(),
			TurnEndSpecialId = res.Get("turn_end_special_id").AsInt32(),
			BeforeActionSpecialId = res.Get("before_action_special_id").AsInt32(),
			BingoSkillType = res.Get("bingo_skill_type").AsInt32(),
			GainEnergy = res.Get("gain_energy").AsInt32(),
			GainHp = res.Get("gain_hp").AsInt32(),
			GainIv = res.Get("gain_iv").AsGodotArray(),
			Marks = res.Get("marks").AsGodotArray(),
			StatusEffects = res.Get("status_effects").AsGodotArray(),
			SoundEffects = res.Get("sound_effects").AsGodotArray(),
			ParticleEffects = res.Get("particle_effects").AsGodotArray(),
			PetActions = res.Get("pet_actions").AsGodotArray(),
			MainDescription = res.Get("main_description").AsGodotArray(),
			AuxiliaryDescription = res.Get("auxiliary_description").AsGodotArray()
		};
	}
}