using Godot;

/// <summary>
/// 战斗中的 Buff 实体类
/// 用于增益/减益宠物某种个体值
/// </summary>
public class InsFightBuff
{
    /// 影响的个体值属性
    public EnumPetBaseStats Stat;

    // 层数
    public int Layer;

    // 每一层多少值
    public int Value; 

    // 是百分比还是纯加值
    public bool IsRatio;

    // 存活回合数
    public int AliveNum;

    // 所属精灵 UUID（标记此 Buff 归属于哪只精灵）
    public string PetUuid;

    // 生效模式（默认 ThisPetAppear = 5）
    public EnumBuffActiveMode ActiveMode = EnumBuffActiveMode.ThisPetAppear;
}