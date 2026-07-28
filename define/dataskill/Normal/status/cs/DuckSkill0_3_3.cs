using Godot;

[GlobalClass]
public partial class DuckSkill0_3_3 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        // GD.Print($"      [{index}] DuckSkill0_3_3.DoSkill | 技能：力量增效 | bingoSkillType={run.BingoSkillType}");

        // 力量增效效果：根据 BingoSkillType 判断使用 GainBuff 还是 GainBuffBingo
        Godot.Collections.Array buffSource = run.BingoSkillType == EnumSkillType.DEFENSE
            ? sideSkill.Skill.GainBuffBingo
            : sideSkill.Skill.GainBuff;
        FightSkillStatusRunTool.ExecuteStatusBuff(run, index, buffSource);
    }
}