using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 开发期能力（Power）工具
/// 提供将配置字典转换为 InsFightPower 实例的方法
/// </summary>
public static class DevPowerTool
{
    /// <summary>
    /// 从 Godot Dictionary 生成 InsFightPower 实例
    /// 支持字典格式：
    ///   {"target_power": 1, "layer": 1, "value": 1, "is_ratio": false}
    ///   {"power_type": 1, "layer": 1, "value": 1, "is_ratio": false}
    /// </summary>
    /// <param name="data">能力配置字典</param>
    /// <returns>生成的 InsFightPower 实例，解析失败返回 null</returns>
    public static InsFightPower CreateFromDict(Godot.Collections.Dictionary data)
    {
        if (data == null || data.Count == 0)
            return null;

        // 兼容 power_type 和 target_power 两种 key 名
        int powerTypeId = -1;
        if (data.ContainsKey("power_type"))
            powerTypeId = (int)data["power_type"];
        else if (data.ContainsKey("target_power"))
            powerTypeId = (int)data["target_power"];

        if (powerTypeId < 0 || !Enum.IsDefined(typeof(EnumFightPowerType), powerTypeId))
        {
            GD.PrintErr($"      ❌ DevPowerTool: 无效的 power_type 值 ({powerTypeId})");
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

        return new InsFightPower
        {
            PowerType = (EnumFightPowerType)powerTypeId,
            Layer = layer,
            Value = value,
            IsRatio = isRatio,
            ActiveMode = activeMode,
        };
    }

    /// <summary>
    /// 从 Godot Array（元素为 Dictionary）批量生成 InsFightPower 列表
    /// 用于解析 gd 资源中的 gain_power / gain_power_bingo 数组
    /// </summary>
    /// <param name="array">能力配置数组</param>
    /// <returns>InsFightPower 列表，空数组返回空列表</returns>
    public static List<InsFightPower> CreateFromArray(Godot.Collections.Array array)
    {
        var list = new List<InsFightPower>();
        if (array == null || array.Count == 0)
            return list;

        for (int i = 0; i < array.Count; i++)
        {
            var dict = array[i].AsGodotDictionary();
            InsFightPower power = CreateFromDict(dict);
            if (power != null)
                list.Add(power);
        }
        return list;
    }

    /// <summary>
    /// 制作能力并添加到敌方精灵身上（技能副作用通用方法）
    /// 根据 run.Side 确定敌方：My -> 敌方 You 精灵，You -> 敌方 My 精灵。
    /// 自动完成：创建能力 -> 绑定敌方场上精灵 UUID -> 加入敌方 PowerManager。
    /// </summary>
    /// <param name="data">能力配置字典（支持 power_type/target_power、layer、value、is_ratio、active_mode）</param>
    /// <param name="run">当前回合运行数据（用于确定敌方与能力归属）</param>
    /// <returns>成功添加的能力数量，失败返回 0</returns>
    public static int AddPowerToTargetPet(Godot.Collections.Dictionary data, FightRunning run)
    {
        if (data == null || data.Count == 0 || run == null)
            return 0;

        InsFightPower power = CreateFromDict(data);
        if (power == null)
            return 0;

        // 确定敌方场上精灵 UUID
        string enemyPetUuid = null;
        if (run.Side == EnumWho.My)
            enemyPetUuid = FightLandYouStandPet.Instance?.FightPetData?.PetUuid;
        else
            enemyPetUuid = FightLandMyStandPet.Instance?.FightPetData?.PetUuid;

        if (string.IsNullOrWhiteSpace(enemyPetUuid))
        {
            GD.PrintErr($"      ❌ DevPowerTool.AddPowerToTargetPet: 敌方场上精灵 UUID 为空，无法添加能力");
            return 0;
        }

        power.PetUuid = enemyPetUuid;

        // 将能力添加到敌方 PowerManager
        if (run.Side == EnumWho.My)
            FightYouStandPowerManager.Instance?.AddPowers(new[] { power });
        else
            FightMyStandPowerManager.Instance?.AddPowers(new[] { power });

        return 1;
    }
}