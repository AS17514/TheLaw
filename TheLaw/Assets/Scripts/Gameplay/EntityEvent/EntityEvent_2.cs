using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EntityEvent_2
{
    Escape,
}

public class EntityEvent_2_01 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "逃避";

    public override string OptionDescription { get; protected set; } = "逃避";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;
    // public override bool IsVisible { get; set; } = true;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 3, E_CompareType.Less),
                new DiceCondition(E_DiceType.Action, 4, E_CompareType.Less),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Escape;

    #endregion

    public override void TriggerOption(OptionContext optionContext = null)
    {
        if (IsVisible)
        {
            bool result = IsSpecialConditionsHave
                ? EventManager.Instance.IsSpecialConditionsMet(specialConditions)
                : true;
            if (IsUseDiceCombo == true)
            {
                result = IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result = IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters,1);
                //应对完之后取消该选项的显示。
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
            
        }
        else
        {
            Debug.Log("为什么这个选项不可见，还会发来选项执行的请求。。。出自怪物二的玩家应对行为逃避。");
        }
    }

    public void NeverRespond(object info = null)
    {
        if (ProgressManager.Instance.player.GetBuff(E_BuffType.Desire) >0)
        {
            ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, -1);        
        }

        else if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
        {
            //造成对象“欲望”数的伤害
            ProgressManager.Instance.player.BeAttacked(entity2.GetBuff(E_BuffType.Desire));
        }
        //取消该选项的显示。
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
        
    }
}
