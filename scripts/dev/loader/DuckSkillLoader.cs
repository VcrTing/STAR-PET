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
}