using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatteredStars : OptionBase
{
    public override string OptionName { get; protected set; } = "画下的星星将破碎地放光";
    public override int OptionID
    {
        get { return 6; }
        
    }
    public override string OptionDescription
    {
        get { return "选择一个时间骰点数+3，获得一个百搭骰子"; }
    }

    public override bool IsVisible 
    {
        get { return true; }
    }

    public override E_OptionType OptionType
    {
        get{return E_OptionType.Player_Law;}
    }
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.TimeAny,4,E_CompareType.Less),
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

            if (result)
            {
                foreach (var dice in DiceManager.Instance.selectedDice)
                {
                    if (dice.type == E_DiceType.Time1 || dice.type == E_DiceType.Time2 ||
                        dice.type == E_DiceType.Time3 || dice.type == E_DiceType.Time4)
                    {
                        DiceManager.Instance.ModifyDieValue(dice, 3);
                        DiceManager.Instance.AddDice(E_DiceType.Wild);
                    }
                }
                DiceManager.Instance.ClearSelected();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
