using Godot;

[GlobalClass]
public partial class DuckSkill0_1_1 : Resource
{
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        GD.Print($"      [{index}] DuckSkill0_1_1.DoSkill | 技能：拍击 | Side={run.Side}");

        // 读取 gain_energy，无/0 则默认=1
        int gainEnergy = sideSkill?.Skill?.GainEnergy ?? 0;
        if (gainEnergy <= 0)
            gainEnergy = 1;

        FightPpTool.GainPp(run.Side, gainEnergy, false, index);
    }
}