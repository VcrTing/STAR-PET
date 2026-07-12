extends Resource
# ======================================================
# 精灵图鉴数据 - pet_0000
# 零号精灵 — 古龙残魂，失忆者
# 金系（Gold），古龙的残存之魂化身，目前失忆，主角还不知道其秘密
# ======================================================

# ---- 基础信息 ----
var pet_id := 0                    # 图鉴编号
var pet_name := "零仔"              # 精灵名称（失忆，未知其真名）
var pet_type := 5                  # 主系别（5=金系，对应 PetTypeDesign.Gold / EnumPetType.Gold）
var pet_types := [5]               # 系别数组（第一个元素为主系别，可双属性如 [5, 9]=金+龙）
var pet_model := ""                # 模型资源路径（后续补充）

# ---- 种族值（六维） ----
# 对应 PetBaseStatsDesign: 1=生命 2=物攻 3=魔攻 4=物防 5=魔防 6=速度
var base_stats := {
	1: 90,   # 生命（HP）
	2: 100,  # 物攻（ATK）
	3: 100,   # 魔攻（MATK）
	4: 70,  # 物防（DEF）
	5: 70,   # 魔防（MDEF）
	6: 90,   # 速度（SPD）
}

# ---- 可学习技能列表 ----
# skill_id: 技能ID, learn_level: 学习等级
class SkillEntry:
	var skill_id: int
	var learn_level: int
	
	func _init(id: int, level: int):
		skill_id = id
		learn_level = level

var learnable_skills := [
	SkillEntry.new(1, 1),     # 冲撞，1级学习
	SkillEntry.new(2, 5),     # 铁壁，5级学习
	SkillEntry.new(3, 10),    # 龙息，10级学习
	SkillEntry.new(4, 15),    # 龙鳞之护，15级学习
	SkillEntry.new(5, 20),    # 古龙之力，20级学习
	SkillEntry.new(6, 25),    # 龙魂觉醒，25级学习
]

# ---- 进化信息 ----
var evolution_id := 0              # 进化目标图鉴编号（0=最终形态或无进化）
var evolution_level := 0           # 进化等级（0=无法进化）

# ---- 捕捉信息 ----
var capture_rate := 0              # 捕捉率（0=不可捕捉，剧情获得）
var female_ratio := 50.00          # 雌性系别概率（0.00=全雄性, 50.00=男女各半, 100.00=全雌性）
var base_exp := 80                 # 基础经验值
var base_gold := 15                # 基础金币

# ---- 体型/声音（用于奖牌判定） ----
var body_size := 1.2               # 体型（>1.0=偏大，古龙残魂残留部分龙体特征）
var voice_pitch := 0.7             # 音调（<1.0=低沉，龙吼的低沉余韵）

# ---- 图鉴描述 ----
var description := "？？？\n\n不知何时出现在这片大陆上的神秘精灵，身上缠绕着古老而强大的金属性能量。\n它的眼神中透着迷茫与警惕，似乎失去了大部分记忆，只留下战斗的本能和一丝若隐若现的龙族威严。\n它沉默地跟在主角身后，仿佛在寻找什么——或者说，在等待什么。\n\n据古老文献记载，在遥远的过去，曾有一条统御万物的古龙，其力量足以撕裂星辰。\n当它陨落之时，天地变色，山河破碎。然而，有一缕残存的魂魄并未消散，而是化作了一道金光，坠入凡间，沉睡至今。\n\n主角尚未知晓这个秘密。"