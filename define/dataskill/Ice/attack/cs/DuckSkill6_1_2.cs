using Godot;

[GlobalClass]
public partial class DuckSkill6_1_2 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill6_1_2.DoSkill | 技能：冰心 | bingoSkillType={run.BingoSkillType}");

        // 冰核冲击判定（10%概率，300点固伤），触发时对敌方造成固定伤害
        FightSkillAttackRunTool.ExecuteInstantKill(index, sideSkill, run, "冰心", "冰核冲击");
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
