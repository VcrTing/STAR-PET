using Godot;

[GlobalClass]
public partial class DuckSkill6_1_1 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill6_1_1.DoSkill | 技能：极度冰点 | bingoSkillType={run.BingoSkillType}");

        // 秒杀判定（3%概率，99999固伤），触发时对敌方造成固定伤害
        FightSkillAttackRunTool.ExecuteInstantKill(index, sideSkill, run, "极度冰点", "秒杀");
    }

    /// <summary>
    /// Real-time sync skill
    /// 实时刷新技能状态
    /// 通过 sideSkill 修改技能源头
    /// </summary>
    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        // 通过 sideSkill 修改技能源头（留空待实现）
    }
}
