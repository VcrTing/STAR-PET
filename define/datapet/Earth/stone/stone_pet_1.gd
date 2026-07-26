extends Resource
# ======================================================
# 一号精灵固定生成配置 — stone_pet_0001
# 字典格式：pet_{petId}__{Index}
# 每一组定义一个固定精灵的生成参数
# 由 DevPackPetGeneraTool.InitSpecialStonePet(pet, petType, index) 加载
# ======================================================

var pet_1__0 := {
	initial_level = 60,              # 初始等级（PVP等级）
	initial_nature = 1,              # 初始性格（EnumPetNature）
	initial_intimacy = 100,          # 初始亲密度
	is_locked = false,               # 是否锁定
	is_special = false,              # 是否特殊精灵
	default_big = 2,                 # 默认个体档位（2=中等，EnumPetBig.Medium）
	talent_type = 4,                 # 初始天赋类型（4=极品天赋，EnumPetTalent.Excellent）
	talent_fixed_stats = [2, 3, 6],  # 固定天赋的个体项
	obtained_method = "敌方精灵",     # 获得方式
	obtained_location = "战斗区域",   # 获得地点
}