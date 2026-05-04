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

                #region 交流类
                
                new EntityEvent_2_02(), //交流（一次）
                new EntityEvent_2_03(),//询问
                new EntityEvent_2_04(),//请求
                new EntityEvent_2_05(),//夸奖
                new EntityEvent_2_06(),//安抚
                new EntityEvent_2_07(),//鼓励
                new EntityEvent_2_08(),//那都是你独一无二的装饰
                
                #endregion
                
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
    /// <summary>
    /// 按下按钮直接调用这个方法,然后前端参数直接往里面放就行。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    public void ExcuteOption(E_OptionType type, int index, OptionContext context = null)
    {
        optionPool[type][index].TriggerOption(context);
    }
}
