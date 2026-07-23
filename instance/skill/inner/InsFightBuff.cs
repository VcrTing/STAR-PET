using Godot;

/// <summary>
/// 战斗中的 Buff 实体类
/// 用于增益/减益宠物某种个体值
/// </summary>
public class InsFightBuff
{
    /// 影响的个体值属性
    public EnumPetBaseStats TargetStat;

    // 层数
    public int Num;

    // 每一层多少值
    public int Value; 

    // 是百分比还是纯加值
    public bool IsRatio;
}