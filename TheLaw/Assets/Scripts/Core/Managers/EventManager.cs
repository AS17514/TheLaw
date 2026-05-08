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
                new EntityEvent_2_09(),//观察
                
                //对象在许下"藏在衣服的后面"愿望后
                new EntityEvent_2_10(),//推动
                new EntityEvent_2_11(),//无视
                
                //对象在许下"如果那也算我的衣服"愿望后
                new EntityEvent_2_12(),//那不是你的伪装
                new EntityEvent_2_13(),//中伤
            }
        },
        {
            E_OptionType.Level3_Option, new OptionBase[]
            {
                new EntityEvent_3_01(),//应对(球或者弹珠)——弹回(╯°□°）╯︵ ⚾
                new EntityEvent_3_02(),//应对——拥抱(つ≧▽≦)つ
                new EntityEvent_3_03(),//应对(毛球1)——弹回(っ・ω・)っ⚾
                new EntityEvent_3_04(),//应对(毛球2)——弹回( ﾟ∀ﾟ)つ⚾
                new EntityEvent_3_05(),//应对(毛球3)——弹回(ヘ･_･)ヘ┳━┳  ⚾
                new EntityEvent_3_06(),//应对(毛球4)——弹回∑(ﾟДﾟノ)ノ⚾
                
                new EntityEvent_3_07(),//奔跑
                new EntityEvent_3_08(),//交流
                new EntityEvent_3_09(),//观察
                new EntityEvent_3_10(),//观察
                new EntityEvent_3_11(),//观察
                new EntityEvent_3_12(),//聆听
                new EntityEvent_3_13(),//捷径
                new EntityEvent_3_14(),//一同探究
                new EntityEvent_3_15(),//你的好奇永远是你的自由
                
            }
        },
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
    
    /// <summary>
    /// 重新注册选项池数据
    /// 每次调用都会清空旧数据并重新写入
    /// </summary>
    public void RegisterOptions(int level)
    {
        // 每次输入时先清空原本存的东西
        optionPool.Clear();

        // 重新写入内容

        switch (level)
        {
            case 1:
                optionPool.Add(E_OptionType.Level1_Option, new OptionBase[]
                {
                    // 狩猎结束！ 把你们统统解体！
                    new EntityEvent_1_01() , //应对——闪避ε=ε=ε=(ﾟ◇ﾟﾉ)ﾉ
                    new EntityEvent_1_02(), new EntityEvent_1_03(), //交流
                    new EntityEvent_1_04(), new EntityEvent_1_05(),//观察
                    new EntityEvent_1_06(), new EntityEvent_1_07(),new EntityEvent_1_08(),//对象在许下"装满食物"愿望后
                });
                break;
            case 2:
                optionPool.Add(E_OptionType.Level2_Option, new OptionBase[]
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
                    new EntityEvent_2_09(),//观察
            
                    //对象在许下"藏在衣服的后面"愿望后
                    new EntityEvent_2_10(),//推动
                    new EntityEvent_2_11(),//无视
            
                    //对象在许下"如果那也算我的衣服"愿望后
                    new EntityEvent_2_12(),//那不是你的伪装
                    new EntityEvent_2_13(),//中伤
                });
                break;
            case 3:
                optionPool.Add(E_OptionType.Level3_Option, new OptionBase[]
                {
                    new EntityEvent_3_01(), //应对(球或者弹珠)——弹回(╯°□°）╯︵ ⚾
                    new EntityEvent_3_02(), //应对——拥抱(つ≧▽≦)つ
                    new EntityEvent_3_03(), //应对(毛球1)——弹回(っ・ω・)っ⚾
                    new EntityEvent_3_04(), //应对(毛球2)——弹回( ﾟ∀ﾟ)つ⚾
                    new EntityEvent_3_05(), //应对(毛球3)——弹回(ヘ･_･)ヘ┳━┳  ⚾
                    new EntityEvent_3_06(), //应对(毛球4)——弹回∑(ﾟДﾟノ)ノ⚾

                    new EntityEvent_3_07(), //奔跑
                    new EntityEvent_3_08(), //交流
                    new EntityEvent_3_09(), //观察
                    new EntityEvent_3_10(), //观察
                    new EntityEvent_3_11(), //观察
                    new EntityEvent_3_12(), //聆听
                    new EntityEvent_3_13(), //捷径
                    new EntityEvent_3_14(), //一同探究
                    new EntityEvent_3_15(), //你的好奇永远是你的自由
                });
                break;
                
        }
    }
}
