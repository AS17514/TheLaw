using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunArt3 : OptionBase
{
    public override string OptionName { get; protected set; } = "“枪械”艺术装置3";
    public override int OptionID
    {
        get { return 5; }
        
    }
    public override string OptionDescription
    {
        get { return "选择一个时间骰子点数+2，选择自己一个行动骰子点数-1，并获得1个点数为1的行动骰子"; }
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
                new DiceCondition(E_DiceType.Action, 0, E_CompareType.Greater),
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
                        DiceManager.Instance.ModifyDieValue(dice, 2);
                    }
                    else
                    {
                        if (dice.type == E_DiceType.Action)
                        {
                            DiceManager.Instance.ModifyDieValue(dice, -1);
                            ActionDice tempDice=new ActionDice();
                            tempDice.value = 1;
                            DiceManager.Instance.AddDice(E_DiceType.Action, tempDice);
                        }
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
