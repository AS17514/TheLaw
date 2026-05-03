using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colorfull : OptionBase
{
    public override string OptionName { get; protected set; } = "鲜艳";

    public override int OptionID
    {
        get { return 8; }
        
    }
    public override string OptionDescription
    {
        get { return "选择任意数量的行动或思维骰子，将其全部转化为某个点数的对应骰子"; }
    }

    public override bool IsVisible 
    {
        get
        {
            if(ProgressManager.Instance.level>2)
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
        if (!IsVisible) return;

        // 1.过一下特殊条件安检
        bool result = IsSpecialConditionsHave ? EventManager.Instance.IsSpecialConditionsMet(specialConditions) : true;

        // 2. 专属骰子安检
        if (result)
        {
            var selected = DiceManager.Instance.selectedDice;

            // 如果玩家一个都没选，肯定不行
            if (selected == null || selected.Count == 0)
            {
                result = false;
            }
            else
            {
                // 遍历玩家选中的所有骰子
                foreach (var dice in selected)
                {
                    // 如果发现里面混进了时间骰子，直接判定失败
                    if (dice.type == E_DiceType.Time1 || dice.type == E_DiceType.Time2 || 
                        dice.type == E_DiceType.Time3 || dice.type == E_DiceType.Time4)
                    {
                        result = false;
                        break; 
                    }
                    //里面不能有万能骰子
                    if(dice.type == E_DiceType.Wild)
                    {
                        result = false;
                        break; 
                    }
                }
            }
            // 【非常关键的一步】
            // 因为没走 DiceManager 的安检，我们需要手动给通过测试的骰子盖章 (isValid = true)
            // 只有盖了章，一会 ExecuteLogic 里的 ConsumeValidSelectedDice 才知道该销毁哪些骰子
            if (result && optionContext is ColorfullOptionContext colorfullCtx)
            {
                if (colorfullCtx.i < 7 && colorfullCtx.i > 0)
                {
                    foreach (var dice in selected)
                    {
                        dice.value = colorfullCtx.i;
                        dice.isValid = true;
                    }
                }
                else
                {
                    Debug.Log("传的参数必须是1~7。");
                }
            }
        }

        // 3. 执行结果
        if (result)
        {
            DiceManager.Instance.ConsumeValidSelectedDice();
        }
        else
        {
            DiceManager.Instance.ClearSelected();
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
        }
    }
}
