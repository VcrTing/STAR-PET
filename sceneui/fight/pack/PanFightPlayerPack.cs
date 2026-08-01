using Godot;
using System;

public partial class PanFightPlayerPack : PanelContainer
{
	public static PanFightPlayerPack Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		Visible = false;
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OpenForLimit(InsFightPetData[] fightPetDatas)
	{
		Visible = true;
		// 刷新关闭按钮状态：强制换宠时隐藏（不允许关闭），允许关闭时显示
		RefreshCloseBtnState();
		ScrollPetsContent.Instance?.RefreshPackPetItems(fightPetDatas);
	}
	public void Open()
	{
		OpenForLimit(PlayerLandMyStandPlayer.Instance?.FightPets.ToArray());
	}

	public void Close()
	{
		// 强制换宠状态（My 精灵濒死）下不允许关闭切换宠物 Pan
		if (FightCenterManger.Instance?.NeedPlayerFaintSwitch == true)
		{
			GD.Print("[PanFightPlayerPack.Close] 强制换宠状态，禁止关闭切换宠物面板");
			return;
		}

		Visible = false;
	}

	/// <summary>
	/// 刷新关闭按钮状态：
	/// 强制换宠状态（不允许关闭）→ 隐藏关闭按钮；
	/// 允许关闭 → 显示关闭按钮。
	/// 通过 BtnCloseFightPlayerPack 单例刷新。
	/// </summary>
	private void RefreshCloseBtnState()
	{
		var btnClose = BtnCloseFightPlayerPack.Instance;
		if (btnClose == null)
			return;

		bool forced = FightCenterManger.Instance?.NeedPlayerFaintSwitch == true;
		btnClose.Visible = !forced;
	}
}
