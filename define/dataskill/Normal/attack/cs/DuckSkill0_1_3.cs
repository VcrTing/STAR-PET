using Godot;

[GlobalClass]
public partial class DuckSkill0_1_3 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_3.DoSkill | 技能：后发制人 | bingoSkillType={run.BingoSkillType}");
    }
}