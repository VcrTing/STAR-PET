extends Resource
# ======================================================
# 五号精灵固定生成配置 — stone_pet_0005
# 字典格式：pet_{petId}__{Index}
# 每一组定义一个固定精灵的生成参数
# 由 DevPackPetGeneraTool.InitSpecialStonePet(pet, petType, index) 加载
# ======================================================

var pet_5__0 := {
	initial_level = 5,               # 初始等级
	initial_nature = 0,              # 初始性格（0=无修正，EnumPetNature）
	initial_intimacy = 100,          # 初始亲密度
	is_locked = true,                # 是否锁定
	is_special = true,               # 是否特殊精灵
	default_big = 2,                 # 默认个体档位（2=中个体，EnumPetBig.Medium）
	talent_type = 4,                 # 初始天赋类型（4=极品天赋，EnumPetTalent.Excellent）
	talent_fixed_stats = [1, 3, 5],  # 固定天赋的个体项（1=HP, 3=MATK, 5=MDEF）
	obtained_method = "野外捕捉",     # 获得方式
	obtained_location = "森林中央花海-空心古树", # 获得地点
}