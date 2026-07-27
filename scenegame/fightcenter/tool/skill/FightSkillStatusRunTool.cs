using Godot;

/// <summary>
/// 技能状态运行工具
/// 提供状态技能执行时 Buff 生成、判定等可复用的方法
/// </summary>
public static class FightSkillStatusRunTool
{
    /// <summary>
    /// 执行状态技能的 Buff 生成逻辑
    /// 根据传入的 buffSource 数组生成 InsFightBuff 并添加到对应阵营的 BuffManager
    /// </summary>
    /// <param name="run">战斗运行实例</param>
    /// <param name="index">索引</param>
    /// <param name="buffSource">Buff 源数组（由调用方决定使用 GainBuff 还是 GainBuffBingo）</param>
    public static void ExecuteStatusBuff(FightRunning run, int index, Godot.Collections.Array buffSource)
    {
        if (buffSource == null || buffSource.Count <= 0)
            return;

        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";

        // 获取 Buff 所属的精灵 UUID
        string petUuid = run.Side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData?.PetUuid
            : FightLandYouStandPet.Instance?.FightPetData?.PetUuid;

        var buffs = DevBuffTool.CreateFromArray(buffSource);
        if (buffs != null && buffs.Count > 0)
        {
            // 为每个 Buff 设置所属精灵 UUID
            if (!string.IsNullOrWhiteSpace(petUuid))
            {
                foreach (var buff in buffs)
                {
                    buff.PetUuid = petUuid;
                }
            }

            // 根据 Side 判断保存到哪个 BuffManager
            if (run.Side == EnumWho.My)
            {
                FightMyStandBuffManager.Instance?.AddBuffs(buffs.ToArray());
            }

            EnumWho targetSide = run.Side == EnumWho.My ? EnumWho.You : EnumWho.My;
            GD.Print($"      [{index}] {sideLabel} {run.RunningType} | " +
                     $"生成 InsFightBuff {buffs.Count} 个 → 目标:{targetSide}");
        }
    }

    /// <summary>
    /// 执行聚能效果：根据 side 判定是哪一方的精灵，从 FightGameInit 读取补充量补充能量（Pp）
    /// </summary>
    public static void ExecuteFocusEnergy(int index, FightRunning run)
    {
        string sideLabel = run.Side == EnumWho.My ? "我方" : "敌方";

        int gainAmount = run.Side == EnumWho.My
            ? FightGameInit.Instance.GainPpMy
            : FightGameInit.Instance.GainPpYou;

        InsFightPetData petData = run.Side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        if (petData != null)
        {
            int beforePp = petData.Pp;
            petData.Pp += gainAmount;
            GD.Print($"      [{index}] ExecuteFocusEnergy | 聚能: [{sideLabel}] Pp {beforePp} → {petData.Pp}（+{gainAmount}）");
        }
        else
        {
            GD.Print($"      [{index}] ExecuteFocusEnergy | 警告：未找到 [{sideLabel}] 战斗精灵数据，无法补充 Pp");
        }
    }
}
