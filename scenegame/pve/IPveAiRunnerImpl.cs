/// <summary>
/// PVE AI 大脑接口
/// 根据玩家行动决策返回敌方行动
/// </summary>
public interface IPveAiRunnerImpl
{
	TurnAction GetAction(TurnAction playerAction);
}