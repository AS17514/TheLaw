using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum E_EntityEvent_4
{
    contentment,//知足
}

public class EntityEvent_4_01 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "知足";
    public override string OptionDescription { get; protected set; } = "";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 2, E_CompareType.Equal),
                new DiceCondition(E_DiceType.Mind, 4, E_CompareType.Equal),
            };
        }
    }

    public override UnityAction<object> GetNeverRespondHandler() => NeverRespond;
    
    
    #endregion

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.contentment;


    #endregion

    public EntityEvent_4_01()
    {
        ExecuteLogic = ExecuteLogicImpl;
    }

    private void ExecuteLogicImpl()
    {
        EventCenter.Instance.RemoveEventListener(
            E_EventType.Logic_PlayerActionExecuted, NeverRespond);
        IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }
    
    public void NeverRespond(object info = null)
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity entity)
        {
            int length = 0;
            length += entity.GetBuff(E_BuffType.Want0) > 0 ? 0 : 1;
            length += entity.GetBuff(E_BuffType.Want1) > 0 ? 0 : 1;
            length += entity.GetBuff(E_BuffType.Want2) > 0 ? 0 : 1;
            length += entity.GetBuff(E_BuffType.Want3) > 0 ? 0 : 1;
            ProgressManager.Instance.player.BeAttacked(length);
        }
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
    }
}
