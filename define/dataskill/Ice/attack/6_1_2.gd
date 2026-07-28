extends Resource
# ======================================================
# 技能静态数据
# 命名: 6_1_2.gd
# 6 = 冰系, 1 = 攻击, 2 = code
# ======================================================

# ---- 系别信息 ----
var pet_type := 6                   # 系别（6=冰系，对应 EnumPetType.Ice）

# ---- 基础信息 ----
var skill_type := 1                # 技能类型（对应SkillTypeDesign：1=攻击, 2=防御, 3=状态）
var skill_code := 2                 # 技能编号
var skill_name := "冰心"            # 技能名

# 实现脚本
var impl_class = "res://define/dataskill/Ice/attack/cs/DuckSkill6_1_2.cs"

# ---- 攻击数值 ----
var attack_value := 20              # 攻击数值/威力
var attack_type := 2                # 攻击类型（对应EnumPetBaseStats：2=物攻[ATK], 3=魔攻[MATK], 0=固伤）

# ---- 连击 ----
var hit_count := 1                  # 连击数（默认1，>1表示连击技能）
var is_hit_combo := false           # 是否连击技能（默认false）

# ---- 能耗 ----
var pp_cost := 1                    # PP能耗

# ---- 图标 ----
var icon_path := "res://IMG/skill/Ice/attack/6_1_2.png"    # 技能图标图片地址

# ---- 命中与先手 ----
var hit_rate := 100.00              # 命中率（默认100.00）
var priority := 0                   # 先手值（默认0）
var hidden_priority := 0            # 隐藏先手判断（0=不先手判断，1=需根据对方释放的技能判断本技能是否先手）

# ---- 应对与减伤 ----
var bingo_skill_type = 0            # 1 = 应对攻击，0 = 无应对
var damage_reduction_rate = 0       # 减伤率（默认0，0-100范围，防御技能如70代表70%）

# ---- 特殊效果 ----
var instant_kill_rate := 12.00      # 秒杀敌人概率（12%概率爆发冰核冲击）
var instant_kill_damage := 300      # 秒杀伤害值（触发时造成300点固定物理伤害）

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
	[0.0, "ice_crystal", 1.0],
]  # 音效数组（数组类型，每个元素为[播放时间点(float), 音效名称(string), 音量(float)]）

# ---- 特效 ----
var particle_effects := [
	[0.0, "ice_shard"],
]  # 特效数组（数组类型，每个元素为[播放时间点(float), 特效名称(string)]）

# ---- 宠物动作 ----
var pet_actions := [
	[0.0, "attack_pierce"],
]  # 宠物动作数组（数组类型，每个元素为[播放时间点(float), 动作名称(string)]）

# ---- 描述 ----
var main_description := ["凝一缕清心寒冰刺向敌方躯体，微弱冰劲划伤皮肉"]                    # 主描述（数组类型）
var auxiliary_description := ["威力20，命中后有10%概率爆发冰核冲击，直接造成300点固定物理伤害"]        # 辅助描述（数组类型）