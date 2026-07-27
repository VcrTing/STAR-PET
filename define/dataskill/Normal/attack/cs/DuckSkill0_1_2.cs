using Godot;

[GlobalClass]
public partial class DuckSkill0_1_2 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_2.DoSkill | 技能：先发制人 | bingoSkillType={run.BingoSkillType}");
    }
}