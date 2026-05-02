using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : ManagerBase<EventManager>
{
    public Dictionary<E_OptionType, OptionBase[]> optionPool = new Dictionary<E_OptionType, OptionBase[]>
    {
        {
            E_OptionType.Level1_Option, new OptionBase[]
            {
                // 狩猎结束！ 把你们统统解体！
                new EntityEvent_1_01() , //应对——闪避ε=ε=ε=(ﾟ◇ﾟﾉ)ﾉ
                new EntityEvent_1_02(), new EntityEvent_1_03(), //交流
                new EntityEvent_1_04(), new EntityEvent_1_05(),//观察
                new EntityEvent_1_06(), new EntityEvent_1_07(),new EntityEvent_1_08(),//对象在许下"装满食物"愿望后
            }
        },
        {
            E_OptionType.Level2_Option, new OptionBase[]
            {
                new EntityEvent_2_01(),//应对——逃避ε=ε=ε=┏(゜ロ゜;)┛
            }
        }
    };
    /// <summary>
    /// 选项是否满足特殊条件
    /// </summary>
    /// <param name="specialConditions"></param>
    /// <returns></returns>
    public bool IsSpecialConditionsMet(E_SpecialOptionConditions specialConditions)
    {
        switch (specialConditions)
        {
            case E_SpecialOptionConditions.Tatters1:
                if (BuffManager.Instance.player.GetBuff(E_BuffType.Tatters) >= 7)
                    return true;
                break;
        }
        return false;
    }

    public OptionBase[] GetOptionPoolByType(E_OptionType type)
    {
        return optionPool[type];
    }
}
