using Godot;
using System;

public partial class VBoxPetMsgContent : VBoxContainer
{
	public static VBoxPetMsgContent Instance { get; private set; }

	private TextureButton _btnSureGoFight;
	private Label _labelPetMsgName;
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

		_labelPetMsgName = FindChild("LabelPetMsgName", true, false) as Label;
		if (_labelPetMsgName == null)
			GD.PrintErr("  ⚠ VBoxPetMsgContent 未找到 LabelPetMsgName");
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

		if (_labelPetMsgName != null && _fightPetData != null)
			_labelPetMsgName.Text = _fightPetData.PetName;
	}
}
