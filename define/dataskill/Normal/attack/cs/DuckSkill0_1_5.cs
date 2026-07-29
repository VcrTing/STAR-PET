using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class DuckSkill0_1_5 : Resource
{
    /// <summary>
    /// 冲撞技能实现
    /// 全速向前猛撞敌人，20%概率降低敌方10%物防，该减益效果在目标精灵离场后立即清除。
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_5.DoSkill | 技能：冲撞 | bingoSkillType={run.BingoSkillType}");

        // 20%概率降低敌方10%物防
        if (GD.Randf() <= 0.20f)
        {
            GD.Print($"      [{index}] DuckSkill0_1_5.DoSkill | 触发20%概率，降低敌方10%物防");

            // 构造 Buff 字典：target_stat=4(DEF), layer=1, value=10, is_ratio=true
            var buffDict = new Godot.Collections.Dictionary
            {
                { "target_stat", (int)EnumPetBaseStats.DEF },
                { "num", 1 },
                { "layer", 1 },
                { "value", 10 },
                { "is_ratio", true }
            };

            // 确定敌方精灵 UUID
            string enemyPetUuid = null;
            if (run.Side == EnumWho.My)
            {
                enemyPetUuid = FightLandYouStandPet.Instance?.FightPetData?.PetUuid;
            }
            else
            {
                enemyPetUuid = FightLandMyStandPet.Instance?.FightPetData?.PetUuid;
            }

            var buffs = DevBuffTool.CreateFromArray(new Godot.Collections.Array { buffDict });
            if (buffs != null && buffs.Count > 0)
            {
                // 设置 Buff 所属精灵 UUID（敌方精灵）
                if (!string.IsNullOrWhiteSpace(enemyPetUuid))
                {
                    foreach (var buff in buffs)
                    {
                        buff.PetUuid = enemyPetUuid;
                    }
                }

                // 将 Buff 添加到敌方 BuffManager
                if (run.Side == EnumWho.My)
                {
                    FightYouStandBuffManager.Instance?.AddBuffs(buffs.ToArray());
                    GD.Print($"      [{index}] DuckSkill0_1_5.DoSkill | 物防降低Buff已添加至敌方");
                }
                else
                {
                    FightMyStandBuffManager.Instance?.AddBuffs(buffs.ToArray());
                    GD.Print($"      [{index}] DuckSkill0_1_5.DoSkill | 物防降低Buff已添加至我方");
                }
            }
        }
        else
        {
            GD.Print($"      [{index}] DuckSkill0_1_5.DoSkill | 未触发减物防效果（80%未命中）");
        }
    }
}