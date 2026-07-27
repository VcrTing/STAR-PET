using Godot;

[GlobalClass]
public partial class DuckSkill0_4_1 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_4_1.DoSkill | 技能：选择切换宠物 | bingoSkillType={run.BingoSkillType}");
    }
}