using Godot;
using System;

public partial class BtnPackPetItem : TextureButton
{
	private TextureRect _avatar;
	private Label _labelName;
	private Label _labelHpNow;
	private Label _labelHpAll;

	private InsFightPetData _cachedPetData;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_avatar = FindChild("TextureRectPetAvatar", true, false) as TextureRect;
		_labelName = FindChild("LabelPetName", true, false) as Label;
		_labelHpNow = FindChild("LabelPetHeartNow", true, false) as Label;
		_labelHpAll = FindChild("LabelPetHeartAll", true, false) as Label;

		if (_avatar == null) GD.PrintErr("  ⚠ BtnPackPetItem 未找到 TextureRectPetAvatar");
		if (_labelName == null) GD.PrintErr("  ⚠ BtnPackPetItem 未找到 LabelPetName");
		if (_labelHpNow == null) GD.PrintErr("  ⚠ BtnPackPetItem 未找到 LabelPetHeartNow");
		if (_labelHpAll == null) GD.PrintErr("  ⚠ BtnPackPetItem 未找到 LabelPetHeartAll");

		// 如果 SetPetData 在 _Ready 之前已被调用，在这里套用缓存的数据
		ApplyCachedData();

		Pressed += OnClick;
	}

	private void OnClick()
	{
		if (_cachedPetData == null) return;

		string name = _cachedPetData.PetName;
		GD.Print($"  🐾 点击精灵: {name}");

		VBoxPetMsgContent.Instance?.UpdatePetData(_cachedPetData);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetPetData(InsFightPetData petData)
	{
		if (petData == null) return;

		_cachedPetData = petData;

		// 如果 _Ready 已执行过（字段已赋值），立刻刷新 UI
		if (_labelName != null)
		{
			_labelName.Text = petData.PetName;
			_labelHpNow.Text = petData.Hp.ToString();
			_labelHpAll.Text = petData.MaxHp.ToString();
		}
		else
		{
			GD.PrintErr("  ⚠ BtnPackPetItem 未找到 LabelPetName");
		}
	}

	private void ApplyCachedData()
	{
		if (_cachedPetData == null) return;

		if (_labelName != null)
			_labelName.Text = _cachedPetData.PetName;
		if (_labelHpNow != null)
			_labelHpNow.Text = _cachedPetData.Hp.ToString();
		if (_labelHpAll != null)
			_labelHpAll.Text = _cachedPetData.MaxHp.ToString();
	}
}
