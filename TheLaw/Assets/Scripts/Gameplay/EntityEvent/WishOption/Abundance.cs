using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abundance : OptionBase
{
    public override string OptionName { get; protected set; } = "富足";
    public override int OptionID
    {
        get { return 7; }
        
    }
    public override string OptionDescription
    {
        get { return "随机投掷并获得3个行动或思维骰子,骰子的类型和数值都随机。"; }
    }

    public override bool IsVisible 
    {
        get
        {
            if(ProgressManager.Instance.level>1)
                return true;
            else
            {
                return false;
            }
        }
    }

    public override E_OptionType OptionType
    {
        get{return E_OptionType.Player_Wish;}
    }
    public override DiceCondition[] DiceCost
    {
        get
        {
            return null;
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
                DiceManager.Instance.GetRandomDice(E_DiceType.Action,E_DiceType.Mind);
                DiceManager.Instance.GetRandomDice(E_DiceType.Action,E_DiceType.Mind);
                DiceManager.Instance.GetRandomDice(E_DiceType.Action,E_DiceType.Mind);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToUnavailable);
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
