using Godot;

[GlobalClass]
public partial class DuckSkill0_1_10 : Resource
{
    /// <summary>
    /// 垂死反击技能实现
    /// 血量濒危时迸发反击力量，自身当前血量每降低10%，技能威力永久提升8点。
    ///
    /// 实现机制（与气势一击相同的 RebuildTurn/DoSkill 分离模式，衔接伤害计算与技能执行）：
    /// 1. RebuildTurn 阶段（回合行动排序时调用，早于伤害计算）：
    ///    - 读取自身当前血量（FightPetHpTool.GetCurrentHp(side)）与最大血量（GetMaxHp(side)）
    ///    - 计算血量损失百分比，每降低10% => 威力 +8（80→提升后，影响 FightDamageTool 伤害计算）
    /// 2. DoSkill 阶段（回合执行时调用，晚于伤害计算）：
    ///    - 由于伤害已按提升后的威力结算，此处还原威力为基础威力
    /// </summary>
    public void DoSkill(int index, FightRunning run, InsFightSkill sideSkill)
    {
        
    }

    /// <summary>
    /// 重构 TurnAction 数组并返回
    /// 根据自身当前血量计算威力加成：自身当前血量每降低10%，技能威力提升8点。
    /// 在 RebuildTurn 阶段设置 ActualAttackValue（供伤害计算读取）。
    /// </summary>
    public TurnAction[] RebuildTurn(TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        // 本技能（side 侧）行动数组
        TurnAction[] sideActions = side == EnumWho.My ? myTurnActions : youTurnActions;

        return sideActions;
    }

    /// <summary>
    /// 技能开始执行（Skill 阶段开始）
    /// </summary>
    public void StartSkill(int index, FightRunning run, InsFightSkill sideSkill, FightRunning[] sideRunnings)
    {
        // 留空待实现
    }

    /// <summary>
    /// 技能结束执行（Skill 阶段结束）
    /// </summary>
    public void EndSkill(int index, FightRunning run, InsFightSkill sideSkill, FightRunning[] sideRunnings)
    {
        // 留空待实现
    }

    /// <summary>
    /// Real-time sync skill
    /// 实时刷新技能状态
    /// 垂死反击：血量濒危时迸发反击力量，自身当前血量每降低10%，技能威力永久提升8点。
    /// 通过 sideSkill 干预伤害（写入 ActualAttackValue 供伤害计算读取）。
    /// </summary>
    public void RealtimeSync(EnumWho side, InsFightPetData MyPet, InsFightPetData YouPet, InsFightPetData[] MyPackPet, InsFightPetData[] YouPackPet, InsFightSkill sideSkill)
    {
        if (sideSkill?.Skill == null)
            return;

        // 根据 side 确定自身的精灵数据
        InsFightPetData selfPet = side == EnumWho.My ? MyPet : YouPet;
        if (selfPet == null)
            return;

        // 计算血量损失百分比：自身当前血量每降低10%，技能威力永久提升8点
        int currentHp = selfPet.Hp;
        int maxHp = selfPet.MaxHp;
        if (maxHp <= 0)
            return;

        // 血量损失百分比，取整到10%向下（例如损失35% => 3档 => +24）
        int lostPercent = (int)(((float)(maxHp - currentHp) / maxHp) * 100.0f);
        int lostTens = lostPercent / 10;

        // 最终威力 = 基础威力 + 档数 * 8，干预 sideSkill 伤害值
        int basePower = sideSkill.Skill.AttackValue;
        int finalPower = basePower + lostTens * 8;

        int beforePower = sideSkill.ActualAttackValue;
        sideSkill.ActualAttackValue = finalPower;

        GD.Print($"      [RealtimeSync] DuckSkill0_1_10 | 垂死反击 | {selfPet.PetName} HP={currentHp}/{maxHp} | 损失{lostPercent}%（{lostTens}档） | 威力: {beforePower} → {finalPower}");
    }
}