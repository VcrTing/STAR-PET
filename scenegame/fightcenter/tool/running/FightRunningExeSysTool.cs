using Godot;

/// <summary>
/// 回合运行执行系统工具
/// 提供系统类阶段（如切换宠物）的可复用执行方法
/// </summary>
public static class FightRunningExeSysTool
{
    /// <summary>
    /// 执行主动切换宠物阶段
    /// </summary>
    public static void ExecuteSwitchPet(FightRunning run, int index)
    {
        if (run.SwitchPet == null) return;

        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";
        GD.Print($"      [{index}] {sideLabel} 切换宠物 → {run.SwitchPet.PetName}");

        if (run.Side == EnumWho.My)
            FightLandMyStandPet.Instance?.SwitchPet(run.SwitchPet);
        else
            FightLandYouStandPet.Instance?.SwitchPet(run.SwitchPet);
    }
}