using Godot;

[GlobalClass]
public partial class DuckSkill0_1_1 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_1.DoSkill | 技能：拍击 | bingoSkillType={run.BingoSkillType}");
    }
}