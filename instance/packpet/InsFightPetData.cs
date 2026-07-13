using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 战斗中的精灵动态数据
/// 与 InsPackPetData 结构一致，是背包数据在战斗场景中的镜像副本
/// </summary>
public class InsFightPetData
{
	public string PetUuid;                                        // 宠物UUID（唯一键，标识背包中每只精灵实例）
	public string PetId;                                          // 精灵静态ID（对应 datapet/ 下的文件）
	public string PetName;                                        // 真实名称（来自物种数据，如 "零仔"）
	public List<EnumPetType> PetTypes = new();                    // 精灵系别数组（第一个元素为主系别，可双属性如金+龙）
	public string Nickname;                                       // 昵称（玩家自定义，默认等于 PetName）
	public int Level;                                             // 等级
	public int Exp;                                               // 当前经验
	public int Hp;                                                // 当前生命值
	public int MaxHp;                                             // 最大生命值
	public Dictionary<EnumPetBaseStats, int> Iv = new();          // 个体值字典（stat -> value）
	public Dictionary<EnumPetBaseStats, int> Talent = new();      // 天赋值字典（stat -> talent，范围0-10）
	public List<string> Skills = new();                           // 已学习的技能ID列表
	public List<string> CarriedSkills = new();                    // 当前携带/装备的技能ID数组（Skills的子集，最多4个）
	public EnumPetNature Nature;                                  // 性格
	public EnumPetGender Gender;                                  // 性别
	public int BallType;                                          // 捕获用精灵球类型
	public bool IsShiny;                                          // 是否闪光
	public int HatchCounter;                                      // 孵化计数（0=已孵化）
	public int Intimacy;                                          // 亲密度值
	public bool IsLocked;                                         // 是否锁定（锁定后不可放生/交易）
	public bool IsSpecial;                                        // 是否特殊精灵（如古龙残魂、活动限定等特殊身份标识）
	public string ObtainedDate;                                   // 获得日期（如 "2026-07-12"）
	public string ObtainedMethod;                                 // 获得方式（如 "捕捉", "孵化", "交换", "活动"）
	public string ObtainedLocation;                               // 获得地点（如 "风眼省", "洛克里安省"）
	public List<EnumPetMedal> Medals = new();                     // 获得的徽章列表
	public Dictionary<EnumPetBaseStats, int> FinalStats = new();   // 最终个体值（基础Iv + 等级修正 + 天赋修正后 × 性格修正的最终值）

	/// <summary>
	/// 从 InsPackPetData 转换为 InsFightPetData（深拷贝）
	/// </summary>
	/// <param name="packData">背包中的精灵数据</param>
	/// <returns>战斗中的精灵数据副本</returns>
	public static InsFightPetData FromPackData(InsPackPetData packData)
	{
		if (packData == null)
			return null;

		return new InsFightPetData
		{
			PetUuid = packData.PetUuid,
			PetId = packData.PetId,
			PetName = packData.PetName,
			PetTypes = new List<EnumPetType>(packData.PetTypes),
			Nickname = packData.Nickname,
			Level = packData.Level,
			Exp = packData.Exp,
			Hp = packData.Hp,
			MaxHp = packData.MaxHp,
			Iv = new Dictionary<EnumPetBaseStats, int>(packData.Iv),
			Talent = new Dictionary<EnumPetBaseStats, int>(packData.Talent),
			Skills = new List<string>(packData.Skills),
			CarriedSkills = new List<string>(packData.CarriedSkills),
			Nature = packData.Nature,
			Gender = packData.Gender,
			BallType = packData.BallType,
			IsShiny = packData.IsShiny,
			HatchCounter = packData.HatchCounter,
			Intimacy = packData.Intimacy,
			IsLocked = packData.IsLocked,
			IsSpecial = packData.IsSpecial,
			ObtainedDate = packData.ObtainedDate,
			ObtainedMethod = packData.ObtainedMethod,
			ObtainedLocation = packData.ObtainedLocation,
			Medals = new List<EnumPetMedal>(packData.Medals),
			FinalStats = new Dictionary<EnumPetBaseStats, int>(packData.FinalStats)
		};
	}
}