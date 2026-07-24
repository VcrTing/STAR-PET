using Godot;
using System;
using System.Collections.Generic;
// ════════════════════════════════════════════════════════════════
//  回合运行工具
//  专门处理 FightRunning 相关逻辑：
//   ・构建回合运行数组
//   ・管理当前运行状态
//   ・获取下一阶段、标记完成、重置
// ════════════════════════════════════════════════════════════════

/// <summary>
/// 回合运行工具
/// 负责构建和管理 FightRunning 运行数组，
/// 每回合通过 BuildRunArray 生成 12 长度的步骤数组，
/// 然后依次执行各阶段
/// </summary>
public static class FightRunningBuildTool
{

    /// <summary>
    /// 构建 CurrentRunArray 的结束阶段。
    /// 查找其中的 StartXX，通过 FightRunningTypeDesign.GetEndType 找到 EndXX，
    /// 根据双方 speed 决定先后顺序，依次将 EndXX 加入 CurrentRunArray。
    /// </summary>
    public static void BuildEndSkill()
    {
        // 收集 CurrentRunArray 中所有 StartXX 类型
         List<FightRunning> startList = new  List<FightRunning>();
        for (int i = 0; i < FightRunningHouse.RunArrayLength; i++)
        {
            FightRunning run = FightRunningHouse.CurrentRunArray[i];
            if (run != null && FightRunningTypeDesign.IsStartType(run.RunningType))
            {
                startList.Add(run);
            }
        }

        if (startList.Count == 0)
            return;

        // 获取双方速度
        int mySpeed = FightLandMyStandPet.Instance?.GetSpeed() ?? 20;
        int youSpeed = FightLandYouStandPet.Instance?.GetSpeed() ?? 20;

        // 按 side 分组：先处理 speed 快的一方
        List<FightRunning> myStarts = new List<FightRunning>();
        List<FightRunning> youStarts = new List<FightRunning>();
        for (int i = 0; i < startList.Count; i++)
        {
            if (startList[i].Side == EnumWho.My)
                myStarts.Add(startList[i]);
            else
                youStarts.Add(startList[i]);
        }

        // 速度快的先执行 End
        if (mySpeed >= youSpeed)
        {
            AddEndRunningList(myStarts);
            AddEndRunningList(youStarts);
        }
        else
        {
            AddEndRunningList(youStarts);
            AddEndRunningList(myStarts);
        }
    }


    /// <summary>
    /// 将一组 FightRunning 的 EndXX 版本加入 CurrentRunArray
    /// </summary>
    private static void AddEndRunningList(List<FightRunning> runs)
    {
        for (int i = 0; i < runs.Count; i++)
        {
            FightRunning startRun = runs[i];
            EnumFightRunningType endType = FightRunningTypeDesign.GetEndType(startRun.RunningType);
            if (endType != startRun.RunningType) // 确保有对应的 End 类型
            {
                FightRunningHouse.AddRunning(endType, startRun.Side, startRun.FightSkill, startRun.Damage);
            }
        }
    }

    /// <summary>
    /// 构建检查血量阶段。
    /// 查找 CurrentRunArray 中是否存在 CheckHpMy / CheckHpYou，
    /// 速度慢的一方先检查血量（濒死优先处理）。
    /// </summary>
    public static void BuildCheckHp()
    {
        bool hasMyCheckHp = false;
        bool hasYouCheckHp = false;
        for (int i = 0; i < FightRunningHouse.RunArrayLength; i++)
        {
            FightRunning run = FightRunningHouse.CurrentRunArray[i];
            if (run == null) continue;
            if (run.RunningType == EnumFightRunningType.CheckHpMy) hasMyCheckHp = true;
            if (run.RunningType == EnumFightRunningType.CheckHpYou) hasYouCheckHp = true;
        }

        // 获取双方速度
        int mySpeed = FightLandMyStandPet.Instance?.GetSpeed() ?? 20;
        int youSpeed = FightLandYouStandPet.Instance?.GetSpeed() ?? 20;

        // 速度慢的先检查血量（濒死方优先处理）
        if (hasMyCheckHp && hasYouCheckHp)
        {
            if (mySpeed <= youSpeed)
            {
                FightRunningHouse.AddRunning(EnumFightRunningType.CheckHpMy, EnumWho.My);
                FightRunningHouse.AddRunning(EnumFightRunningType.CheckHpYou, EnumWho.You);
            }
            else
            {
                FightRunningHouse.AddRunning(EnumFightRunningType.CheckHpYou, EnumWho.You);
                FightRunningHouse.AddRunning(EnumFightRunningType.CheckHpMy, EnumWho.My);
            }
        }
        else if (hasMyCheckHp)
        {
            FightRunningHouse.AddRunning(EnumFightRunningType.CheckHpMy, EnumWho.My);
        }
        else if (hasYouCheckHp)
        {
            FightRunningHouse.AddRunning(EnumFightRunningType.CheckHpYou, EnumWho.You);
        }
    }
}
