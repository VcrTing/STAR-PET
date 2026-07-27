using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// 系统技能执行工具
/// 负责执行换宠等系统类技能
/// </summary>
public static class FightSkillSystemTool
{
    /// <summary>
    /// 执行换宠操作
    /// </summary>
    /// <param name="sideSkill">系统技能（换宠技能）</param>
    /// <param name="side">使用方标识</param>
    /// <param name="targetIndex">要切换到的宠物索引</param>
    public static void ExecSwitchPet(
        InsFightSkill sideSkill,
        EnumWho side,
        int targetIndex, TurnAction[] otherOneActions)
    {
        if (sideSkill?.Skill == null) return;
        // 提取应对
        int bingoType = sideSkill.Skill.BingoSkillType;

        string label = side == EnumWho.My ? "🧑我方" : "👹敌方";
        GD.Print($"      → [FightSkillSystemTool.ExecSwitchPet] {label} 准备换宠");

        InsFightPetData willSwitchPet = GetSwitchPet(targetIndex, side);

        // 检查应对
        InsFightSkill[] sideSkills = FightSkillJudge2Tool.GetFightSkillFromActions(otherOneActions);
        for (int i = 0; i < sideSkills.Length; i++)
        {
            InsFightSkill otherOneSkill = sideSkills[i];
            if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.ATTACK)
            {
                // 检查应对攻击
                if (bingoType == (int)EnumSkillType.ATTACK)
                {
                    // 应对攻击，没什么影响
                }
                // 1. side 开始换宠，FightRunning 加入CurrentRunArray
                FightRunningHouse.AddRunningSys(
                    side == EnumWho.My ? EnumFightRunningType.SwitchPetMy : EnumFightRunningType.SwitchPetYou,
                    side, sideSkill, willSwitchPet, null);
                FightSkillJudge2Tool.SideSystemWhenOtherOneAttack(sideSkill, side, otherOneSkill);
            }
            // 应对防御
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.DEFENSE)
            {
                // 检查应对防御
                if (bingoType == (int)EnumSkillType.DEFENSE)
                {
                }
                // 1. side 开始换宠，FightRunning 加入CurrentRunArray
                FightRunningHouse.AddRunningSys(
                    side == EnumWho.My ? EnumFightRunningType.SwitchPetMy : EnumFightRunningType.SwitchPetYou,
                    side, sideSkill, willSwitchPet, null);
                // 3. side 计算自己要扣的血量，应对防御暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.STATUS)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.STATUS)
                {
                    // 应对状态，暂无作用
                }
                // 1. side 开始换宠，FightRunning 加入CurrentRunArray
                FightRunningHouse.AddRunningSys(
                    side == EnumWho.My ? EnumFightRunningType.SwitchPetMy : EnumFightRunningType.SwitchPetYou,
                    side, sideSkill, willSwitchPet, null);
                // 2. side 计算自己要扣的血量，应对状态暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
            // 系统 应对 系统
            else if (otherOneSkill.Skill.SkillType == (int)EnumSkillType.SYSTEM)
            {
                // 检查应对状态
                if (bingoType == (int)EnumSkillType.SYSTEM)
                {
                    // 应对状态，暂无作用
                }
                // 1. side 开始换宠，FightRunning 加入CurrentRunArray
                FightRunningHouse.AddRunningSys(
                    side == EnumWho.My ? EnumFightRunningType.SwitchPetMy : EnumFightRunningType.SwitchPetYou,
                    side, sideSkill, willSwitchPet, null);
                // 2. side 计算自己要扣的血量，应对状态暂时 = 0，FightRunning 加入CurrentRunArray
                int basicDamag = 0;
                FightRunningHouse.AddRunning2(
                    side == EnumWho.My ? EnumFightRunningType.DoDamageMy : EnumFightRunningType.DoDamageYou,
                    side, sideSkill, basicDamag, otherOneSkill);
            }
        }
    }

    static InsFightPetData GetSwitchPet(int targetIndex, EnumWho side)
    {
        if (side == EnumWho.My)
        {
            var pets = PlayerLandMyStandPlayer.Instance.FightPets;
            if (pets != null && targetIndex >= 0 && targetIndex < pets.Count && pets[targetIndex].Hp > 0)
            {
                return pets[targetIndex];
            }
        }
        else if (side == EnumWho.You)
        {
            var pets = PlayerLandYouStandPlayer.Instance.FightPets;
            if (pets != null && targetIndex >= 0 && targetIndex < pets.Count && pets[targetIndex].Hp > 0)
            {
                return pets[targetIndex];
            }
        }
        GD.Print("无宠物");
        return null;
    }
}