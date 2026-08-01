using Godot;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

public static class DuckSkillLoader
{
    // 实例缓存，避免反复反射创建
    private static readonly Dictionary<string, object> _skillInstanceCache = new();
    // 程序集缓存一次获取
    private static readonly Assembly _gameAssembly = Assembly.GetExecutingAssembly();

    public static void ExecuteDuckSkill(string implCsFilePath, int index, FightRunning run, InsFightSkill sideSkill)
    {
        if (string.IsNullOrWhiteSpace(implCsFilePath) || !File.Exists(ProjectSettings.GlobalizePath(implCsFilePath)))
        {
            GD.PrintErr($"技能脚本文件不存在：{implCsFilePath}");
            return;
        }

        // 命中缓存直接拿实例
        if (_skillInstanceCache.TryGetValue(implCsFilePath, out var cachedIns))
        {
            CallDoSkill(cachedIns, index, run, sideSkill);
            return;
        }

        try
        {
            // 从文件路径提取类名：文件名叫 DuckSkill0_3_1.cs → 类名 DuckSkill0_3_1
            string fileName = Path.GetFileNameWithoutExtension(implCsFilePath);
            // 在当前程序集查找这个类
            Type targetType = _gameAssembly.GetType(fileName);
            if (targetType == null)
            {
                GD.PrintErr($"程序集中找不到类：{fileName} （脚本路径：{implCsFilePath}）");
                return;
            }

            // 无参构造实例化
            object skillObj = Activator.CreateInstance(targetType);
            _skillInstanceCache[implCsFilePath] = skillObj;

            // 鸭子调用方法
            CallDoSkill(skillObj, index, run, sideSkill);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"加载/执行鸭子技能异常 {implCsFilePath}\n{ex.Message}\n{ex.StackTrace}");
        }
    }

    /// 统一鸭子调用封装
    private static void CallDoSkill(object skillObj, int index, FightRunning run, InsFightSkill sideSkill)
    {
        dynamic dyn = skillObj;
        dyn.DoSkill(index, run, sideSkill);
    }

    /// <summary>
    /// 执行技能实现类的 RebuildTurn 鸭子方法
    /// 传入双方行动数组与 side，调用 DuckSkill 类的 RebuildTurn(myTurnActions, youTurnActions, side) 返回重构后的行动数组
    /// </summary>
    /// <param name="implCsFilePath">实现脚本路径（res://define/dataskill/.../DuckSkillXxx.cs）</param>
    /// <param name="myTurnActions">我方行动数组</param>
    /// <param name="youTurnActions">敌方行动数组</param>
    /// <param name="side">要返回的行动方</param>
    /// <returns>重构后的 side 行动数组；脚本不存在/类找不到/异常时返回 null</returns>
    public static TurnAction[] ExecuteRebuildTurn(string implCsFilePath, TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        if (string.IsNullOrWhiteSpace(implCsFilePath) || !File.Exists(ProjectSettings.GlobalizePath(implCsFilePath)))
        {
            GD.PrintErr($"技能脚本文件不存在：{implCsFilePath}");
            return null;
        }

        // 命中缓存直接拿实例
        if (_skillInstanceCache.TryGetValue(implCsFilePath, out var cachedIns))
        {
            return CallRebuildTurn(cachedIns, myTurnActions, youTurnActions, side);
        }

        try
        {
            // 从文件路径提取类名：文件名叫 DuckSkill0_3_1.cs → 类名 DuckSkill0_3_1
            string fileName = Path.GetFileNameWithoutExtension(implCsFilePath);
            // 在当前程序集查找这个类
            Type targetType = _gameAssembly.GetType(fileName);
            if (targetType == null)
            {
                GD.PrintErr($"程序集中找不到类：{fileName} （脚本路径：{implCsFilePath}）");
                return null;
            }

            // 无参构造实例化，并复用 DoSkill 的实例缓存
            object skillObj = Activator.CreateInstance(targetType);
            _skillInstanceCache[implCsFilePath] = skillObj;

            // 鸭子调用方法
            return CallRebuildTurn(skillObj, myTurnActions, youTurnActions, side);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"加载/执行鸭子技能异常 {implCsFilePath}\n{ex.Message}\n{ex.StackTrace}");
            return null;
        }
    }

    /// 统一 RebuildTurn 鸭子调用封装
    private static TurnAction[] CallRebuildTurn(object skillObj, TurnAction[] myTurnActions, TurnAction[] youTurnActions, EnumWho side)
    {
        dynamic dyn = skillObj;
        return dyn.RebuildTurn(myTurnActions, youTurnActions, side);
    }

    /// <summary>
    /// 执行技能实现类的 RealtimeSync 鸭子方法（实时刷新技能）
    /// 参数由外部传入
    /// </summary>
    /// <param name="implCsFilePath">实现脚本路径</param>
    /// <param name="side">阵营</param>
    /// <param name="myPet">我方场上当前精灵</param>
    /// <param name="youPet">敌方场上当前精灵</param>
    /// <param name="myPackPet">我方背包精灵数组</param>
    /// <param name="youPackPet">敌方背包精灵数组</param>
    /// <param name="sideSkill">当前技能实例（通过它修改技能源头）</param>
    public static void ExecuteRealtimeSync(string implCsFilePath, EnumWho side,
        InsFightPetData myPet, InsFightPetData youPet,
        InsFightPetData[] myPackPet, InsFightPetData[] youPackPet,
        InsFightSkill sideSkill)
    {
        if (sideSkill == null)
        {
            GD.PrintErr("[DuckSkillLoader.ExecuteRealtimeSync] sideSkill 为空");
            return;
        }

        if (string.IsNullOrWhiteSpace(implCsFilePath) || !File.Exists(ProjectSettings.GlobalizePath(implCsFilePath)))
        {
            GD.PrintErr($"技能脚本文件不存在：{implCsFilePath}");
            return;
        }

        // 命中缓存直接拿实例
        if (_skillInstanceCache.TryGetValue(implCsFilePath, out var cachedIns))
        {
            CallRealtimeSync(cachedIns, side, myPet, youPet, myPackPet, youPackPet, sideSkill);
            return;
        }

        try
        {
            string fileName = Path.GetFileNameWithoutExtension(implCsFilePath);
            Type targetType = _gameAssembly.GetType(fileName);
            if (targetType == null)
            {
                GD.PrintErr($"程序集中找不到类：{fileName} （脚本路径：{implCsFilePath}）");
                return;
            }

            object skillObj = Activator.CreateInstance(targetType);
            _skillInstanceCache[implCsFilePath] = skillObj;

            CallRealtimeSync(skillObj, side, myPet, youPet, myPackPet, youPackPet, sideSkill);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"加载/执行鸭子技能 RealtimeSync 异常 {implCsFilePath}\n{ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// 统一 RealtimeSync 鸭子调用封装
    /// </summary>
    private static void CallRealtimeSync(object skillObj, EnumWho side,
        InsFightPetData myPet, InsFightPetData youPet,
        InsFightPetData[] myPackPet, InsFightPetData[] youPackPet,
        InsFightSkill sideSkill)
    {
        dynamic dyn = skillObj;
        dyn.RealtimeSync(side, myPet, youPet, myPackPet, youPackPet, sideSkill);
    }

    /// <summary>
    /// 执行技能实现类的 StartSkill 鸭子方法（技能阶段开始）
    /// </summary>
    /// <param name="implCsFilePath">实现脚本路径</param>
    /// <param name="index">阶段索引号（仅用于日志）</param>
    /// <param name="run">战斗运行实例</param>
    /// <param name="sideSkill">当前技能实例</param>
    public static void ExecuteStartSkill(string implCsFilePath, int index, FightRunning run, InsFightSkill sideSkill)
    {
        if (sideSkill == null)
        {
            GD.PrintErr("[DuckSkillLoader.ExecuteStartSkill] sideSkill 为空");
            return;
        }

        if (string.IsNullOrWhiteSpace(implCsFilePath) || !File.Exists(ProjectSettings.GlobalizePath(implCsFilePath)))
        {
            GD.PrintErr($"技能脚本文件不存在：{implCsFilePath}");
            return;
        }

        // 命中缓存直接拿实例
        if (_skillInstanceCache.TryGetValue(implCsFilePath, out var cachedIns))
        {
            CallStartSkill(cachedIns, index, run, sideSkill);
            return;
        }

        try
        {
            string fileName = Path.GetFileNameWithoutExtension(implCsFilePath);
            Type targetType = _gameAssembly.GetType(fileName);
            if (targetType == null)
            {
                GD.PrintErr($"程序集中找不到类：{fileName} （脚本路径：{implCsFilePath}）");
                return;
            }

            object skillObj = Activator.CreateInstance(targetType);
            _skillInstanceCache[implCsFilePath] = skillObj;

            CallStartSkill(skillObj, index, run, sideSkill);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"加载/执行鸭子技能 StartSkill 异常 {implCsFilePath}\n{ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// 统一 StartSkill 鸭子调用封装
    /// </summary>
    private static void CallStartSkill(object skillObj, int index, FightRunning run, InsFightSkill sideSkill)
    {
        dynamic dyn = skillObj;
        dyn.StartSkill(index, run, sideSkill);
    }

    /// <summary>
    /// 执行技能实现类的 EndSkill 鸭子方法（技能阶段结束）
    /// </summary>
    /// <param name="implCsFilePath">实现脚本路径</param>
    /// <param name="index">阶段索引号（仅用于日志）</param>
    /// <param name="run">战斗运行实例</param>
    /// <param name="sideSkill">当前技能实例</param>
    public static void ExecuteEndSkill(string implCsFilePath, int index, FightRunning run, InsFightSkill sideSkill)
    {
        if (sideSkill == null)
        {
            GD.PrintErr("[DuckSkillLoader.ExecuteEndSkill] sideSkill 为空");
            return;
        }

        if (string.IsNullOrWhiteSpace(implCsFilePath) || !File.Exists(ProjectSettings.GlobalizePath(implCsFilePath)))
        {
            GD.PrintErr($"技能脚本文件不存在：{implCsFilePath}");
            return;
        }

        // 命中缓存直接拿实例
        if (_skillInstanceCache.TryGetValue(implCsFilePath, out var cachedIns))
        {
            CallEndSkill(cachedIns, index, run, sideSkill);
            return;
        }

        try
        {
            string fileName = Path.GetFileNameWithoutExtension(implCsFilePath);
            Type targetType = _gameAssembly.GetType(fileName);
            if (targetType == null)
            {
                GD.PrintErr($"程序集中找不到类：{fileName} （脚本路径：{implCsFilePath}）");
                return;
            }

            object skillObj = Activator.CreateInstance(targetType);
            _skillInstanceCache[implCsFilePath] = skillObj;

            CallEndSkill(skillObj, index, run, sideSkill);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"加载/执行鸭子技能 EndSkill 异常 {implCsFilePath}\n{ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// 统一 EndSkill 鸭子调用封装
    /// </summary>
    private static void CallEndSkill(object skillObj, int index, FightRunning run, InsFightSkill sideSkill)
    {
        dynamic dyn = skillObj;
        dyn.EndSkill(index, run, sideSkill);
    }
}
