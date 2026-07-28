using Godot;

[GlobalClass]
public partial class DuckSkill0_1_4 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_4.DoSkill | 技能：夹击 | bingoSkillType={run.BingoSkillType}");
    }
}