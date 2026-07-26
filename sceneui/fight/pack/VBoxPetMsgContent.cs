using Godot;
using System;

public partial class VBoxPetMsgContent : VBoxContainer
{
	public static VBoxPetMsgContent Instance { get; private set; }

	private TextureButton _btnSureGoFight;
	private Label _labelPetName;
	private Label _labelPetHealth;
	private Label _labelPetPP;
	private InsFightPetData _fightPetData;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;

		_btnSureGoFight = FindChild("BtnSureGoFight", true, false) as TextureButton;
		if (_btnSureGoFight == null)
			GD.PrintErr("  ⚠ VBoxPetMsgContent 未找到 BtnSureGoFight");
		else
			_btnSureGoFight.Pressed += OnSureGoFight;
	}

	void Check()
	{
		if (_labelPetName == null)
		{
			_labelPetName = FindChild("LabelPetName", true, false) as Label;
			if (_labelPetName == null)
				GD.PrintErr("  ⚠ VBoxPetMsgContent 未找到 LabelPetName");
		}

		if (_labelPetHealth == null)
		{
			_labelPetHealth = FindChild("LabelPetHealth", true, false) as Label;
			if (_labelPetHealth == null)
				GD.PrintErr("  ⚠ VBoxPetMsgContent 未找到 LabelPetHealth");
		}

		if (_labelPetPP == null)
		{
			_labelPetPP = FindChild("LabelPetPP", true, false) as Label;
			if (_labelPetPP == null)
				GD.PrintErr("  ⚠ VBoxPetMsgContent 未找到 LabelPetPP");
		}
	}

	public override void _ExitTree()
	{
		if (Instance == this) Instance = null;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnSureGoFight()
	{
		if (_fightPetData == null)
		{
			GD.Print("  ⚠ VBoxPetMsgContent.OnSureGoFight _fightPetData 为 null");
			return;
		}

		// 通过 Uuid 匹配要切换的精灵索引
		var fightPets = PlayerLandMyStandPlayer.Instance?.FightPets;
		if (fightPets == null || fightPets.Count == 0)
		{
			GD.Print("  ⚠ VBoxPetMsgContent.OnSureGoFight 玩家没有战斗精灵数据");
			return;
		}

		int targetIndex = -1;
		for (int i = 0; i < fightPets.Count; i++)
		{
			if (fightPets[i].PetUuid == _fightPetData.PetUuid)
			{
				targetIndex = i;
				break;
			}
		}

		if (targetIndex < 0)
		{
			GD.Print($"  ⚠ VBoxPetMsgContent.OnSureGoFight 未找到 Uuid={_fightPetData.PetUuid} 的精灵");
			return;
		}

		GD.Print($"  🐾 VBoxPetMsgContent 确认上场: {_fightPetData.PetName} (Index={targetIndex})");
		FightCenterManger.Instance?.PlayerSelectSwitch(targetIndex);
	}

	public void UpdatePetData(InsFightPetData petData)
	{
		_fightPetData = petData;

		if (_fightPetData == null) return;

		Check();

		if (_labelPetName != null)
			_labelPetName.Text = _fightPetData.PetName;

		if (_labelPetHealth != null)
			_labelPetHealth.Text = $"血量: {_fightPetData.Hp}/{_fightPetData.MaxHp}";

		if (_labelPetPP != null)
			_labelPetPP.Text = $"能量: {_fightPetData.Pp}";
	}
}
