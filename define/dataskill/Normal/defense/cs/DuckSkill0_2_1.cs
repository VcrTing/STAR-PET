using Godot;

[GlobalClass]
public partial class DuckSkill0_2_1 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_2_1.DoSkill | 技能：防御 | bingoSkillType={run.BingoSkillType}");
    }
}