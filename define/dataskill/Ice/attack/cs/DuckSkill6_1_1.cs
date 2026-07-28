using Godot;

[GlobalClass]
public partial class DuckSkill6_1_1 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill6_1_1.DoSkill | 技能：极度冰点 | bingoSkillType={run.BingoSkillType}");

        // 秒杀判定（3%概率，99999固伤）
        FightSkillAttackRunTool.ExecuteInstantKill(index, sideSkill, "极度冰点", "秒杀");
    }
}