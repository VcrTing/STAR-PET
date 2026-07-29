using Godot;

/// <summary>
/// 精灵 HP 工具
/// 提供扣血、回血等可复用的 HP 操作方法
/// 根据 side 获取对应的精灵数据进行操作
/// </summary>
public static class FightPetHpTool
{
    /// <summary>
    /// 扣除对应方当前精灵的 HP（扣血）
    /// </summary>
    /// <param name="side">阵营（我方/敌方）</param>
    /// <param name="damage">要扣除的伤害值（正数）</param>
    /// <param name="index">阶段索引号（仅用于日志，默认0）</param>
    /// <returns>实际扣除的伤害值</returns>
    public static int DeductHp(EnumWho side, int damage, int index = 0)
    {
        string sideLabel = side == EnumWho.My ? "🧑我方" : "👹敌方";

        if (damage <= 0)
        {
            GD.Print($"      [{index}] {sideLabel} DeductHp | damage={damage}，无需扣血");
            return 0;
        }

        InsFightPetData pet = side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        if (pet == null)
        {
            GD.Print($"      [{index}] {sideLabel} DeductHp | 目标精灵为空，跳过扣血");
            return 0;
        }

        int beforeHp = pet.Hp;
        int actualDamage = Mathf.Min(damage, pet.Hp);
        pet.Hp = Mathf.Max(pet.Hp - damage, 0);

        GD.Print($"      [{index}] {sideLabel} DeductHp | " +
                 $"damage={actualDamage} {pet.PetName} HP: {beforeHp} → {pet.Hp}/{pet.MaxHp}");

        return actualDamage;
    }

    /// <summary>
    /// 为对应方当前精灵恢复 HP（回血）
    /// </summary>
    /// <param name="side">阵营（我方/敌方）</param>
    /// <param name="amount">要恢复的血量（正数）</param>
    /// <param name="allowOverflow">是否允许溢出上限（默认 false，超过上限时取 MaxHp）</param>
    /// <param name="index">阶段索引号（仅用于日志，默认0）</param>
    /// <returns>实际恢复的血量</returns>
    public static int GainHp(EnumWho side, int amount, bool allowOverflow = false, int index = 0)
    {
        string sideLabel = side == EnumWho.My ? "🧑我方" : "👹敌方";

        if (amount <= 0)
        {
            GD.Print($"      [{index}] {sideLabel} GainHp | amount={amount}，无需回血");
            return 0;
        }

        InsFightPetData pet = side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        if (pet == null)
        {
            GD.Print($"      [{index}] {sideLabel} GainHp | 目标精灵为空，跳过回血");
            return 0;
        }

        int beforeHp = pet.Hp;
        int actualHeal;

        if (allowOverflow)
        {
            // 允许溢出，不设上限
            pet.Hp = pet.Hp + amount;
            actualHeal = amount;
        }
        else
        {
            // 不允许溢出，取 MaxHp 上限
            int maxHp = pet.MaxHp;
            int beforeClamp = pet.Hp;
            pet.Hp = Mathf.Min(pet.Hp + amount, maxHp);
            actualHeal = pet.Hp - beforeClamp;
        }

        GD.Print($"      [{index}] {sideLabel} GainHp | " +
                 $"heal={actualHeal} {pet.PetName} HP: {beforeHp} → {pet.Hp}/{pet.MaxHp} " +
                 $"allowOverflow={allowOverflow}");

        return actualHeal;
    }

    /// <summary>
    /// 获取对应方当前精灵的当前 HP
    /// </summary>
    /// <param name="side">阵营（我方/敌方）</param>
    /// <returns>当前 HP 值，精灵不存在返回 0</returns>
    public static int GetCurrentHp(EnumWho side)
    {
        InsFightPetData pet = side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        return pet?.Hp ?? 0;
    }

    /// <summary>
    /// 获取对应方当前精灵的最大 HP
    /// </summary>
    /// <param name="side">阵营（我方/敌方）</param>
    /// <returns>最大 HP 值，精灵不存在返回 0</returns>
    public static int GetMaxHp(EnumWho side)
    {
        InsFightPetData pet = side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        return pet?.MaxHp ?? 0;
    }

    /// <summary>
    /// 判断对应方当前精灵是否存活
    /// </summary>
    /// <param name="side">阵营（我方/敌方）</param>
    /// <returns>存活返回 true，精灵不存在或 HP <= 0 返回 false</returns>
    public static bool IsAlive(EnumWho side)
    {
        InsFightPetData pet = side == EnumWho.My
            ? FightLandMyStandPet.Instance?.FightPetData
            : FightLandYouStandPet.Instance?.FightPetData;

        return pet != null && pet.Hp > 0;
    }
}