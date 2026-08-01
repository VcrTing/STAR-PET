using Godot;

/// <summary>
/// PP 扣除工具
/// 负责执行回合行动时的 PP 消耗与获取逻辑
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

    /// <summary>
    /// 获取对应方精灵增加 PP（能量）
    /// 根据 side 获取对应的精灵数据，增加 amount 点 PP
    /// </summary>
    /// <param name="side">阵营（我方/敌方）</param>
    /// <param name="amount">要增加的能量值（正数）</param>
    /// <param name="allowOverflow">是否允许溢出上限（默认 false，超过上限时取上限值）</param>
    /// <param name="index">阶段索引号（仅用于日志，默认0）</param>
    public static void GainPp(EnumWho side, int amount, bool allowOverflow = false, int index = 0)
    {
        string sideLabel = side == EnumWho.My ? "🧑我方" : "👹敌方";

        if (amount <= 0)
        {
            GD.Print($"      [{index}] {sideLabel} GainPp | amount={amount}，无需增加 PP");
            return;
        }

        // 获取要加 PP 的精灵
        InsFightPetData pet = side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        if (pet == null)
        {
            GD.Print($"      [{index}] {sideLabel} GainPp | 目标精灵为空，跳过增加 PP");
            return;
        }

        int beforePp = pet.Pp;

        if (allowOverflow)
        {
            // 允许溢出，不设上限
            pet.Pp = pet.Pp + amount;
        }
        else
        {
            // 不允许溢出，取 PP 上限
            int maxPp = side == EnumWho.My
                ? FightGameInit.MaxPpMy
                : FightGameInit.MaxPpYou;
            pet.Pp = Mathf.Min(pet.Pp + amount, maxPp);
        }

        GD.Print($"      [{index}] {sideLabel} GainPp | " +
                 $"PP gain={amount} {pet.PetName} PP: {beforePp} → {pet.Pp} " +
                 $"allowOverflow={allowOverflow}");
    }
}