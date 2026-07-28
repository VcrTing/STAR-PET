using Godot;

/// <summary>
/// 回合运行执行系统工具
/// 提供系统类阶段（如切换宠物）的可复用执行方法
/// </summary>
public static class FightRunningExeSysTool
{
    /// <summary>
    /// 执行主动切换宠物阶段
    /// 
    /// ⚠️ 切换宠物没那么简单，目前只做了基础换宠操作（销毁旧Wrapper→设置新数据→创建新Wrapper→刷新技能UI），
    /// 但还有很多事情需要考虑和完善：
    /// 
    /// 1. 【Buff 清理】旧精灵离场时，应清除该精灵身上除 ThisPetPermanent（永久）之外的所有 Buff，
    ///    调用 FightMyStandBuffManager.WhenPetDisAppear() 
    ///    或 FightYouStandBuffManager.WhenPetDisAppear()
    /// 
    /// 2. 【Buff 恢复】新精灵上场后，应重新应用该精灵本应享有的 Buff（如全局/永久型 Buff），
    ///    确保新精灵的属性面板正确叠加了 Buff 加成。
    /// 
    /// 3. 【HP/状态同步】换宠前旧精灵的 HP、PP、异常状态等数据需要正确保存到背包数据中，
    ///    换宠后新精灵的 HP、PP、异常状态等数据需要从背包正确恢复到场上。
    /// 
    /// 4. 【回合行动权】换宠是否会消耗本回合行动权？目前 TurnActionType.SwitchPet 的优先级是 4（较低），
    ///    但实际规则中换宠可能允许先手或被对手攻击打断，取决于技能效果。
    /// 
    /// 5. 【换宠技能联动】通过技能换宠（如"急速轮换"类技能）时，可能还附带额外效果
    ///   （如提升新精灵某属性、或触发特定特性），目前仅在 FightSkillSystemTool.ExecSwitchPet 中
    ///   做了基础的 Running 阶段编排，没有处理技能后续效果。
    /// 
    /// 6. 【换宠动画/表现】目前只是数据层面的切换，没有播放换宠出场/退场动画。
    /// 
    /// 7. 【背包 UI 同步】换宠后需要关闭背包选择面板（已做：PanFightPlayerPack.Instance?.Close()），
    ///    但还应刷新背包中精灵的选中状态和可用性标记。
    /// 
    /// 8. 【敌方 AI 换宠】敌方换宠时（FightLandYouStandPet.SwitchPet），同样需要处理上述逻辑，
    ///    且 AI 换宠时机与玩家不同，应确保一致性。
    /// 
    /// ✅ 已实现：
    /// - BeforeSwitch: 清除旧精灵的非永久 Buff（WhenPetDisAppear）
    /// - AfterSwitch:  刷新新精灵的 Buff UI 视图（WhenPetAppear）
    /// </summary>
    public static void ExecuteSwitchPet(FightRunning run, int index)
    {
        if (run.SwitchPet == null) return;

        string sideLabel = run.Side == EnumWho.My ? "🧑我方" : "👹敌方";
        GD.Print($"      [{index}] {sideLabel} 切换宠物 → {run.SwitchPet.PetName}");

        // 1. 【Buff 清理】旧精灵离场，清除其除 ThisPetPermanent 之外的所有 Buff
        if (run.Side == EnumWho.My)
        {
            InsFightPetData oldPet = FightLandMyStandPet.Instance?.FightPetData;
            if (oldPet != null)
                FightMyStandBuffManager.Instance?.WhenPetDisAppear(oldPet);
        }
        else
        {
            InsFightPetData oldPet = FightLandYouStandPet.Instance?.FightPetData;
            if (oldPet != null)
                FightYouStandBuffManager.Instance?.WhenPetDisAppear(oldPet);
        }

        // 2. 执行切换宠物
        if (run.Side == EnumWho.My)
            FightLandMyStandPet.Instance?.SwitchPet(run.SwitchPet);
        else
            FightLandYouStandPet.Instance?.SwitchPet(run.SwitchPet);

        // 3. 【Buff 恢复】新精灵上场后，刷新该精灵的 Buff UI 视图
        if (run.Side == EnumWho.My)
            FightMyStandBuffManager.Instance?.WhenPetAppear(run.SwitchPet);
        else
            FightYouStandBuffManager.Instance?.WhenPetAppear(run.SwitchPet);
    }
}