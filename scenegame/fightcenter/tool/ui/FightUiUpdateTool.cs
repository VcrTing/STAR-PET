using Godot;

/// <summary>
/// 战斗 UI 更新工具
/// 提供更新我方/敌方 UI 显示的方法
/// PP 扣除后调用，刷新 Ui 上展示的 PP 数值
/// </summary>
public static class FightUiUpdateTool
{
    /// <summary>
    /// 更新我方 UI
    /// 刷新顶部栏精灵信息 + 技能栏 + 背包列表
    /// </summary>
    public static void UpdateMyUi()
    {
        var myPet = FightLandMyStandPet.Instance?.FightPetData;

        // 1. 刷新顶部栏精灵信息（HP/MaxHp）
        UiHTopBarPetInfoMy.Instance?.UpdatePetInfo(myPet);

        // 2. 刷新技能栏：更新当前精灵的技能按钮显示（PP 消耗数值等）
        if (myPet?.FightSkills != null && myPet.FightSkills.Count > 0)
        {
            UiHBoxSkillsManager.Instance?.UpdateSkills(myPet.FightSkills);
        }

        // 3. 刷新背包列表：所有精灵的 PP/HP 显示
        var fightPets = PlayerLandMyStandPlayer.Instance?.FightPets;
        if (fightPets != null && fightPets.Count > 0)
        {
            VBoxFightPlayerPetsPack.Instance?.LoadPlayerPets(fightPets.ToArray());
        }
    }

    /// <summary>
    /// 更新敌方 UI
    /// 刷新敌方顶部栏精灵信息（名称、等级、HP）
    /// </summary>
    public static void UpdateYouUi()
    {
        var youPet = FightLandYouStandPet.Instance?.FightPetData;
        UiHTopBarPetInfoYou.Instance?.UpdatePetInfo(youPet);
    }
}