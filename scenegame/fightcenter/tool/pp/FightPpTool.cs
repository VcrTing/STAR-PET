using Godot;

/// <summary>
/// PP 扣除工具
/// 负责执行回合行动时的 PP 消耗逻辑
/// 在 ExecuteDoAttack / ExecuteDoDefense / ExecuteDoStatus 执行前调用
/// </summary>
public static class FightPpTool
{
    /// <summary>
    /// 扣除对应方当前精灵的 PP
    /// 根据 run.Side 获取对应的精灵数据，扣除 run.SideFightSkill.ActualPpCost 点 PP
    /// </summary>
    /// <param name="run">当前执行的回合运行数据</param>
    /// <param name="index">阶段索引号（仅用于日志）</param>
    public static void DeductPp(FightRunning run, int index)
    {
        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";

        if (run.SideFightSkill == null)
        {
            GD.Print($"      [{index}] {sideLabel} {run.RunningType} | SideFightSkill 为空，跳过 PP 扣除");
            return;
        }

        int ppCost = run.SideFightSkill.ActualPpCost;
        if (ppCost <= 0)
        {
            GD.Print($"      [{index}] {sideLabel} {run.RunningType} | ActualPpCost={ppCost}，无需扣除 PP");
            return;
        }

        // 获取要扣 PP 的精灵
        InsFightPetData pet = run.Side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        if (pet == null)
        {
            GD.Print($"      [{index}] {sideLabel} {run.RunningType} | 目标精灵为空，跳过 PP 扣除");
            return;
        }

        int beforePp = pet.Pp;
        pet.Pp = Mathf.Max(pet.Pp - ppCost, 0);

        GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                 $"PP cost={ppCost} {pet.PetName} PP: {beforePp} → {pet.Pp} " +
                 $"skill={run.SideFightSkill.Skill?.SkillName ?? "null"}");

    }
}