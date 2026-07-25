extends Resource
# ======================================================
# 五号精灵生成配置 — stone_pet_0005
# 萤花仙（草+妖精系）的生成客制化参数
# 低速团队回能辅助定位
# ======================================================

# ---- 基础信息客制化 ----
var initial_level := 5               # 初始等级
var initial_nature := 0              # 初始性格（0=无修正，保守+魔攻-物攻后续可选）
var initial_intimacy := 100          # 初始亲密度

# ---- 特殊标识 ----
var is_locked := true                # 是否锁定（锁定后不可放生/交易）
var is_special := true               # 是否特殊精灵

# ---- 默认个体 ----
var default_big := 2                 # 默认个体档位（2=中个体，对应 EnumPetBig.Medium）

# ---- 初始天赋 ----
# 天赋类型：0=普通天赋(Normal)，1=一般般天赋(NormalPlus)，2=好天赋(Good)，4=极品天赋(Excellent)
var talent_type := 4                 # 初始天赋类型（2=好天赋）

# 固定天赋值的个体项（数组，元素为 EnumPetBaseStats 的 int 值）
# 1=生命(HP)，2=物攻(ATK)，3=魔攻(MATK)，4=物防(DEF)，5=魔防(MDEF)，6=速度(SPD)
# 为空或未定义则使用 GenerateAllTalentDict 全属性
var talent_fixed_stats := [1, 3, 5]  # 固定生命、魔攻、魔防三项为好天赋（突出辅助定位）

# ---- 获得信息 ----
var obtained_method := "野外捕捉"     # 获得方式
var obtained_location := "森林中央花海-空心古树"   # 获得地点