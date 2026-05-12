using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity5 : Entity
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
            E_StateType_5.equipoise,
            new ActionNode[] {
                new ActionNode(E_IntentType.Entity5_Equipoise,equipoise_Action_Equipoise),
            },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity5_TheirWishes,equipoise_Desire_TheirWishes),
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_5.unbalance,
            new ActionNode[] {
                new ActionNode(E_IntentType.Entity5_Oscillation, unbalance_Action_Oscillation),
                new ActionNode(E_IntentType.Entity5_Assemble, unbalance_Action_Assemble),
            },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity5_TheirWishes,equipoise_Desire_TheirWishes),
                new DesireNode(E_DesireType.Entity5_Food,unbalance_Desire_Food),
                new DesireNode(E_DesireType.Entity5_Praise,unbalance_Desire_Praise),
                new DesireNode(E_DesireType.Entity5_Free,equipoise_Desire_Free),
                new DesireNode(E_DesireType.Entity5_Happiness,equipoise_Desire_Happiness),
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_5.throwupthem,
            new ActionNode[]
            {
                new ActionNode(E_IntentType.Entity5_WishesAreEndless,throwupthem_Action_WishesAreEndless),
            },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity5_WeDonNotNeedHimEitherJustLikeHer,throwupthem_Desire_WeDonNotNeedHimEitherJustLikeHer)
            }
        );

        StateManager.Instance.ChangeState(E_StateType_5.equipoise);
        #endregion

        // 最后进行数值初始化
        InitEntity(0, 999999);
    }

    #region 行动

    public int playerState1 = 1;
    public int playerState2 = 1;
    public void equipoise_Action_Equipoise()
    {
        if (ProgressManager.Instance.nowEntities[1].hp > ProgressManager.Instance.player.hp)
        {
            ProgressManager.Instance.player.RemoveLevel5Buff();
            playerState1 = 1;//上
        }
        else
        {
            playerState1 = 0;//下
        }

        if (buffs[E_BuffType.Desire] < 0)
        {
            playerState2 = 1;//左
        }
        else
        {
            playerState2 = 0;//右
        }
    }

    public void unbalance_Action_Oscillation()
    {
        if (ProgressManager.Instance.player.GetBuff(E_BuffType.Left) > 0)
        {
            EventManager.Instance.optionPool[E_OptionType.Level5_Option][0].IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            StartCoroutine(DelayAddListener_OscillationLeft());
        }
        else if (ProgressManager.Instance.player.GetBuff(E_BuffType.Right) > 0)
        {
            EventManager.Instance.optionPool[E_OptionType.Level5_Option][1].IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            StartCoroutine(DelayAddListener_OscillationRight());
        }
    }
    private IEnumerator DelayAddListener_OscillationLeft()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level5_Option][0] is EntityEvent_5_01 e1)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespond);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e1.NeverRespond);
        }
    }
    private IEnumerator DelayAddListener_OscillationRight()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level5_Option][1] is EntityEvent_5_02 e2)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e2.NeverRespond);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e2.NeverRespond);
        }
    }
    public void unbalance_Action_Assemble()
    {
        ProgressManager.Instance.nowEntities[1].hp = Math.Clamp(ProgressManager.Instance.nowEntities[1].hp + 3,
            ProgressManager.Instance.nowEntities[1].hp, ProgressManager.Instance.nowEntities[1].maxHp);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
        if (ProgressManager.Instance.player.GetBuff(E_BuffType.Left) > 0)
        {
            EventManager.Instance.optionPool[E_OptionType.Level5_Option][2].IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            StartCoroutine(DelayAddListener_AssembleLeft());
        }
        else if (ProgressManager.Instance.player.GetBuff(E_BuffType.Right) > 0)
        {
            EventManager.Instance.optionPool[E_OptionType.Level5_Option][3].IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            StartCoroutine(DelayAddListener_AssembleRight());
        }
    }
    private IEnumerator DelayAddListener_AssembleLeft()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level5_Option][2] is EntityEvent_5_03 e3)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e3.NeverRespond);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e3.NeverRespond);
        }
    }
    private IEnumerator DelayAddListener_AssembleRight()
    {
        yield return new WaitForEndOfFrame();
        if (EventManager.Instance.optionPool[E_OptionType.Level5_Option][3] is EntityEvent_5_04 e4)
        {
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, e4.NeverRespond);
            EventCenter.Instance.AddEventListener(E_EventType.Logic_PlayerActionExecuted, e4.NeverRespond);
        }
    }
    public void throwupthem_Action_WishesAreEndless()
    {
        ProgressManager.Instance.nowEntities[1].hp = Math.Clamp(ProgressManager.Instance.player.hp + 1,
            ProgressManager.Instance.nowEntities[1].hp, ProgressManager.Instance.nowEntities[1].maxHp);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
        unbalance_Action_Oscillation();
    }
    #endregion

    #region 愿望

    public void equipoise_Desire_TheirWishes()
    {
        ProgressManager.Instance.nowEntities[1].hp = Math.Clamp(ProgressManager.Instance.nowEntities[1].hp + 5,
            ProgressManager.Instance.nowEntities[1].hp, ProgressManager.Instance.nowEntities[1].maxHp);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
    }

    public void unbalance_Desire_Food()
    {
        ProgressManager.Instance.nowEntities[1].hp = Math.Clamp(ProgressManager.Instance.nowEntities[1].hp + 5,
            ProgressManager.Instance.nowEntities[1].hp, ProgressManager.Instance.nowEntities[1].maxHp);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
        ProgressManager.Instance.player.BeAttacked(Math.Abs(ProgressManager.Instance.player.hp - ProgressManager.Instance.nowEntities[1].hp));
    }

    public void unbalance_Desire_Praise()
    {
        //玩家所有行动和思维骰点数-1（不会小于1），时间进度-1
        ProgressManager.Instance.AddTimeProgress(-1);

        List<DiceBase> dicesToTransform = new List<DiceBase>();

        // 1. 处理行动骰子 (Action)
        foreach (var actionDice in DiceManager.Instance.dicePool[E_DiceType.Action])
        {
            if (actionDice.value == 1)
            {

            }
            else
            {
                actionDice.value -= 1;      // 点数减1
            }
        }

        // 2. 处理思维骰子 (Mind)
        foreach (var mindDice in DiceManager.Instance.dicePool[E_DiceType.Mind])
        {
            if (mindDice.value == 1)
            {

            }
            else
            {
                mindDice.value -= 1;      // 点数减1
            }
        }

        DiceManager.Instance.SortPoolByValue(E_DiceType.Action);
        DiceManager.Instance.SortPoolByValue(E_DiceType.Mind);
    }

    public bool IsPlayerFree = false;

    public void equipoise_Desire_Free()
    {
        ProgressManager.Instance.player.hp = Math.Clamp(ProgressManager.Instance.player.hp + 3,
            ProgressManager.Instance.player.hp, ProgressManager.Instance.player.maxHp);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerHP);
        IsPlayerFree = true;
    }

    public void equipoise_Desire_Happiness()
    {
        if (buffs[E_BuffType.Desire] > 0)
        {
            buffs[E_BuffType.Desire] += 3;
        }
        else
        {
            buffs[E_BuffType.Desire] -= 3;
        }
    }

    public void throwupthem_Desire_WeDonNotNeedHimEitherJustLikeHer()
    {
        ProgressManager.Instance.player.hp = 1;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerHP);
        EventManager.Instance.optionPool[E_OptionType.Level5_Option][16].IsVisible = true;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
    }
    #endregion
}
