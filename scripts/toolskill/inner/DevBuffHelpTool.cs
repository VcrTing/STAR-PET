using Godot;

/// <summary>
/// 开发期 Buff 辅助工具
/// 提供 Buff 数组合并/叠加的静态方法
/// </summary>
public static class DevBuffHelpTool
{
    /// <summary>
    /// 将 newBuffs 合并到 oldBuffs 中，相同 buff（Stat + Value + IsRatio 相同）会叠加：
    /// - AliveNum > 0（回合生效类）：叠加 AliveNum + Layer
    /// - AliveNum <= 0（持久类型）：仅叠加 Layer
    /// </summary>
    /// <param name="oldBuffs">原有的 Buff 数组</param>
    /// <param name="newBuffs">要加入的新 Buff 数组</param>
    /// <returns>合并后的新 Buff 数组</returns>
    public static InsFightBuff[] MergeBuffs(InsFightBuff[] oldBuffs, InsFightBuff[] newBuffs)
    {
        // 防护
        if (newBuffs == null || newBuffs.Length == 0)
            return oldBuffs ?? System.Array.Empty<InsFightBuff>();

        if (oldBuffs == null)
            oldBuffs = System.Array.Empty<InsFightBuff>();

        // 先复制旧列表
        var result = new System.Collections.Generic.List<InsFightBuff>(oldBuffs);

        for (int i = 0; i < newBuffs.Length; i++)
        {
            InsFightBuff newBuff = newBuffs[i];

            // 在 result 中查找相同 buff
            int idx = FindSameBuffIndex(result, newBuff);

            if (idx >= 0)
            {
                // 存在相同 buff → 合并
                var existing = result[idx];
                existing.Layer += newBuff.Layer;

                if (newBuff.AliveNum > 0)
                {
                    existing.AliveNum += newBuff.AliveNum;
                }
            }
            else
            {
                // 不存在相同 buff → 追加
                result.Add(newBuff);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// 在列表中查找与 target 具有相同 Stat + Value + IsRatio 的 buff 索引
    /// </summary>
    private static int FindSameBuffIndex(System.Collections.Generic.List<InsFightBuff> list, InsFightBuff target)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Stat == target.Stat
                && list[i].Value == target.Value
                && list[i].IsRatio == target.IsRatio)
            {
                return i;
            }
        }
        return -1;
    }
}