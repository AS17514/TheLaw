using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prepare : OptionBase
{
    public override int OptionID
    {
        get { return 0; }
        
    }

    public override string OptionDescription
    {
        get { return "准备，选择消耗一个时间骰子，选择行动或思维，投掷并获得一个对应的骰子；推进时间进度"; }
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
            DiceCost = new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Time1,1,E_CompareType.Any)
            };
            return DiceCost;
        }
    }

    public Action<E_DiceType,DiceBase> OtherExecuteLogic=DiceManager.Instance.AddDice;

    public override Action ExecuteLogic
    {
        get
        {
            ExecuteLogic += DiceManager.Instance.ConsumeValidSelectedDice;
            return ExecuteLogic;
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

            if (result)
            {
                OtherExecuteLogic?.Invoke((optionContext as PrepareOptionContext).diceType,null);
                ExecuteLogic?.Invoke();
            }
            else
            {
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
            
        }
    }
}
