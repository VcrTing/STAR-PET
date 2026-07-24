using Godot;
using System;
using System.Collections.Generic;

public static class DevSkillCompuTool {
    
    // 伤害计算减伤率
    public static int DamageBeDefense(int originDamage, int damageReductionRate)
    {
        if (damageReductionRate <= 0) { GD.Print("是不是忘记定义减伤率了"); damageReductionRate = 0; }
        if (damageReductionRate > 100) { damageReductionRate = 100; }
        //
        int finalDamage = originDamage - (int)(originDamage * ((float)damageReductionRate / 100));
        return finalDamage < 0 ? 0 : finalDamage;
    }
}