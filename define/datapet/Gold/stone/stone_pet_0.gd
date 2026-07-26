extends Resource
# ======================================================
# 零号精灵固定生成配置 — stone_pet_0000
# 字典格式：pet_{petId}__{Index}
# 每一组定义一个固定精灵的生成参数
# 由 DevPackPetGeneraTool.InitSpecialStonePet(pet, petType, index) 加载
# ======================================================

var pet_0__0 := {
	initial_level = 5,               # 初始等级
	initial_nature = 1,              # 初始性格（EnumPetNature）
	initial_intimacy = 100,          # 初始亲密度
	is_locked = true,                # 是否锁定
	is_special = true,               # 是否特殊精灵
	default_big = 3,                 # 默认个体档位（EnumPetBig.Large）
	talent_type = 4,                 # 初始天赋类型（4=极品天赋，EnumPetTalent.Excellent）
	talent_fixed_stats = [2, 3, 6],  # 固定天赋的个体项（1=HP, 2=ATK, 3=MATK, 4=DEF, 5=MDEF, 6=SPD）
	obtained_method = "初始精灵",     # 获得方式
	obtained_location = "启程之森",   # 获得地点
}