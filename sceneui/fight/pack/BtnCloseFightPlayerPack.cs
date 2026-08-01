using Godot;
using System;

/// <summary>
/// 专门关闭 PanFightPlayerPack 的关闭按钮（单例）
/// 点击时关闭 PanFightPlayerPack，PanFightPlayerPack.RefreshCloseBtnState 通过此单例刷新显隐
/// </summary>
public partial class BtnCloseFightPlayerPack : TextureButton
{
	public static BtnCloseFightPlayerPack Instance { get; private set; }

	public override void _Ready()
	{
		Instance = this;
		Pressed += OnClick;
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}

	private void OnClick()
	{
		// 强制换宠状态（My 精灵濒死）下拦截关闭，不允许关闭切换宠物 Pan
		if (FightCenterManger.Instance?.NeedPlayerFaintSwitch == true)
		{
			GD.Print("[BtnCloseFightPlayerPack.OnClick] 强制换宠状态，禁止关闭切换宠物面板");
			return;
		}

		PanFightPlayerPack.Instance?.Close();
	}
}