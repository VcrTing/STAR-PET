using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class DuckSkill0_1_6 : Resource
{
    /// <summary>
    /// 冲撞技能实现
    /// 全速向前猛撞敌人，20%概率降低敌方10%物防，该减益效果在目标精灵离场后立即清除。
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_6.DoSkill | 技能：冲撞 | bingoSkillType={run.BingoSkillType}");

        // 20%概率降低敌方10%物防
        if (GD.Randf() <= 0.20f)
        {
            GD.Print($"      [{index}] DuckSkill0_1_6.DoSkill | 触发20%概率，降低敌方10%物防");

            // 构造 Buff 字典：target_stat=4(DEF), num=1, layer=1, value=10, is_ratio=true
            var buffDict = new Godot.Collections.Dictionary
            {
                { "target_stat", (int)EnumPetBaseStats.DEF },
                { "num", 1 },
                { "layer", 1 },
                { "value", 10 },
                { "is_ratio", true }
            };

            int added = DevBuffTool.AddBuffToTargetPet(buffDict, run);
            if (added > 0)
                GD.Print($"      [{index}] DuckSkill0_1_6.DoSkill | 物防降低Buff已添加至敌方");
        }
        else
        {
            GD.Print($"      [{index}] DuckSkill0_1_6.DoSkill | 未触发减物防效果（80%未命中）");
        }
    }
}