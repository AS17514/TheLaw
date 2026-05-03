using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChantingLaw : OptionBase
{
    public override string OptionName { get; protected set; } = "念诵诗句时呼吸的节奏";
    public override int OptionID
    {
        get { return 4; }
        
    }
    public override string OptionDescription
    {
        get { return "选择一个时间般点数+l，选择一个行动或思维骰，将其转化成另一种骰子"; }
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
                new DiceCondition(E_DiceType.ActionOrMind, 0, E_CompareType.Any),
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
            if (result )
            {
                foreach (var dice in DiceManager.Instance.selectedDice)
                {
                    if (dice.type == E_DiceType.Time1 || dice.type == E_DiceType.Time2 ||
                        dice.type == E_DiceType.Time3 || dice.type == E_DiceType.Time4)
                    {
                        DiceManager.Instance.ModifyDieValue(dice, 1);
                    }
                    else
                    {
                        if (dice.type == E_DiceType.Action)
                            DiceManager.Instance.TransformDie(dice,E_DiceType.Mind);
                        else if (dice.type == E_DiceType.Mind)
                            DiceManager.Instance.TransformDie(dice,E_DiceType.Action);
                    }
                }
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
            
        }
    }
}
