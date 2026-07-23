extends Resource
# ======================================================
# 技能静态数据
# 命名: 0_2_1.gd
# 0 = 普通系, 2 = 防御, 1 = code
# ======================================================

# ---- 系别信息 ----
var pet_type := 0                   # 系别（0=普通系，对应 EnumPetType.Normal）

# ---- 基础信息 ----
var skill_type := 2                # 技能类型（对应SkillTypeDesign：1=攻击, 2=防御, 3=状态）
var skill_code := 1                 # 技能编号
var skill_name := "防御"            # 技能名

# ---- 攻击数值 ----
var attack_value := 0              # 攻击数值/威力（防御技能为0）
var attack_type := 0               # 攻击类型（防御技能为0）

# ---- 连击 ----
var hit_count := 1                  # 连击数（默认1，>1表示连击技能）
var is_hit_combo := false           # 是否连击技能（默认false）

# ---- 能耗 ----
var pp_cost := 1                    # PP能耗（默认1，范围0-50）

# ---- 图标 ----
var icon_path := "res://IMG/skill/Normal/defense/0_2_1.png"    # 技能图标图片地址

# ---- 命中与先手 ----
var hit_rate := 100.00              # 命中率（默认100.00）
var priority := 0                   # 先手值（默认0）
var hidden_priority := 1            # 隐藏先手判断（1=需根据对方释放的技能判断本技能是否先手，防御类技能根据对方攻击技能决定是否先手）


# ---- 应对与减伤 ----
var bingo_skill_type = 1            # 1 = 应对攻击，0 = 无应对
var damage_reduction_rate = 70      # 减伤率（默认0，0-100范围，70代表70%减伤）

# ---- 特殊效果 ----
var instant_kill_rate := 0.00       # 秒杀敌人概率（默认0.00）

# ---- 特殊处理 ----
var turn_end_special_id := 0        # 回合结束特殊处理代码ID（默认0=无特殊处理）
var before_action_special_id := 2   # 回合内释放前特殊处理代码ID（2=防御特殊处理：本回合受到伤害减半）

# ---- 增减益 ----
var gain_energy := 1                # 获得能量（默认1）
var gain_hp := 0                    # 获得血量（默认0，正数=获得，负数=扣除）
var gain_buff := []  
# 获得 Buff（数组类型，每个元素为字典：target_stat=属性ID对应EnumPetBaseStats；num=层数；value=每层值；is_ratio=是否百分比）
var gain_buff_bingo := []  
# 应对成功后的 Buff（应对成功时替换 gain_buff 生效）

# ---- 印记 ----
var marks := []                     # 印记（数组类型，每个元素为印记ID对应SkillMarkDesign，默认空数组。精灵下场印记不消失）

# ---- 异常状态 ----
var status_effects := []            # 异常状态（数组类型，每个元素为字典，含type和amount，默认空数组）

# ---- 音效 ----
var sound_effects := [
	[0.0, "defense_up", 1.0],
]  # 音效数组（数组类型，每个元素为[播放时间点(float), 音效名称(string), 音量(float)]）

# ---- 特效 ----
var particle_effects := [
	[0.0, "shield_raise"],
]  # 特效数组（数组类型，每个元素为[播放时间点(float), 特效名称(string)]）

# ---- 宠物动作 ----
var pet_actions := [
	[0.0, "defense_guard"],
]  # 宠物动作数组（数组类型，每个元素为[播放时间点(float), 动作名称(string)]）

# ---- 描述 ----
var main_description := ["架起防御姿态，减少本回合受到的伤害"]                    # 主描述（数组类型）
var auxiliary_description := ["防御技能，使用时根据对方攻击技能判断先手，本回合受伤减半"]        # 辅助描述（数组类型）