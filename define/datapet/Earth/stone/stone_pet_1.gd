extends Resource
# ======================================================
# 一号精灵生成配置 — stone_pet_0001
# 敌方的铁甲虫生成参数
# ======================================================

# ---- 基础信息客制化 ----
var initial_level := 60               # 初始等级（PVP等级）
var initial_nature := 7               # 初始性格（7=固执，对应 EnumPetNature.Adamant）
var initial_intimacy := 100           # 初始亲密度

# ---- 特殊标识 ----
var is_locked := false                # 是否锁定
var is_special := false               # 是否特殊精灵

# ---- 默认个体 ----
var default_big := 2                 # 默认个体档位（2=中等，对应 EnumPetBig.Medium）

# ---- 初始天赋 ----
var talent_type := 4                 # 天赋类型（2=好天赋）

var talent_fixed_stats := [2, 3, 6]  # 固定物攻、魔攻、速度三项为极品天赋

# ---- 获得信息 ----
var obtained_method := "敌方精灵"     # 获得方式
var obtained_location := "战斗区域"   # 获得地点