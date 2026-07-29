/// <summary>
/// PVE AI 大脑接口
/// 根据玩家行动决策返回敌方行动，并提供换宠逻辑
/// </summary>
public interface IPveAiRunnerImpl
{
	TurnAction GetAction(TurnAction playerAction);

	/// <summary>
	/// 宠物死亡时获取下一只上场的宠物
	/// 默认取背包中第一只存活的精灵
	/// </summary>
	/// <returns>下一只上场的精灵数据，没有可用精灵返回 null</returns>
	InsFightPetData GetNextPetWhenPreDie();
}
