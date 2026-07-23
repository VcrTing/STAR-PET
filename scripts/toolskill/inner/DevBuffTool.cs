using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发期 Buff 工具
/// 提供将配置字典转换为 InsFightBuff 实例的方法
/// </summary>
public static class DevBuffTool
{
    /// <summary>
    /// 从 Godot Dictionary 生成 InsFightBuff 实例
    /// 支持字典格式：
    ///   {"target_stat": 4, "num": 1, "value": 70, "is_ratio": true}
    ///   {"stat": 4, "num": 1, "value": 70, "is_ratio": true}
    /// </summary>
    /// <param name="data">buff 配置字典</param>
    /// <returns>生成的 InsFightBuff 实例，解析失败返回 null</returns>
    public static InsFightBuff CreateFromDict(Godot.Collections.Dictionary data)
    {
        if (data == null || data.Count == 0)
            return null;

        // 兼容 target_stat 和 stat 两种 key 名
        int statId = -1;
        if (data.ContainsKey("stat"))
            statId = (int)data["stat"];

        if (statId < 0 || !Enum.IsDefined(typeof(EnumPetBaseStats), statId))
        {
            GD.PrintErr($"      ❌ DevBuffTool: 无效的 stat 值 ({statId})");
            return null;
        }

        int layer = data.ContainsKey("layer") ? (int)data["layer"] : 1;
        int value = data.ContainsKey("value") ? (int)data["value"] : 0;
        bool isRatio = data.ContainsKey("is_ratio") && (bool)data["is_ratio"];

        return new InsFightBuff
        {
            Stat = (EnumPetBaseStats)statId,
            Layer = layer,
            Value = value,
            IsRatio = isRatio,
        };
    }

    /// <summary>
    /// 从 Godot Array（元素为 Dictionary）批量生成 InsFightBuff 列表
    /// 用于解析 gd 资源中的 gain_buff / gain_buff_bingo 数组
    /// </summary>
    /// <param name="array">buff 配置数组</param>
    /// <returns>InsFightBuff 列表，空数组返回空列表</returns>
    public static List<InsFightBuff> CreateFromArray(Godot.Collections.Array array)
    {
        var list = new List<InsFightBuff>();
        if (array == null || array.Count == 0)
            return list;

        for (int i = 0; i < array.Count; i++)
        {
            var dict = array[i].AsGodotDictionary();
            InsFightBuff buff = CreateFromDict(dict);
            if (buff != null)
                list.Add(buff);
        }
        return list;
    }
}