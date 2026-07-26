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
			_btnSureGoFight.Pressed += () => GD.Print("  🐾 精灵上场");

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
