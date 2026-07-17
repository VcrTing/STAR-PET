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
		ResolveRefs();

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

		// 若 _Ready 尚未执行导致字段为空，在此重新获取
		ResolveRefs();

		GD.Print($"  🐾 SetPetData: {petData.PetName}, Hp={petData.Hp}/{petData.MaxHp}");

		if (_labelName != null)
			_labelName.Text = petData.PetName;
		else
			GD.PrintErr("  ⚠ BtnPackPetItem 未找到 LabelPetName");

		if (_labelHpNow != null)
			_labelHpNow.Text = petData.Hp.ToString();

		if (_labelHpAll != null)
			_labelHpAll.Text = petData.MaxHp.ToString();
	}

	private void ResolveRefs()
	{
		if (_avatar == null)
			_avatar = GodotTool.FindChildByName(this, "TextureRectPetAvatar") as TextureRect;
		if (_labelName == null)
			_labelName = GodotTool.FindChildByName(this, "LabelPetName") as Label;
		if (_labelHpNow == null)
			_labelHpNow = GodotTool.FindChildByName(this, "LabelPetHeartNow") as Label;
		if (_labelHpAll == null)
			_labelHpAll = GodotTool.FindChildByName(this, "LabelPetHeartAll") as Label;
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
