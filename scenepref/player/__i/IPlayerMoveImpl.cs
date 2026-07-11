using Godot;

/// <summary>
/// 移动模块接口，只处理移动相关逻辑
/// </summary>
public interface IPlayerMoveImpl
{
	void OnMoveUp();
	void OnMoveDown();
	void OnMoveLeft();
	void OnMoveRight();
	void OnMoveLeftRelease();
	void OnMoveRightRelease();
	void OnSprint();
	void OnJump();
}