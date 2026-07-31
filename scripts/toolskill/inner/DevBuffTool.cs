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
        else if (data.ContainsKey("target_stat"))
            statId = (int)data["target_stat"];

        if (statId < 0 || !Enum.IsDefined(typeof(EnumPetBaseStats), statId))
        {
            GD.PrintErr($"      ❌ DevBuffTool: 无效的 stat 值 ({statId})");
            return null;
        }

        int layer = data.ContainsKey("layer") ? (int)data["layer"] : 1;
        int value = data.ContainsKey("value") ? (int)data["value"] : 0;
        bool isRatio = data.ContainsKey("is_ratio") && (bool)data["is_ratio"];

        // 解析生效模式，默认 ThisPetAppear = 5
        EnumBuffActiveMode activeMode = EnumBuffActiveMode.ThisPetAppear;
        if (data.ContainsKey("active_mode"))
        {
            int modeVal = (int)data["active_mode"];
            if (System.Enum.IsDefined(typeof(EnumBuffActiveMode), modeVal))
                activeMode = (EnumBuffActiveMode)modeVal;
        }

        return new InsFightBuff
        {
            Stat = (EnumPetBaseStats)statId,
            Layer = layer,
            Value = value,
            IsRatio = isRatio,
            ActiveMode = activeMode,
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

    /// <summary>
    /// 制作 Buff 并添加到敌方精灵身上（技能副作用通用方法）
    /// 根据 run.Side 确定敌方：My -> 敌方 You 精灵，You -> 敌方 My 精灵。
    /// 自动完成：创建 Buff -> 绑定敌方场上精灵 UUID -> 加入敌方 BuffManager。
    /// </summary>
    /// <param name="data">buff 配置字典（支持 stat/target_stat、num、layer、value、is_ratio、active_mode）</param>
    /// <param name="run">当前回合运行数据（用于确定敌方与 Buff 归属）</param>
    /// <returns>成功添加的 Buff 数量，失败返回 0</returns>
    public static int AddBuffToTargetPet(Godot.Collections.Dictionary data, FightRunning run)
    {
        if (data == null || data.Count == 0 || run == null)
            return 0;

        InsFightBuff buff = CreateFromDict(data);
        if (buff == null)
            return 0;

        // 确定敌方场上精灵 UUID
        string enemyPetUuid = null;
        if (run.Side == EnumWho.My)
            enemyPetUuid = FightLandYouStandPet.Instance?.FightPetData?.PetUuid;
        else
            enemyPetUuid = FightLandMyStandPet.Instance?.FightPetData?.PetUuid;

        if (string.IsNullOrWhiteSpace(enemyPetUuid))
        {
            GD.PrintErr($"      ❌ DevBuffTool.AddBuffToEnemy: 敌方场上精灵 UUID 为空，无法添加 Buff");
            return 0;
        }

        buff.PetUuid = enemyPetUuid;

        // 将 Buff 添加到敌方 BuffManager
        if (run.Side == EnumWho.My)
            FightYouStandBuffManager.Instance?.AddBuffs(new[] { buff });
        else
            FightMyStandBuffManager.Instance?.AddBuffs(new[] { buff });

        return 1;
    }
}
