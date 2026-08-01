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
                GD.Print($"      [{index}] DuckSkill0_1_5.DoSkill | 物防降低Buff已添加至敌方");
        }
        else
        {
            GD.Print($"      [{index}] DuckSkill0_1_5.DoSkill | 未触发减物防效果（80%未命中）");
        }
    }

    /// <summary>
    /// 重构 TurnAction 数组并返回
    /// 传入双方行动数组，返回 side 对应的行动数组
    /// ⚠ 暂为占位实现：不做额外处理，直接返回 side 的行动数组
    /// </summary>
    public TurnAction[] RebuildTurn(TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        return side == EnumWho.My ? myTurnActions : youTurnActions;
    }

    /// <summary>
    /// Real-time sync skill
    /// 实时刷新技能状态
    /// 通过 sideSkill 修改技能源头
    /// </summary>
    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        // 通过 sideSkill 修改技能源头（留空待实现）
    }
}
