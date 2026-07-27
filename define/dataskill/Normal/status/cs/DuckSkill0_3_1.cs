using Godot;

[GlobalClass]
public partial class DuckSkill0_3_1 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_3_1.DoSkill | 技能：聚能 | bingoSkillType={run.BingoSkillType}");

        // 聚能效果：委托 FightSkillStatusRunTool 补充 5 点能量（Pp）
        FightSkillStatusRunTool.ExecuteFocusEnergy(index, run);
    }
}