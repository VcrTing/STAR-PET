using Godot;

/// <summary>
/// 精灵天赋类型枚举
/// 天赋值范围 0-10
/// 0=普通天赋（0-3），1=一般般天赋（4-6），2=好天赋（7-9），4=极品天赋（10）
/// </summary>
public enum EnumPetTalent
{
	Normal = 0,       // 普通天赋（0-3）
	NormalPlus = 1,   // 一般般天赋（4-6）
	Good = 2,         // 好天赋（7-9）
	Excellent = 4,    // 极品天赋（10）
}