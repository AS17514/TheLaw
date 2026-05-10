namespace Gameplay.EntityEvent.WishOption
{
    public class SmoothAndSteady:OptionBase
    {
        public override string OptionName { get; protected set; } = "安稳";

        public override int OptionID
        {
            get { return 10; }
        
        }
        public override string OptionDescription
        {
            get { return "选择一个行动或思维骰，回复其点数-1的生命，并将其的点数变为1"; }
        }

        public override bool IsVisible 
        {
            get
            {
                if(ProgressManager.Instance.level>4)
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
                return new DiceCondition[]
                {
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

                if (result)
                {
                    //DiceManager.Instance.selectedDice
                    //DiceManager.Instance.ModifyDieValue();
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                    DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
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
}