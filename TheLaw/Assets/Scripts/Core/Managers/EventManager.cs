using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : ManagerBase<EventManager>
{
    public Dictionary<E_OptionType, OptionBase[]> optionPool = new Dictionary<E_OptionType, OptionBase[]>
    {
        { E_OptionType.Level1_Option, new OptionBase[] { new EntityEvent_1_01() , new EntityEvent_1_02(), new EntityEvent_1_03() } },
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
