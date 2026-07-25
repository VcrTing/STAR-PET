extends Resource
# ======================================================
# 技能静态数据
# 命名: 0_4_1.gd
# 0 = 普通系, 4 = 系统, 1 = code
# ======================================================

# ---- 系别信息 ----
var pet_type := 0                   # 系别（0=普通系，对应 EnumPetType.Normal）

# ---- 基础信息 ----
var skill_type := 4                 # 技能类型（对应SkillTypeDesign：4=系统）
var skill_code := 1                 # 技能编号
var skill_name := "选择切换宠物"      # 技能名

# ---- 攻击数值 ----
var attack_value := 0               # 攻击数值/威力（系统技能为0）
var attack_type := 0                # 攻击类型（系统技能为0）

# ---- 连击 ----
var hit_count := 1                  # 连击数（默认1，>1表示连击技能）
var is_hit_combo := false           # 是否连击技能（默认false）

# ---- 能耗 ----
var pp_cost := 0                    # PP能耗（默认0）

# ---- 图标 ----
var icon_path := "res://IMG/skill/Normal/system/0_4_1.png"    # 技能图标图片地址

# ---- 命中与先手 ----
var hit_rate := 100.00              # 命中率（默认100.00）
var priority := 0                   # 先手值（默认0）
var hidden_priority := 0            # 隐藏先手判断（0=不先手判断，1=需根据对方释放的技能判断本技能是否先手）

# ---- 应对与减伤 ----
var bingo_skill_type = 0            # 1 = 应对攻击，0 = 无应对
var damage_reduction_rate = 0       # 减伤率（默认0，0-100范围）

# ---- 特殊效果 ----
var instant_kill_rate := 0.00       # 秒杀敌人概率（默认0.00）

# ---- 特殊处理 ----
var turn_end_special_id := 0        # 回合结束特殊处理代码ID（默认0=无特殊处理）
var before_action_special_id := 0   # 回合内释放前特殊处理代码ID（默认0=无特殊处理）

# ---- 增减益 ----
var gain_energy := 0                # 获得能量（默认0）
var gain_hp := 0                    # 获得血量（默认0，正数=获得，负数=扣除）
var gain_buff := []                 # 获得 Buff（默认空数组）
var gain_buff_bingo := []           # 应对成功后的 Buff（默认空数组）

# ---- 印记 ----
var marks := []                     # 印记（默认空数组）

# ---- 异常状态 ----
var status_effects := []            # 异常状态（默认空数组）

# ---- 音效 ----
var sound_effects := []             # 音效数组（默认空数组）

# ---- 特效 ----
var particle_effects := []          # 特效数组（默认空数组）

# ---- 宠物动作 ----
var pet_actions := []               # 宠物动作数组（默认空数组）

# ---- 描述 ----
var main_description := ["选择当前上阵宠物，将其切换为备选池中的另一只宠物"]                    # 主描述（数组类型）
var auxiliary_description := ["系统技能，战斗中切换宠物时使用"]                                # 辅助描述（数组类型）