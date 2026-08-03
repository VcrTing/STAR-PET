using Godot;

/// <summary>
/// 战斗中的能力（Power）实体类
/// 类似 Buff，但作用于"能力类型"（如先手值）而非个体值（Stat）
/// </summary>
public class InsFightPower
{
    /// 影响的能力类型（如先手值 RoundPriority）
    public EnumFightPowerType PowerType;

    // 层数
    public int Layer;

    // 每一层多少值
    public int Value;

    // 是百分比还是纯加值
    public bool IsRatio;

    // 存活回合数
    public int AliveNum;

    // 所属精灵 UUID（标记此能力归属于哪只精灵）
    public string PetUuid;

    // 生效模式（默认 ThisPetAppear = 5）
    public EnumBuffActiveMode ActiveMode = EnumBuffActiveMode.ThisPetAppear;
}