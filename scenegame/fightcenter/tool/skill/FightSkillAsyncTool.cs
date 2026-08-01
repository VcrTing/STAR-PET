using Godot;

/// <summary>
/// 技能同步工具（Async = RealtimeSync 实时同步）
/// 负责调用 DuckSkillLoader.ExecuteRealtimeSync 执行技能实现类的 RealtimeSync 鸭子方法
/// </summary>
public static class FightSkillAsyncTool
{
    /// <summary>
    /// 实时同步单个技能
    /// 调用 DuckSkillLoader.ExecuteRealtimeSync 执行指定技能实现类的 RealtimeSync 方法
    /// 参数全部由外部传入
    /// </summary>
    /// <param name="implCsFilePath">技能实现脚本路径（res://define/dataskill/.../DuckSkillXxx.cs）</param>
    /// <param name="side">阵营</param>
    /// <param name="myPet">我方场上当前精灵</param>
    /// <param name="youPet">敌方场上当前精灵</param>
    /// <param name="myPackPet">我方背包精灵数组</param>
    /// <param name="youPackPet">敌方背包精灵数组</param>
    /// <param name="sideSkill">当前技能实例（通过它修改技能源头）</param>
    public static void SyncSkill(string implCsFilePath, EnumWho side,
        InsFightPetData myPet, InsFightPetData youPet,
        InsFightPetData[] myPackPet, InsFightPetData[] youPackPet,
        InsFightSkill sideSkill)
    {
        if (sideSkill?.Skill == null)
        {
            GD.PrintErr("[FightSkillAsyncTool.SyncSkill] sideSkill 或 Skill 为空");
            return;
        }

        DuckSkillLoader.ExecuteRealtimeSync(implCsFilePath, side,
            myPet, youPet, myPackPet, youPackPet, sideSkill);
    }

    /// <summary>
    /// 实时同步精灵的全部技能
    /// 遍历 ownerPet 的 FightSkills 列表，对每个技能调用 RealtimeSync
    /// myPet/youPet/myPackPet/youPackPet 根据 side 自动从单例获取，无需外部传入
    /// </summary>
    /// <param name="side">阵营（根据 side 自动获取对应方场上/背包精灵数据）</param>
    /// <param name="ownerPet">技能所属精灵（取其全部技能逐一同步）</param>
    public static void SyncAllSkills(EnumWho side, InsFightPetData ownerPet)
    {
        if (ownerPet?.FightSkills == null)
        {
            GD.PrintErr("[FightSkillAsyncTool.SyncAllSkills] ownerPet 或 FightSkills 为空");
            return;
        }

        // 根据 side 自动获取场上当前精灵与背包精灵数组
        InsFightPetData myPet = side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;
        InsFightPetData youPet = side == EnumWho.My
            ? FightLandYouStandPet.Instance?.FightPetData
            : FightLandMyStandPet.Instance?.FightPetData;

        InsFightPetData[] myPackPet = (side == EnumWho.My
            ? PlayerLandMyStandPlayer.Instance?.FightPets
            : PlayerLandYouStandPlayer.Instance?.FightPets)?.ToArray() ?? new InsFightPetData[0];
        InsFightPetData[] youPackPet = (side == EnumWho.My
            ? PlayerLandYouStandPlayer.Instance?.FightPets
            : PlayerLandMyStandPlayer.Instance?.FightPets)?.ToArray() ?? new InsFightPetData[0];

        foreach (var fightSkill in ownerPet.FightSkills)
        {
            if (fightSkill?.Skill == null)
                continue;

            SyncSkill(fightSkill.ImplClass, side,
                myPet, youPet, myPackPet, youPackPet, fightSkill);
        }
    }
}