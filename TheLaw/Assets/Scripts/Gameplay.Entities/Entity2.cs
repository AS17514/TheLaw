using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Entity2 : Entity
{
    #region 特殊效果
    //当双方受到伤害>=1的攻击时，会使“欲望”-1并免疫此次伤害
    public override void BeAttacked(int atk)
    {
        // if (GetBuff(E_BuffType.Desire) >= 0)
        // {
        //     AddBuff(E_BuffType.Desire, -1);
        // }
        // else
        //     base.BeAttacked(atk);
        if (atk >= 1 && GetBuff(E_BuffType.Desire) > 0)
        {
            AddBuff(E_BuffType.Desire, -1);
        
            // 欲望因攻击变为0，切入歇斯底里状态
            if (GetBuff(E_BuffType.Desire) == 0)
            {
                StateManager.Instance.ChangeState(E_StateType_2.hysterial);
            }
        }
        else
        {
            base.BeAttacked(atk);
        }
    }

    #endregion
    public override void InitEntity(int initialDesire = 0, int maxHp = 10)
    {
        AddBuff(E_BuffType.Desire,initialDesire);
        this.maxHp = maxHp;
        this.hp = maxHp;
    }
    
    public override void ManualInit()
    {
        base.ManualInit(); // 必须先调用父类，把自己注册进 BuffManager

        #region 状态管理器
        StateManager.Instance.ClearStates();

        StateManager.Instance.RegisterStateData(
            E_StateType_2.normal, 
            new ActionNode[]{ 
                new ActionNode(E_IntentType.Entity2_LightRain,normal_Action_LightRain),
                new ActionNode(E_IntentType.Entity2_ScorchingSun, normal_Action_ScorchingSun),
                new ActionNode(E_IntentType.Entity2_Gale,normal_Action_Gale)
            },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity2_HiddenBehindTheClothes,normal_Desire_HiddenBehindTheClothes),
                
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_2.ashamed, 
            new ActionNode[]{ 
                new ActionNode(E_IntentType.Entity2_Stress, ashamed_Action_Stress)
            },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity2_IfThatCountsAsMyClothesToo,ashamed_Desire_IfThatCountsAsMyClothesToo),
                new DesireNode(E_DesireType.Entity2_IfThoseCouldBeSofter,ashamed_Desire_IfThoseCouldBeSofter)
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_2.hysterial,
            new ActionNode[]
            {
                new ActionNode(E_IntentType.Entity2_HysterialStress,hysterial_Action_HysterialStress)
            }, 
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity2_PleaseTearThoseTornTattersApart,hysterial_Desire_PleaseTearThoseTornTattersApart)
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_2.composed,
            new ActionNode[]
            {
                new ActionNode(E_IntentType.Entity2_Rainstorm,hysterial_Action_Rainstorm)
            }, 
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity2_ThereAreNoMoreWishesLeft,composed_Desire_ThereAreNoMoreWishesLeft)
            }
        );

        StateManager.Instance.ChangeState(E_StateType_2.normal);
        #endregion

        // 最后进行数值初始化
        InitEntity(7, 6);
    }

    #region 行动

    public void normal_Action_LightRain()
    {
        // 准备一个列表，用来统一收集减到0需要被移除（变成破布）的骰子
        // 这样可以避免在 foreach 遍历过程中直接 Remove 导致的集合报错
        List<DiceBase> dicesToTransform = new List<DiceBase>();

        // 1. 处理行动骰子 (Action)
        foreach (var actionDice in DiceManager.Instance.dicePool[E_DiceType.Action])
        {
            if (actionDice.value == 1)
            {
                actionDice.isValid = true; // 标记为将要变成破布
                dicesToTransform.Add(actionDice);
            }
            else
            {
                actionDice.isValid = false; // 不转化为破布
                actionDice.value -= 1;      // 点数减1
            }
        }

        // 2. 处理思维骰子 (Mind)
        foreach (var mindDice in DiceManager.Instance.dicePool[E_DiceType.Mind])
        {
            if (mindDice.value == 1)
            {
                mindDice.isValid = true; // 标记为将要变成破布
                dicesToTransform.Add(mindDice);
            }
            else
            {
                mindDice.isValid = false; // 不转化为破布
                mindDice.value -= 1;      // 点数减1
            }
        }
        
        int tatterCount = 0;//玩家破布增加数
        // 3. 统一将变为0（isValid = true）的骰子从池子中移除，并给自己添加Buff
        foreach (var dice in dicesToTransform)
        {
            // 从对应的骰子池中移除它
            DiceManager.Instance.dicePool[dice.type].Remove(dice);
    
            //加玩家破布数
            tatterCount++;
        }
        
        //增加玩家破布
        ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters, tatterCount);
        
        // 4. 数据变动完毕后，重新排序并触发UI刷新
        DiceManager.Instance.SortPoolByValue(E_DiceType.Action);
        DiceManager.Instance.SortPoolByValue(E_DiceType.Mind);
    }

    public void normal_Action_ScorchingSun()
    {
        if (EventManager.Instance.optionPool[E_OptionType.Level2_Option][0] != null &&
            EventManager.Instance.optionPool[E_OptionType.Level2_Option][0] is EntityEvent_2_01 e1)
        {
            e1.IsVisible=true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            
            // 开启协程延迟注册，防止同一帧的事件派发导致直接触发
            StartCoroutine(DelayAddListener_ScorchingSun());
            
        }
    }
    private IEnumerator DelayAddListener_ScorchingSun()
    {
        // 等待当前帧结束，确保当前玩家引发的事件已经全部派发完毕
        yield return new WaitForEndOfFrame();

        if (EventManager.Instance.optionPool[E_OptionType.Level2_Option][0] is EntityEvent_2_01 e1)
        {
            // 防御性编程，先移除一下防止里面原本就有事件了
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespondToScorchingSun);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespondToScorchingSun);
        }
    }
    public void normal_Action_Gale()
    {
        ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters,3);
    }

    public void ashamed_Action_Stress()
    {
        if (EventManager.Instance.optionPool[E_OptionType.Level2_Option][0] != null &&
            EventManager.Instance.optionPool[E_OptionType.Level2_Option][0] is EntityEvent_2_01 e1)
        {
            e1.IsVisible=true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            
            // 开启协程延迟注册
            StartCoroutine(DelayAddListener_Stress());
            
        }
    }
    private IEnumerator DelayAddListener_Stress()
    {
        // 等待当前帧结束，确保当前玩家引发的事件已经全部派发完毕
        yield return new WaitForEndOfFrame();

        if (EventManager.Instance.optionPool[E_OptionType.Level2_Option][0] is EntityEvent_2_01 e1)
        {
            // 防御性编程，先移除一下防止里面原本就有事件了
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespondToStress);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespondToStress);
        }
    }
    public void hysterial_Action_HysterialStress()//无法应对
    {
        int atk=ProgressManager.Instance.player.GetBuff(E_BuffType.Tatters);
        int playerDesire=ProgressManager.Instance.player.GetBuff(E_BuffType.Desire);
        if (playerDesire > 0)
        {
            ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, -1);
        }
        else
        {
            ProgressManager.Instance.player.BeAttacked(atk);
        }
    }

    public void hysterial_Action_Rainstorm()
    {
        //玩家破布增加数
        int tatterCount = DiceManager.Instance.dicePool[E_DiceType.Action].Count +
                          DiceManager.Instance.dicePool[E_DiceType.Mind].Count;
        
        //增加玩家破布
        ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters, tatterCount);
        
        DiceManager.Instance.dicePool[E_DiceType.Action].Clear();
        DiceManager.Instance.dicePool[E_DiceType.Mind].Clear();
        
        // 数据变动完毕后，重新排序并触发UI刷新
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_ActionDice);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_MindDice);
    }
    #endregion

    #region 欲望

    public void normal_Desire_HiddenBehindTheClothes()
    {
       if(GetBuff(E_BuffType.Desire)>4)
            AddBuff(E_BuffType.Desire,7-GetBuff(E_BuffType.Desire));
       else
       {
          AddBuff(E_BuffType.Desire,3));
       }
       normal_Action_LightRain();//立即执行一次“行动”：“小雨”
       
       isHiddenBehindTheClothesUse=true;
       EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
       
    }

    public bool isHiddenBehindTheClothesUse=false;
    
    public void ashamed_Desire_IfThatCountsAsMyClothesToo()
    {
        AddBuff(E_BuffType.Desire,ProgressManager.Instance.player.GetBuff(E_BuffType.Tatters));
        normal_Action_LightRain();
        
        isIfThatCountsAsMyClothesTooUse=true;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
    }

    public bool isIfThatCountsAsMyClothesTooUse=false;
    
    public void ashamed_Desire_IfThoseCouldBeSofter()
    {
        ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters,4);
    }

    public void hysterial_Desire_PleaseTearThoseTornTattersApart()
    {
        int atk=ProgressManager.Instance.player.GetBuff(E_BuffType.Tatters);
        int playerDesire=ProgressManager.Instance.player.GetBuff(E_BuffType.Desire);
        if (playerDesire > 0)
        {
            ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, -1);
        }
        else
        {
            ProgressManager.Instance.player.BeAttacked(atk);
        }
    }

    public void composed_Desire_ThereAreNoMoreWishesLeft()
    {}
    #endregion
}
