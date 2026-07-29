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
}