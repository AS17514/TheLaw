using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Entity3 : Entity
{
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
            E_StateType_3.normal, 
            new ActionNode[]{ 
                new ActionNode(E_IntentType.Entity3_Marble,normal_Action_Marble),
            },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity3_TheWishToStopInTheMiddleOfRunning,normal_Desire_TheWishToStopInTheMiddleOfRunning),
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_3.weightless, 
            new ActionNode[]{ 
                new ActionNode(E_IntentType.Entity3_CosmicRoaming, weightless_Action_CosmicRoaming)
            },
            new DesireNode[]
            {
                //new DesireNode(E_DesireType.Entity2_IfThatCountsAsMyClothesToo,ashamed_Desire_IfThatCountsAsMyClothesToo),
                //new DesireNode(E_DesireType.Entity2_IfThoseCouldBeSofter,ashamed_Desire_IfThoseCouldBeSofter)
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_3.free_notfree,
            new ActionNode[]
            {
                //new ActionNode(E_IntentType.Entity2_HysterialStress,hysterial_Action_HysterialStress)
            }, 
            new DesireNode[]
            {
                //new DesireNode(E_DesireType.Entity2_PleaseTearThoseTornTattersApart,hysterial_Desire_PleaseTearThoseTornTattersApart)
            }
        );
        

        StateManager.Instance.ChangeState(E_StateType_3.normal);
        #endregion

        // 最后进行数值初始化
        InitEntity(0, 27);
    }

    #region 行动

    public void normal_Action_Marble()
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
                                                                 - ProgressManager.Instance.player.GetBuff(E_BuffType
                                                                     .Desire)));
            if(e3.GetBuff(E_BuffType.Desire)
                - ProgressManager.Instance.player.GetBuff(E_BuffType
                    .Desire)<0)//若此时对象的“欲望”更大则无法应对
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][0].IsVisible = true;
        }
        
        
    }

    public void weightless_Action_CosmicRoaming()
    {
        DiceManager.Instance.ClearEntityPool();
        DiceManager.Instance.AddEntityDice();
        if (DiceManager.Instance.entityDicePool != null && DiceManager.Instance.entityDicePool[0] != null)
        {
            if (DiceManager.Instance.entityDicePool[0].value == 5 || DiceManager.Instance.entityDicePool[0].value == 6)
            {
                DiceManager.Instance.entityDicePool[0].value = Random.Range(1, 5);//5不取
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice);
            }

            switch (DiceManager.Instance.entityDicePool[0].value)
            {
                case 1:
                    weightless_Action_Ball();
                    break;
                case 2:
                    weightless_Action_Curiousity();
                    break;
                case 3:
                    weightless_Action_Struggle();
                    break;
                case 4:
                    weightless_Action_Gravity();
                    break;
            }
        }
        
    }

    public void weightless_Action_Ball()
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
                                                                 - ProgressManager.Instance.player.GetBuff(E_BuffType
                                                                     .Desire)));
            EventManager.Instance.optionPool[E_OptionType.Level2_Option][0].IsVisible = true;
        }
    }
    public void weightless_Action_Curiousity(){}
    public void weightless_Action_Struggle(){}
    public void weightless_Action_Gravity(){}
    #endregion

    #region 欲望

    public void normal_Desire_TheWishToStopInTheMiddleOfRunning()
    {
        DiceManager.Instance.ClearEntityPool();
        DiceManager.Instance.AddEntityDice();
        if (DiceManager.Instance.entityDicePool != null && DiceManager.Instance.entityDicePool[0] != null)
        {
            switch (DiceManager.Instance.entityDicePool[0].value)
            {
                case 1:
                    normal_Desire_RegardingTheSharpStonesOnTheRiverbank();
                    break;
                case 2:
                    normal_Desire_RegardingTheWitheringFlowersOnTheRoadside();
                    break;
                case 3:
                    normal_Desire_RegardingOccasionalShuttleCars();
                    break;
                case 4:
                    normal_Desire_AGentleBreezeThatBlowsAwayFallenLeaves();
                    break;
                case 5:
                    normal_Desire_RumorsAboutDisinterest();
                    break;
                case 6:
                    normal_Desire_PromiseAboutUnrelatedWishes();
                    break;
            }
        }
    }

    #region TheWishToStopInTheMiddleOfRunning
    public void normal_Desire_RegardingTheSharpStonesOnTheRiverbank()
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            e3.AddBuff(E_BuffType.Desire, 1);
        ProgressManager.Instance.player.BeAttacked(1);
    }
    public void normal_Desire_RegardingTheWitheringFlowersOnTheRoadside()
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            e3.AddBuff(E_BuffType.Desire, 2);
        ProgressManager.Instance.AddTimeProgress(-2);
    }
    public void normal_Desire_RegardingOccasionalShuttleCars()
    {
        if (ProgressManager.Instance.nowEntities[0] is Part3_1 e1)
        {
            if (e1.isDestroyed)//若车轮已被破坏，恢复3点生命，取消车轮的破坏
            {
                e1.hp = 3;
                ProgressManager.Instance.nowEntities[0].hp =Math.Clamp(hp+3,hp,ProgressManager.Instance.nowEntities[0].maxHp) ;
                e1.isDestroyed=false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP);
            }
            if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)//若车轮未被破坏，对象“欲望”+3
                e3.AddBuff(E_BuffType.Desire, 3);
        }
    }
    
    public void normal_Desire_AGentleBreezeThatBlowsAwayFallenLeaves()
    {
        // 处理实体的 Buff 逻辑
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            e3.AddBuff(E_BuffType.Desire, 4);
        }

        // 获取行动和思维骰子池的引用
        var actionPool = DiceManager.Instance.dicePool[E_DiceType.Action];
        var mindPool = DiceManager.Instance.dicePool[E_DiceType.Mind];

        int totalDiceCount = actionPool.Count + mindPool.Count;

        // 如果骰子总数不足 5 个，直接将所有行动和思维骰子重投
        if (totalDiceCount < 5)
        {
            foreach (var dice in actionPool)
            {
                dice.Roll();
            }
            foreach (var dice in mindPool)
            {
                dice.Roll();
            }
        }
        else
        {
            // 构建临时“大箱子”：将所有符合条件的骰子引用放入一个临时列表中
            List<DiceBase> eligibleDice = new List<DiceBase>();
            eligibleDice.AddRange(actionPool);
            eligibleDice.AddRange(mindPool);

            // 随机抽取 4 个不重复的骰子进行重投
            for (int i = 0; i < 4; i++)
            {
                // 在当前剩余的骰子中随机选一个
                int randomIndex = Random.Range(0, eligibleDice.Count);
            
                // 执行重投
                eligibleDice[randomIndex].Roll();
            
                // 【关键】将该骰子从临时列表中移除，确保下次循环绝对不会抽到同一个骰子
                eligibleDice.RemoveAt(randomIndex);
            }
        }

        // 统一重新排序并触发 UI 刷新
        DiceManager.Instance.SortPoolByValue(E_DiceType.Action);
        DiceManager.Instance.SortPoolByValue(E_DiceType.Mind);
        }

    public void normal_Desire_RumorsAboutDisinterest()
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            e3.AddBuff(E_BuffType.Desire, 5);
        }
        IsRumorsAboutDisinterestUse=true;
    }
    public bool IsRumorsAboutDisinterestUse=false;

    public void normal_Desire_PromiseAboutUnrelatedWishes()
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            e3.AddBuff(E_BuffType.Desire, 6);
            if(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)-ProgressManager.Instance.player.GetBuff(E_BuffType.Desire))>=6)
            {
                //若此次许愿后对象与玩家的“欲望”差值>=6，获得胜利
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
            }
        }
    }
    

    #endregion
    
    #endregion
}
