using Godot;

/// <summary>
/// 开发期 Power（能力）辅助工具
/// 提供 Power 数组合并/叠加的静态方法
/// </summary>
public static class DevPowerDesign
{
    /// <summary>
    /// 将 newPowers 合并到 oldPowers 中，相同 power（PowerType + Value + IsRatio 相同）会叠加：
    /// - AliveNum > 0（回合生效类）：叠加 AliveNum + Layer
    /// - AliveNum <= 0（持久类型）：仅叠加 Layer
    /// </summary>
    /// <param name="oldPowers">原有的 Power 数组</param>
    /// <param name="newPowers">要加入的新 Power 数组</param>
    /// <returns>合并后的新 Power 数组</returns>
    public static InsFightPower[] MergePowers(InsFightPower[] oldPowers, InsFightPower[] newPowers)
    {
        // 防护
        if (newPowers == null || newPowers.Length == 0)
            return oldPowers ?? System.Array.Empty<InsFightPower>();

        if (oldPowers == null)
            oldPowers = System.Array.Empty<InsFightPower>();

        // 先复制旧列表
        var result = new System.Collections.Generic.List<InsFightPower>(oldPowers);

        for (int i = 0; i < newPowers.Length; i++)
        {
            InsFightPower newPower = newPowers[i];

            // 在 result 中查找相同 power
            int idx = FindSamePowerIndex(result, newPower);

            if (idx >= 0)
            {
                // 存在相同 power → 合并
                var existing = result[idx];
                existing.Layer += newPower.Layer;

                if (newPower.AliveNum > 0)
                {
                    existing.AliveNum += newPower.AliveNum;
                }
            }
            else
            {
                // 不存在相同 power → 追加
                result.Add(newPower);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// 将 InsFightPower 转换为可读文本
    /// 格式：能力名 （layer正数？+ ： -）（isRatio？（layer*value %）: (layer*value 点)）
    /// 示例："先手值 +70%" 或 "连击数 -1点"
    /// </summary>
    /// <param name="power">Power 数据</param>
    /// <returns>格式化后的文本</returns>
    public static string PowerToText(InsFightPower power)
    {
        if (power == null)
            return "";

        int an = power.AliveNum;
        string end = an > 0 ? " " + an : "";
        string powerTypeName = GetPowerTypeName(power.PowerType);
        string sign = power.Layer >= 0 ? "+" : "-";

        int totalValue = System.Math.Abs(power.Layer) * power.Value;
        string valueStr = power.IsRatio
            ? $"{totalValue}%"
            : $"{totalValue}点";

        return $"{powerTypeName}:{sign}{valueStr} {end}";
    }

    /// <summary>
    /// 获取能力类型中文名称
    /// </summary>
    public static string GetPowerTypeName(EnumFightPowerType powerType)
    {
        return powerType switch
        {
            EnumFightPowerType.RoundPriority => "先手值",
            EnumFightPowerType.ComboCount => "连击数",
            _ => $"未知({(int)powerType})"
        };
    }

    /// <summary>
    /// 在列表中查找与 target 具有相同 PowerType + Value + IsRatio 的 power 索引
    /// </summary>
    private static int FindSamePowerIndex(System.Collections.Generic.List<InsFightPower> list, InsFightPower target)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].PowerType == target.PowerType
                && list[i].Value == target.Value
                && list[i].IsRatio == target.IsRatio)
            {
                return i;
            }
        }
        return -1;
    }
}