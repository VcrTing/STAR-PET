using Godot;

/// <summary>
/// 精灵天赋类型枚举
/// 天赋值范围 0-10
/// 0=零天赋（固定0），1=普通天赋（0-3），2=一般般天赋（4-6），3=好天赋（7-9），4=极品天赋（10）
/// </summary>
public enum EnumPetTalent
{
	Zero = 0,         // 零天赋（固定0）
	Normal = 1,       // 普通天赋（0-3）
	NormalPlus = 2,   // 一般般天赋（4-6）
	Good = 3,         // 好天赋（7-9）
	Excellent = 4,    // 极品天赋（10）
}
