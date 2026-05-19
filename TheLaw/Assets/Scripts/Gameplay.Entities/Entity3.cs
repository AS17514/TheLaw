using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Entity3 : Entity
{
    public override void InitEntity(int initialDesire = 0, int maxHp = 10)
    {
        AddBuff(E_BuffType.Desire, initialDesire);
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
            new ActionNode[] {
                new ActionNode(E_IntentType.Entity3_Marble,normal_Action_Marble),
            },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity3_TheWishToStopInTheMiddleOfRunning,normal_Desire_TheWishToStopInTheMiddleOfRunning),
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_3.weightless,
            new ActionNode[] {
                new ActionNode(E_IntentType.Entity3_CosmicRoaming, weightless_Action_CosmicRoaming)
            },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity3_IWantToLeaveLikeYou,weightless_Desire_IWantToLeaveLikeYou),
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_3.free_notfree,
            new ActionNode[]
            {
                new ActionNode(E_IntentType.Entity3_AnEmptyPlanet,free_notfree_Action_AnEmptyPlanet),
                new ActionNode(E_IntentType.Entity3_Curiousity,weightless_Action_Curiousity),
            },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity3_RunAwayHopingThatYourRunningWonNotBeGivenThatNameAgain,free_notfree_Desire_RunAwayHopingThatYourRunningWonNotBeGivenThatNameAgain)
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
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            // 若此时对象的“欲望”更大则无法应对
            if (e3.GetBuff(E_BuffType.Desire) - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire) < 0)
            {
                if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][0] != null &&
                    EventManager.Instance.optionPool[E_OptionType.Level3_Option][0] is EntityEvent_3_01 e1)
                {
                    e1.IsVisible = true;
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                    StartCoroutine(DelayAddListener_Marble());
                }
            }
            else
            {
                ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
                                                                     - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)));
            }
        }
    }

    private IEnumerator DelayAddListener_Marble()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][0] is EntityEvent_3_01 e1)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespondToMarble);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespondToMarble);
        }
    }

    public void weightless_Action_CosmicRoaming()
    {
        DiceManager.Instance.ClearEntityPool();
        EntityDice dice = DiceManager.Instance.AddEntityDice();

        if (dice.value == 5 || dice.value == 6)
        {
            dice.value = Random.Range(1, 5); // 5不取
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice);
        }

        switch (dice.value)
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

    #region CosmicRoaming

    public void weightless_Action_Ball()
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
                                                                 - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)));
            
            if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][0] != null &&
                EventManager.Instance.optionPool[E_OptionType.Level3_Option][0] is EntityEvent_3_01 e1)
            {
                e1.IsVisible = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                StartCoroutine(DelayAddListener_Ball());
            }
        }
    }

    private IEnumerator DelayAddListener_Ball()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][0] is EntityEvent_3_01 e1)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespondToBall);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespondToBall);
        }
    }

    public void weightless_Action_Curiousity()
    {
        EntityDice dice1 = DiceManager.Instance.AddEntityDice();
        EntityDice dice2 = DiceManager.Instance.AddEntityDice();
        dice1.value = Random.Range(1, 5); // 5不取;
        dice2.value = Random.Range(1, 5); // 5不取;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice);
        switch (dice1.value)
        {
            case 1:
                normal_Desire_RegardingTheSharpStonesOnTheRiverbank();
                if (ProgressManager.Instance.nowEntities[0] is Entity3 e31)
                    e31.AddBuff(E_BuffType.Desire, -1);
                break;
            case 2:
                normal_Desire_RegardingTheWitheringFlowersOnTheRoadside();
                if (ProgressManager.Instance.nowEntities[0] is Entity3 e32)
                    e32.AddBuff(E_BuffType.Desire, -2);
                break;
            case 3:
                normal_Desire_RegardingOccasionalShuttleCars();
                if (ProgressManager.Instance.nowEntities[0] is Entity3 e33)
                    e33.AddBuff(E_BuffType.Desire, -3);
                break;
            case 4:
                normal_Desire_AGentleBreezeThatBlowsAwayFallenLeaves();
                if (ProgressManager.Instance.nowEntities[0] is Entity3 e34)
                    e34.AddBuff(E_BuffType.Desire, -4);
                break;
        }
        switch (dice2.value)
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
        }
        EventManager.Instance.optionPool[E_OptionType.Level3_Option][13].IsVisible = true;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
    }
    
    public void weightless_Action_Struggle()
    {
        if (ProgressManager.Instance.nowEntities[4] is Part3_4 p4)
        {
            if (!p4.isDestroyed)
            {
                p4.BeAttacked(3);
            }
        }
    }

    public void weightless_Action_Gravity()
    {
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][1] != null &&
            EventManager.Instance.optionPool[E_OptionType.Level3_Option][1] is EntityEvent_3_02 e2)
        {
            e2.IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            StartCoroutine(DelayAddListener_Gravity());
        }
    }

    private IEnumerator DelayAddListener_Gravity()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][1] is EntityEvent_3_02 e2)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e2.NeverRespondToGravity);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e2.NeverRespondToGravity);
        }
    }

    #endregion

    public void free_notfree_Action_AnEmptyPlanet()
    {
        DiceManager.Instance.ClearEntityPool();
        EntityDice dice = DiceManager.Instance.AddEntityDice();
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            e3.AddBuff(E_BuffType.Desire, dice.value);
            if (e3.GetBuff(E_BuffType.Desire) > ProgressManager.Instance.player.GetBuff(E_BuffType.Desire))
            {
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerDied);
            }
        }
    }

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
                    ForOption_IsRumorsAboutDisinterestUse=true;
                    break;
                case 6:
                    normal_Desire_PromiseAboutUnrelatedWishes();
                    break;
            }
        }
    }
    public bool ForOption_IsRumorsAboutDisinterestUse=false;
    
    #region TheWishToStopInTheMiddleOfRunning
    public void normal_Desire_RegardingTheSharpStonesOnTheRiverbank()
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            e3.AddBuff(E_BuffType.Desire, 1);
        ProgressManager.Instance.player.BeAttacked(1);
    }
    
    public void normal_Desire_RegardingTheWitheringFlowersOnTheRoadside()
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            e3.AddBuff(E_BuffType.Desire, 2);
        ProgressManager.Instance.AddTimeProgress(-2);
    }
    
    public void normal_Desire_RegardingOccasionalShuttleCars()
    {
        if (ProgressManager.Instance.nowEntities[1] is Part3_1 e1)
        {
            if (e1.isDestroyed) // 若车轮已被破坏，恢复3点生命，取消车轮的破坏
            {
                e1.hp = 3;
                ProgressManager.Instance.nowEntities[0].hp = Math.Clamp(hp + 3, hp, ProgressManager.Instance.nowEntities[0].maxHp);
                e1.isDestroyed = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP);
            }
            else if (ProgressManager.Instance.nowEntities[0] is Entity3 e3) // 若车轮未被破坏，对象“欲望”+3
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
        IsRumorsAboutDisinterestUse = true;
    }
    public bool IsRumorsAboutDisinterestUse = false;

    public void normal_Desire_PromiseAboutUnrelatedWishes()
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            e3.AddBuff(E_BuffType.Desire, 6);
            if (Mathf.Abs(e3.GetBuff(E_BuffType.Desire) - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)) > 6)
            {
                // 若此次许愿后对象与玩家的“欲望”差值>6，获得胜利
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
            }
        }
    }

    #endregion

    public void weightless_Desire_IWantToLeaveLikeYou()
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
        {
            if (Math.Abs(e3.GetBuff(E_BuffType.Desire) - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)) >= 4)
            {
                DiceManager.Instance.ClearEntityPool();
                EntityDice dice = DiceManager.Instance.AddEntityDice();
                dice.value = Random.Range(1, 5);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice);
                switch (dice.value)
                {
                    case 1:
                        weightless_Desire_FurBall1();
                        break;
                    case 2:
                        weightless_Desire_FurBall2();
                        break;
                    case 3:
                        weightless_Desire_FurBall3();
                        break;
                    case 4:
                        weightless_Desire_FurBall4();
                        break;
                }
            }
            else
            {
                weightless_Action_Struggle();
            }
        }
    }

    #region IWantToLeaveLikeYou

    public void weightless_Desire_FurBall1()
    {
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][2] != null &&
            EventManager.Instance.optionPool[E_OptionType.Level3_Option][2] is EntityEvent_3_03 e3)
        {
            e3.IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            StartCoroutine(DelayAddListener_FurBall1());
        }
    }

    private IEnumerator DelayAddListener_FurBall1()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][2] is EntityEvent_3_03 e3)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e3.NeverRespondToFurBall1);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e3.NeverRespondToFurBall1);
        }
    }

    public void weightless_Desire_FurBall2()
    {
        weightless_Desire_FurBall1();
        
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][3] != null &&
            EventManager.Instance.optionPool[E_OptionType.Level3_Option][3] is EntityEvent_3_04 e4)
        {
            e4.IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            StartCoroutine(DelayAddListener_FurBall2());
        }
    }

    private IEnumerator DelayAddListener_FurBall2()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][3] is EntityEvent_3_04 e4)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e4.NeverRespondToFurBall2);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e4.NeverRespondToFurBall2);
        }
    }

    public void weightless_Desire_FurBall3()
    {
        weightless_Desire_FurBall2();
        
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][4] != null &&
            EventManager.Instance.optionPool[E_OptionType.Level3_Option][4] is EntityEvent_3_05 e5)
        {
            e5.IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            StartCoroutine(DelayAddListener_FurBall3());
        }
    }

    private IEnumerator DelayAddListener_FurBall3()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][4] is EntityEvent_3_05 e5)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e5.NeverRespondToFurBall3);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e5.NeverRespondToFurBall3);
        }
    }

    public void weightless_Desire_FurBall4()
    {
        weightless_Desire_FurBall3();
        
        // 注意：原代码这里是 [4]，可能是笔误，帮你顺延改为了 [5]
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][5] != null &&
            EventManager.Instance.optionPool[E_OptionType.Level3_Option][5] is EntityEvent_3_06 e6)
        {
            e6.IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            StartCoroutine(DelayAddListener_FurBall4());
        }
    }

    private IEnumerator DelayAddListener_FurBall4()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][5] is EntityEvent_3_06 e6)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e6.NeverRespondToFurBall4);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e6.NeverRespondToFurBall4);
        }
    }

    #endregion

    public void free_notfree_Desire_RunAwayHopingThatYourRunningWonNotBeGivenThatNameAgain()
    {
        if (ProgressManager.Instance.nowEntities[1] is Part3_1 p1)
        {
            if (p1.isDestroyed) // 若车轮已被破坏，恢复3点生命，取消车轮的破坏
            {
                p1.hp = 3;
                ProgressManager.Instance.nowEntities[0].hp = Math.Clamp(hp + 6, hp, ProgressManager.Instance.nowEntities[0].maxHp);
                p1.isDestroyed = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP);
            }
            else if (ProgressManager.Instance.nowEntities[0] is Entity3 e3) // 若车轮未被破坏，对象“欲望”+3
                e3.AddBuff(E_BuffType.Desire, 6);
        }

        ForOption_IsRunAwayHopingThatYourRunningWonNotBeGivenThatNameAgainUse = true;
    }
    
    public bool ForOption_IsRunAwayHopingThatYourRunningWonNotBeGivenThatNameAgainUse=false;
    #endregion
}
// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// public class Entity3 : Entity
// {
// public override void InitEntity(int initialDesire = 0, int maxHp = 10)
//     {
//         AddBuff(E_BuffType.Desire,initialDesire);
//         this.maxHp = maxHp;
//         this.hp = maxHp;
//     }
//     
//     public override void ManualInit()
//     {
//         base.ManualInit(); // 必须先调用父类，把自己注册进 BuffManager
//
//         #region 状态管理器
//         StateManager.Instance.ClearStates();
//
//         StateManager.Instance.RegisterStateData(
//             E_StateType_3.normal, 
//             new ActionNode[]{ 
//                 new ActionNode(E_IntentType.Entity3_Marble,normal_Action_Marble),
//             },
//             new DesireNode[]
//             {
//                 new DesireNode(E_DesireType.Entity3_TheWishToStopInTheMiddleOfRunning,normal_Desire_TheWishToStopInTheMiddleOfRunning),
//             }
//         );
//         StateManager.Instance.RegisterStateData(
//             E_StateType_3.weightless, 
//             new ActionNode[]{ 
//                 new ActionNode(E_IntentType.Entity3_CosmicRoaming, weightless_Action_CosmicRoaming)
//             },
//             new DesireNode[]
//             {
//                 new DesireNode(E_DesireType.Entity3_IWantToLeaveLikeYou,weightless_Desire_IWantToLeaveLikeYou),
//             }
//         );
//         StateManager.Instance.RegisterStateData(
//             E_StateType_3.free_notfree,
//             new ActionNode[]
//             {
//                 new ActionNode(E_IntentType.Entity3_AnEmptyPlanet,free_notfree_Action_AnEmptyPlanet),
//                 new ActionNode(E_IntentType.Entity3_Curiousity,weightless_Action_Curiousity),
//             }, 
//             new DesireNode[]
//             {
//                 new DesireNode(E_DesireType.Entity3_RunAwayHopingThatYourRunningWonNotBeGivenThatNameAgain,free_notfree_Desire_RunAwayHopingThatYourRunningWonNotBeGivenThatNameAgain)
//             }
//         );
//         
//
//         StateManager.Instance.ChangeState(E_StateType_3.normal);
//         #endregion
//
//         // 最后进行数值初始化
//         InitEntity(0, 27);
//     }
//
//     #region 行动
//
//     public void normal_Action_Marble()
//     {
//         if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
//         {
//             ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
//                                                                  - ProgressManager.Instance.player.GetBuff(E_BuffType
//                                                                      .Desire)));
//             if(e3.GetBuff(E_BuffType.Desire)
//                 - ProgressManager.Instance.player.GetBuff(E_BuffType
//                     .Desire)<0)//若此时对象的“欲望”更大则无法应对
//                 EventManager.Instance.optionPool[E_OptionType.Level2_Option][0].IsVisible = true;
//         }
//         
//         
//     }
//
//     public void weightless_Action_CosmicRoaming()
//     {
//         DiceManager.Instance.ClearEntityPool();
//         EntityDice dice= DiceManager.Instance.AddEntityDice();
//         
//         if (dice.value == 5 || dice.value == 6)
//         {
//             dice.value = Random.Range(1, 5);//5不取
//             EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice);
//         }
//
//         switch (dice.value)
//         {
//             case 1:
//                 weightless_Action_Ball();
//                 break;
//             case 2:
//                 weightless_Action_Curiousity();
//                 break;
//             case 3:
//                 weightless_Action_Struggle();
//                 break;
//             case 4:
//                 weightless_Action_Gravity();
//                 break;
//         }
//     }
//
//     #region CosmicRoaming
//
//     public void weightless_Action_Ball()
//     {
//         if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
//         {
//             ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
//                                                                  - ProgressManager.Instance.player.GetBuff(E_BuffType
//                                                                      .Desire)));
//             EventManager.Instance.optionPool[E_OptionType.Level2_Option][0].IsVisible = true;
//         }
//     }
//
//     public void weightless_Action_Curiousity()
//     {
//         EntityDice dice1=DiceManager.Instance.AddEntityDice(); 
//         EntityDice dice2=DiceManager.Instance.AddEntityDice(); 
//         dice1.value = Random.Range(1, 5);//5不取;
//         dice2.value = Random.Range(1, 5);//5不取;
//         EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice);
//         switch (dice1.value)
//         {
//             case 1:
//                 normal_Desire_RegardingTheSharpStonesOnTheRiverbank();
//                 if(ProgressManager.Instance.nowEntities[0] is Entity3 e31)
//                     e31.AddBuff(E_BuffType.Desire, -1);
//                 break;
//             case 2:
//                 normal_Desire_RegardingTheWitheringFlowersOnTheRoadside();
//                 if(ProgressManager.Instance.nowEntities[0] is Entity3 e32)
//                     e32.AddBuff(E_BuffType.Desire, -2);
//                 break;
//             case 3:
//                 normal_Desire_RegardingOccasionalShuttleCars();
//                 if(ProgressManager.Instance.nowEntities[0] is Entity3 e33)
//                     e33.AddBuff(E_BuffType.Desire, -3);
//                 break;
//             case 4:
//                 normal_Desire_AGentleBreezeThatBlowsAwayFallenLeaves();
//                 if(ProgressManager.Instance.nowEntities[0] is Entity3 e34)
//                     e34.AddBuff(E_BuffType.Desire, -4);
//                 break;
//         }
//         switch (dice2.value)
//         {
//             case 1:
//                 normal_Desire_RegardingTheSharpStonesOnTheRiverbank();
//                 break;
//             case 2:
//                 normal_Desire_RegardingTheWitheringFlowersOnTheRoadside();
//                 break;
//             case 3:
//                 normal_Desire_RegardingOccasionalShuttleCars();
//                 break;
//             case 4:
//                 normal_Desire_AGentleBreezeThatBlowsAwayFallenLeaves();
//                 break;
//         }
//     }
//
//     public void weightless_Action_Struggle()
//     {
//         if (ProgressManager.Instance.nowEntities[54
//             if (!p4.isDestroyed)
//             {
//                 p4.BeAttacked(3);
//             }
//         }
//     }
//
//     public void weightless_Action_Gravity()
//     {
//         //DiceManager.Instance.SetAllTimeDiceToFour();
//         EventManager.Instance.optionPool[E_OptionType.Level3_Option][1].IsVisible = true;
//         EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
//     }
//
//     #endregion
//
//     public void free_notfree_Action_AnEmptyPlanet()
//     {
//         DiceManager.Instance.ClearEntityPool();
//         EntityDice dice= DiceManager.Instance.AddEntityDice();
//         if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
//         {
//             e3.AddBuff(E_BuffType.Desire, dice.value);
//             if (e3.GetBuff(E_BuffType.Desire) > ProgressManager.Instance.player.GetBuff(E_BuffType.Desire))
//             {
//                 EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerDied);
//             }
//         }
//         
//     }
//     
//     #endregion
//
//     #region 欲望
//
//     public void normal_Desire_TheWishToStopInTheMiddleOfRunning()
//     {
//         DiceManager.Instance.ClearEntityPool();
//         DiceManager.Instance.AddEntityDice();
//         if (DiceManager.Instance.entityDicePool != null && DiceManager.Instance.entityDicePool[0] != null)
//         {
//             switch (DiceManager.Instance.entityDicePool[0].value)
//             {
//                 case 1:
//                     normal_Desire_RegardingTheSharpStonesOnTheRiverbank();
//                     break;
//                 case 2:
//                     normal_Desire_RegardingTheWitheringFlowersOnTheRoadside();
//                     break;
//                 case 3:
//                     normal_Desire_RegardingOccasionalShuttleCars();
//                     break;
//                 case 4:
//                     normal_Desire_AGentleBreezeThatBlowsAwayFallenLeaves();
//                     break;
//                 case 5:
//                     normal_Desire_RumorsAboutDisinterest();
//                     break;
//                 case 6:
//                     normal_Desire_PromiseAboutUnrelatedWishes();
//                     break;
//             }
//         }
//     }
//
//     #region TheWishToStopInTheMiddleOfRunning
//     public void normal_Desire_RegardingTheSharpStonesOnTheRiverbank()
//     {
//         if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
//             e3.AddBuff(E_BuffType.Desire, 1);
//         ProgressManager.Instance.player.BeAttacked(1);
//     }
//     public void normal_Desire_RegardingTheWitheringFlowersOnTheRoadside()
//     {
//         if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)
//             e3.AddBuff(E_BuffType.Desire, 2);
//         ProgressManager.Instance.AddTimeProgress(-2);
//     }
//     public void normal_Desire_RegardingOccasionalShuttleCars()
//     {
//         if (ProgressManager.Instance.nowEntities[1] is Part3_1 e1)
//         {
//             if (e1.isDestroyed)//若车轮已被破坏，恢复3点生命，取消车轮的破坏
//             {
//                 e1.hp = 3;
//                 ProgressManager.Instance.nowEntities[0].hp =Math.Clamp(hp+3,hp,ProgressManager.Instance.nowEntities[0].maxHp) ;
//                 e1.isDestroyed=false;
//                 EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
//                 EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP);
//             }
//             else if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)//若车轮未被破坏，对象“欲望”+3
//                 e3.AddBuff(E_BuffType.Desire, 3);
//         }
//     }
//     
//     public void normal_Desire_AGentleBreezeThatBlowsAwayFallenLeaves()
//     {
//         // 处理实体的 Buff 逻辑
//         if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
//         {
//             e3.AddBuff(E_BuffType.Desire, 4);
//         }
//
//         // 获取行动和思维骰子池的引用
//         var actionPool = DiceManager.Instance.dicePool[E_DiceType.Action];
//         var mindPool = DiceManager.Instance.dicePool[E_DiceType.Mind];
//
//         int totalDiceCount = actionPool.Count + mindPool.Count;
//
//         // 如果骰子总数不足 5 个，直接将所有行动和思维骰子重投
//         if (totalDiceCount < 5)
//         {
//             foreach (var dice in actionPool)
//             {
//                 dice.Roll();
//             }
//             foreach (var dice in mindPool)
//             {
//                 dice.Roll();
//             }
//         }
//         else
//         {
//             // 构建临时“大箱子”：将所有符合条件的骰子引用放入一个临时列表中
//             List<DiceBase> eligibleDice = new List<DiceBase>();
//             eligibleDice.AddRange(actionPool);
//             eligibleDice.AddRange(mindPool);
//
//             // 随机抽取 4 个不重复的骰子进行重投
//             for (int i = 0; i < 4; i++)
//             {
//                 // 在当前剩余的骰子中随机选一个
//                 int randomIndex = Random.Range(0, eligibleDice.Count);
//             
//                 // 执行重投
//                 eligibleDice[randomIndex].Roll();
//             
//                 // 【关键】将该骰子从临时列表中移除，确保下次循环绝对不会抽到同一个骰子
//                 eligibleDice.RemoveAt(randomIndex);
//             }
//         }
//
//         // 统一重新排序并触发 UI 刷新
//         DiceManager.Instance.SortPoolByValue(E_DiceType.Action);
//         DiceManager.Instance.SortPoolByValue(E_DiceType.Mind);
//         }
//
//     public void normal_Desire_RumorsAboutDisinterest()
//     {
//         if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
//         {
//             e3.AddBuff(E_BuffType.Desire, 5);
//         }
//         IsRumorsAboutDisinterestUse=true;
//     }
//     public bool IsRumorsAboutDisinterestUse=false;
//
//     public void normal_Desire_PromiseAboutUnrelatedWishes()
//     {
//         if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
//         {
//             e3.AddBuff(E_BuffType.Desire, 6);
//             if(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)-ProgressManager.Instance.player.GetBuff(E_BuffType.Desire))>=6)
//             {
//                 //若此次许愿后对象与玩家的“欲望”差值>=6，获得胜利
//                 EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
//             }
//         }
//     }
//     
//
//     #endregion
//
//     public void weightless_Desire_IWantToLeaveLikeYou()
//     {
//         if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
//         {
//             if (Math.Abs(e3.GetBuff(E_BuffType.Desire) - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)) >=
//                 4)
//             {
//                 DiceManager.Instance.ClearEntityPool();
//                 EntityDice dice= DiceManager.Instance.AddEntityDice();
//                 dice.value = Random.Range(1, 5);
//                 EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice);
//                 switch (dice.value)
//                 {
//                     case 1:
//                         weightless_Desire_FurBall1();
//                         break;
//                     case 2:
//                         weightless_Desire_FurBall2();
//                         break;
//                     case 3:
//                         weightless_Desire_FurBall3();
//                         break;
//                     case 4:
//                         weightless_Desire_FurBall4();
//                         break;
//                 }
//             }
//             else
//             {
//                 weightless_Action_Struggle();
//             }
//         }
//     }
//     
//     #region IWantToLeaveLikeYou
//
//     public void weightless_Desire_FurBall1()
//     {
//         EventManager.Instance.optionPool[E_OptionType.Level3_Option][2].IsVisible=true;
//     }
//     public void weightless_Desire_FurBall2()
//     {
//         weightless_Desire_FurBall1();
//         EventManager.Instance.optionPool[E_OptionType.Level3_Option][3].IsVisible=true;
//     }
//     public void weightless_Desire_FurBall3()
//     {
//         weightless_Desire_FurBall2();
//         EventManager.Instance.optionPool[E_OptionType.Level3_Option][4].IsVisible=true;
//     }
//     public void weightless_Desire_FurBall4()
//     {
//         weightless_Desire_FurBall3();
//         EventManager.Instance.optionPool[E_OptionType.Level3_Option][4].IsVisible=true;
//     }
//
//     #endregion
//
//
//     public void free_notfree_Desire_RunAwayHopingThatYourRunningWonNotBeGivenThatNameAgain()
//     {
//         if (ProgressManager.Instance.nowEntities[1] is Part3_1 p1)
//         {
//             if (p1.isDestroyed)
//             {
//                 if (p1.isDestroyed)//若车轮已被破坏，恢复3点生命，取消车轮的破坏
//                 {
//                     p1.hp = 3;
//                     ProgressManager.Instance.nowEntities[0].hp =Math.Clamp(hp+6,hp,ProgressManager.Instance.nowEntities[0].maxHp) ;
//                     p1.isDestroyed=false;
//                     EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
//                     EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP);
//                 }
//                 else if(ProgressManager.Instance.nowEntities[0] is Entity3 e3)//若车轮未被破坏，对象“欲望”+3
//                     e3.AddBuff(E_BuffType.Desire, 6);
//             }
//         }
//     }
//     #endregion
// }
