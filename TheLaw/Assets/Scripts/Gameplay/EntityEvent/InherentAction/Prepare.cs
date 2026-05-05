using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Prepare : OptionBase
{
    public override string OptionName { get; protected set; } = "准备";
    public override int OptionID
    {
        get { return 0; }

    }

    public override string OptionDescription
    {
        get { return "选择消耗一个时间骰子，选择行动或思维，投掷并获得一个对应的骰子；推进时间进度"; }
    }

    public override bool IsVisible
    {
        get { return true; }
    }

    public override E_OptionType OptionType
    {
        get { return E_OptionType.Player_InherentAction; }
    }

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.TimeAny,1,E_CompareType.Any)
            };
        }
    }

    public override Action ExecuteLogic
    {
        get
        {
            return () =>
            {
                // 找出真正被选中且符合条件的那个骰子
                int timeValue = 0;
                foreach (var dice in DiceManager.Instance.selectedDice)
                {
                    if (dice.isValid) // IsSelectionValid 验证成功时打的标记
                    {
                        timeValue = dice.value;
                        break;
                    }
                }

                // 任务 1：推进时间
                ProgressManager.Instance.AdvancePhase(1);
                ProgressManager.Instance.AddTimeProgress(timeValue);
                // 任务 2：消耗骰子
                DiceManager.Instance.ConsumeValidSelectedDice();
                // 刷新时间骰ui
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDice);
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
            if (result && optionContext is PrepareOptionContext)
            {
                ExecuteLogic?.Invoke();
                DiceManager.Instance.AddDice((optionContext as PrepareOptionContext).diceType, null);
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
        }
    }
}
