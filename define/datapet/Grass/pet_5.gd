extends Resource
# ======================================================
# 精灵图鉴数据 - pet_0005
# 五号精灵 — 萤花仙
# 草+妖精系（Grass + Fairy），低速团队回能辅助
# ======================================================

# ---- 基础信息 ----
var pet_id := 5                    # 图鉴编号
var pet_name := "萤花仙"            # 精灵名称
var pet_types := [3, 8]            # 系别数组（3=草 Grass, 8=妖精 Fairy）
var pet_model := ""                # 模型资源路径（后续补充）
var pet_fly_type := [4]            # 移动方式数组（4=走路，对应 EnumPetFly.Walk）
var hostile_level := 1             # 对人类攻击等级（1=不主动攻击，对应 EnumPetHostileLevel.Passive）
var pet_role_group := 5            # 战斗定位组（5=增益辅助，对应 EnumPetRoleGroup.BuffSupport）
var pet_category_group := 4        # 外观类别组（4=萌物，对应 EnumPetCategoryGroup.Cute）
var pet_map_group := 6             # 地图区域组（6=陆地，对应 EnumPetMapGroup.Land）

# ---- 种族值（六维） ----
# 对应 PetBaseStatsDesign: 1=生命 2=物攻 3=魔攻 4=物防 5=魔防 6=速度
var base_stats := {
	1: 108,  # 生命（HP）
	2: 30,   # 物攻（ATK）
	3: 110,  # 魔攻（MATK）
	4: 82,   # 物防（DEF）
	5: 112,  # 魔防（MDEF）
	6: 40,   # 速度（SPD）
}

# ---- 可学习技能列表 ----
# 二维数组：[技能ID（string "系别_类型_编号"）, 学习等级（int）]
# 默认包含 dataskill/Normal 下所有技能，0级可学习
var learnable_skills := [
	["0_1_1", 0],   # 拍击，0级学习
	["0_3_1", 0],   # 聚能，0级学习
	["0_1_2", 0],   # 先发制人，0级学习
	["0_1_3", 0],   # 后发制人，0级学习
	["0_3_2", 0],   # 加固，0级学习
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
var body_size := 1.0               # 体型（标准体型）
var voice_pitch := 1.0             # 音调（1.0=标准）

# ---- 图鉴描述 ----
var description := "萤花仙\n\n萤火般温柔闪耀的仙灵精灵，栖息于森林中央花海的空心古树旁。\n周身缠绕细嫩草藤，六片粉绿渐变花瓣翅膀散发着淡雅的荧光。\n脚下自然生成的青草场地滋养着万物，兼具草本生机与妖精仙气。\n\n性格温和，喜爱帮助他人，尤其亲近刚踏上旅途的新手训练家。\n它柔和的治愈之力能够为全队缓缓恢复技能能耗，是开荒旅途中不可或缺的可靠伙伴。\n\n据说当月光洒落在花海之上时，萤花仙会随着夜风翩翩起舞，\n洒下的荧光粉末能为周围的草木带来蓬勃生机。"