using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionExecutedArgs
{
    public E_OptionType OptionType; // 哪个关卡的选项池，如 Level5_Option
    public int OptionID; // 哪个选项，如 11 对应"回忆"
}

public class EventManager : ManagerBase<EventManager>
{

    #region  震撼亚洲的新框架的代码部分喵

    private List<OptionBase> pendingResponseOptions = new List<OptionBase>();

    public void RegisterPendingResponse(OptionBase option)
    {
        if (!pendingResponseOptions.Contains(option))
            pendingResponseOptions.Add(option);
    }

    public void UnregisterPendingResponse(OptionBase option)
    {
        pendingResponseOptions.Remove(option);
    }

    private void FlushPendingRegistrations()
    {
        foreach (var opt in pendingResponseOptions)
        {
            EventCenter.Instance.AddEventListener(
                E_EventType.Logic_PlayerActionExecuted, opt.GetNeverRespondHandler());
        }
        pendingResponseOptions.Clear();
    }

    private Dictionary<int, List<OptionBase>> fatherToSonsIndex;

    public bool UnLockOption(E_OptionType type,int index)
    {
        if (optionPool.ContainsKey(type) && optionPool[type][index] != null)
        {
            if (!optionPool[type][index].IsVisible)
            {
                optionPool[type][index].IsVisible=true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    #endregion
    
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
        {
            E_OptionType.Level5_Option, new OptionBase[]
            {
                new EntityEvent_5_01(),//应对——趁势(ノ￣ー￣)ノ
                new EntityEvent_5_02(),//应对——避开ヘ(￣ω￣ヘ)
                new EntityEvent_5_03(),//应对——动─=≡Σ((( つ•̀ω•́)つ
                new EntityEvent_5_04(),//应对——静[ ʘ _ ʘ ]
                new EntityEvent_5_05(),//交流
                new EntityEvent_5_06(),//观察·前
                new EntityEvent_5_07(),//向左
                new EntityEvent_5_08(),//观察·左
                new EntityEvent_5_09(),//向右
                new EntityEvent_5_10(),//观察·右
                new EntityEvent_5_11(),//回忆
                new EntityEvent_5_12(),//谁的愿望
                new EntityEvent_5_13(),//谁的身影
                new EntityEvent_5_14(),//画下的星星
                new EntityEvent_5_15(),//破碎地发光
                new EntityEvent_5_16(),//分析
                new EntityEvent_5_17(),//为什么他们的面容如此可憎呢？
                new EntityEvent_5_18(),//那个和我许下承诺的孩
                
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
        return optionPool.TryGetValue(type, out var pool) ? pool : null;
    }
    /// <summary>
    /// 按下按钮直接调用这个方法,然后前端参数直接往里面放就行。
    /// </summary>
    /// <param name="type"></param>
    /// <param name="index"></param>
    public void ExcuteOption(E_OptionType type, int index, OptionContext context = null)
    {
        FlushPendingRegistrations();

        if (!optionPool.TryGetValue(type, out var pool) || pool == null)
        { Debug.LogWarning($"ExcuteOption: 未注册选项池 {type}"); return; }
        if (index < 0 || index >= pool.Length)
        { Debug.LogWarning($"ExcuteOption: 索引越界 {type}[{index}]，长度 {pool.Length}");return; }

        pool[index].TriggerOption(context);

        if (pool[index].LastTriggerSuccess)
        {
            // 1. 自动解锁 father-son 链
            TryUnlockSons(type, pool[index].OptionID);

            // 2. 同时广播事件（给第三层复杂条件用）
            EventCenter.Instance.EventTrigger(
                E_EventType.Logic_OptionExecuted,
                new OptionExecutedArgs
                {
                    OptionType = type,
                    OptionID = pool[index].OptionID
                }
            );

            // 3. 原有的 SFX
            EventCenter.Instance.EventTrigger(E_EventType.Audio_Play_SFX,
                new object[] { E_SFX.LevelOptionExecution, false });
        
        }
    }
    
    private void TryUnlockSons(E_OptionType type, int fatherOptionID)
    {
        if (fatherToSonsIndex == null) return;
        if (!fatherToSonsIndex.TryGetValue(fatherOptionID, out var sons)) return;

        foreach (var son in sons)
        {
            if (son.OptionType == type && !son.IsVisible)
            {
                son.IsVisible = true;  // IsVisible setter 自动触发 pending 注册（如果是应对型）
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            }
        }
    }
    /// <summary>
    /// 重新注册选项池数据
    /// 每次调用都会清空旧数据并重新写入
    /// </summary>
    public void RegisterOptions(int level)
    {
        // 每次输入时先清空原本存的东西
        optionPool.Clear();

        fatherToSonsIndex = new Dictionary<int, List<OptionBase>>();
        
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
            case 4:
                optionPool.Add(E_OptionType.Level4_Option, Array.Empty<OptionBase>());
                break;
            case 5:
                optionPool.Add(E_OptionType.Level5_Option, new OptionBase[]
                {
                    new EntityEvent_5_01(), //应对——趁势(ノ￣ー￣)ノ
                    new EntityEvent_5_02(), //应对——避开ヘ(￣ω￣ヘ)
                    new EntityEvent_5_03(), //应对——动─=≡Σ((( つ•̀ω•́)つ
                    new EntityEvent_5_04(), //应对——静[ ʘ _ ʘ ]
                    new EntityEvent_5_05(), //交流
                    new EntityEvent_5_06(), //观察·前
                    new EntityEvent_5_07(), //向左
                    new EntityEvent_5_08(), //观察·左
                    new EntityEvent_5_09(), //向右
                    new EntityEvent_5_10(), //观察·右
                    new EntityEvent_5_11(), //回忆
                    new EntityEvent_5_12(), //谁的愿望
                    new EntityEvent_5_13(), //谁的身影
                    new EntityEvent_5_14(), //画下的星星
                    new EntityEvent_5_15(), //破碎地发光
                    new EntityEvent_5_16(), //分析
                    new EntityEvent_5_17(), //为什么他们的面容如此可憎呢？
                    new EntityEvent_5_18(), //那个和我许下承诺的孩


                });
                break;
        }
        foreach (var kv in optionPool)
        {
            foreach (var opt in kv.Value)
            {
                if (opt.fatherID != 0)
                {
                    if (!fatherToSonsIndex.ContainsKey(opt.fatherID))
                        fatherToSonsIndex[opt.fatherID] = new List<OptionBase>();
                    fatherToSonsIndex[opt.fatherID].Add(opt);
                }
            }
        }
    }
}
