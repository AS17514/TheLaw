using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adjust : OptionBase
{
    public override int OptionID
    {
        get { return 1; }
        
    }
    public override string OptionDescription
    {
        get { return "调整，选择一个时间般点数+1，令一个行动或思维骰的点数+1或-1(不可超出范围)"; }
    }

    public override bool IsVisible 
    {
        get { return true; }
    }

    public override E_OptionType OptionType
    {
        get{return E_OptionType.Player_InherentAction;}
    }
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.TimeAny,4,E_CompareType.Less)
            };
        }
    }
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
            if(DiceManager.Instance.changedDice==null||DiceManager.Instance.changedDice.Count != 1)
                result=false;
            if (result && optionContext is AdjustOptionContext adjustCtx)
            {
                DiceManager.Instance.ModifyDieValue(DiceManager.Instance.selectedDice[0], 1);
                DiceManager.Instance.ModifyDieValue(DiceManager.Instance.changedDice[0], adjustCtx.change);
            }
            else
            {
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
            
        }
    }
}
