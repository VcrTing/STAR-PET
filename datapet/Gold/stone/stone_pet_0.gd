extends Resource
# ======================================================
# 零号精灵生成配置 — stone_pet_0000
# 初始零号精灵（古龙残魂）的生成客制化参数
# 当 DevPackPetGeneraTool.InitZeroPet() 检测到生成零号精灵时
# 自动加载此文件，根据此配置生成精灵数据
# ======================================================

# ---- 基础信息客制化 ----
var initial_level := 5               # 初始等级
var initial_nature := 11             # 初始性格（11=胆小，对应 EnumPetNature.Timid，加速减生命）
var initial_intimacy := 30           # 初始亲密度

# ---- 特殊标识 ----
var is_locked := true                # 是否锁定（锁定后不可放生/交易）
var is_special := true               # 是否特殊精灵

# ---- 默认个体 ----
var default_big := 3                 # 默认个体档位（3=大个体，对应 EnumPetBig.Large）

# ---- 初始天赋 ----
# 天赋类型：0=普通天赋(Normal)，1=一般般天赋(NormalPlus)，2=好天赋(Good)，4=极品天赋(Excellent)
var talent_type := 4                 # 初始天赋类型（4=极品天赋，全属性天赋值=10）

# 固定天赋值的个体项（数组，元素为 EnumPetBaseStats 的 int 值）
# 1=生命(HP)，2=物攻(ATK)，3=魔攻(MATK)，4=物防(DEF)，5=魔防(MDEF)，6=速度(SPD)
# 为空或未定义则使用 GenerateAllTalentDict 全属性
var talent_fixed_stats := [2, 3, 6]  # 固定物攻、魔攻、速度三项为极品天赋

# ---- 获得信息 ----
var obtained_method := "初始精灵"     # 获得方式
var obtained_location := "启程之森"   # 获得地点