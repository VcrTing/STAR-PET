extends Resource
# ======================================================
# 技能静态数据
# 命名: 0_1_3.gd
# 0 = 普通系, 1 = 攻击, 3 = code
# ======================================================

# ---- 系别信息 ----
var pet_type := 0                   # 系别（0=普通系，对应 EnumPetType.Normal）

# ---- 基础信息 ----
var skill_type := 1                # 技能类型（对应SkillTypeDesign：1=攻击, 2=防御, 3=状态）
var skill_code := 3                 # 技能编号
var skill_name := "后发制人"        # 技能名

# ---- 攻击数值 ----
var attack_value := 150            # 攻击数值/威力
var attack_type := 2               # 攻击类型（对应EnumPetBaseStats：2=物攻[ATK], 3=魔攻[MATK], 0=固伤）

# ---- 连击 ----
var hit_count := 1                  # 连击数（默认1，>1表示连击技能）
var is_hit_combo := false           # 是否连击技能（默认false）

# ---- 能耗 ----
var pp_cost := 4                    # PP能耗（默认4，范围0-50）

# ---- 图标 ----
var icon_path := "res://IMG/skill/Normal/attack/0_1_3.png"    # 技能图标图片地址

# ---- 命中与先手 ----
var hit_rate := 100.00              # 命中率（默认100.00）
var priority := -1                  # 先手值（-1=后手出手，比普通技能更慢）
var hidden_priority := 0            # 隐藏先手判断（0=不先手判断，普通先手值比较）

# ---- 应对与减伤 ----
var bingo_skill_type = 0            # 1 = 应对攻击，0 = 无应对
var damage_reduction_rate = 0       # 减伤率（默认0，0-100范围，防御技能如70代表70%）

# ---- 特殊效果 ----
var instant_kill_rate := 0.00       # 秒杀敌人概率（默认0.00）

# ---- 特殊处理 ----
var turn_end_special_id := 0        # 回合结束特殊处理代码ID（默认0=无特殊处理）
var before_action_special_id := 0   # 回合内释放前特殊处理代码ID（默认0=无特殊处理）

# ---- 增减益 ----
var gain_energy := 0                # 获得能量（默认0，正数=获得，负数=扣除）
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
	[0.0, "hit_powerful", 1.0],
]  # 音效数组（数组类型，每个元素为[播放时间点(float), 音效名称(string), 音量(float)]）

# ---- 特效 ----
var particle_effects := [
	[0.0, "strike_powerful"],
]  # 特效数组（数组类型，每个元素为[播放时间点(float), 特效名称(string)]）

# ---- 宠物动作 ----
var pet_actions := [
	[0.0, "attack_heavy"],
]  # 宠物动作数组（数组类型，每个元素为[播放时间点(float), 动作名称(string)]）

# ---- 描述 ----
var main_description := ["在对手行动之后发动强力的反击"]                    # 主描述（数组类型）
var auxiliary_description := ["后手技能，必定比普通技能后出手，但威力极高"]        # 辅助描述（数组类型）