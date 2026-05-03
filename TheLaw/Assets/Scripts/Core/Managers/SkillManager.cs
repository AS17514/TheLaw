using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillManager
{
    // 用字典把所有技能存起来，Key 是 OptionID，Value 是技能实例
    public static readonly Dictionary<int, OptionBase> Skills = new Dictionary<int, OptionBase>();

    // 在游戏初始化时（比如某个 GameManager 的 Awake 里）调用一次
    public static void InitSkills()
    {
        Skills.Clear();
        
        // 把所有的技能在这里 new 一次存进去
        Prepare prepareSkills = new Prepare();
        Skills.Add(prepareSkills.OptionID, prepareSkills); 
        
        Adjust adjustSkill = new Adjust();
        Skills.Add(adjustSkill.OptionID, adjustSkill); 
        
        Overturn overturnSkill = new Overturn();
        Skills.Add(overturnSkill.OptionID, overturnSkill);
        
        Atk atkSkill = new Atk();
        Skills.Add(atkSkill.OptionID, atkSkill);
        
        ChantingLaw chantingLawSkill = new ChantingLaw();
        Skills.Add(chantingLawSkill.OptionID, chantingLawSkill);

        GunArt3 gunArt3Skill = new GunArt3();
        Skills.Add(gunArt3Skill.OptionID, gunArt3Skill);
        
        ShatteredStars shatteredStarsSkill = new ShatteredStars();
        Skills.Add(shatteredStarsSkill.OptionID, shatteredStarsSkill);
        
        Abundance abundanceSkill = new Abundance();
        Skills.Add(abundanceSkill.OptionID, abundanceSkill);
        
        Colorfull colorfullSkill = new Colorfull();
        Skills.Add(colorfullSkill.OptionID, colorfullSkill);

    }
/// <summary>
/// 前端调用技能管理器的这个方法来执行玩家技能,同时部分技能需要传参数。
/// </summary>
/// <param name="optionID"></param>
/// <param name="optionContext"></param>
    public static void ExcuteSkills(int optionID,OptionContext optionContext=null)
    {
        Skills[optionID].TriggerOption(optionContext);
    }
}
