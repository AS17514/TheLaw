using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                // 1. 获取列表中你想要的那个骰子（必然是第0个）
                DiceBase targetDice = DiceManager.Instance.selectedDice[0];

                // 2. 计算回复量（点数 - 1）
                int healAmount = targetDice.value - 1;

                // 3. 执行回复生命逻辑 
                if (healAmount > 0)
                {
                    BuffManager.Instance.player.hp += healAmount;
                    // 记得通知 UI 刷新血量
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerHP);
                }

                // 4. 将其点数变为1
                // 注意：DiceManager.ModifyDieValue 接收的是“变化量(change)”，而不是目标数值。
                // 所以变化量 = 目标值(1) - 当前值
                DiceManager.Instance.ModifyDieValue(targetDice, 1 - targetDice.value);

                // 5. 触发事件并清空选中框（【删除了 ConsumeValidSelectedDice】，因为这是修改而不是消耗）
                DiceManager.Instance.ClearSelected();

                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToUnavailable);

                // 触发玩家执行了动作的事件，推进游戏流程
                EventCenter.Instance.EventTrigger(E_EventType.Logic_PlayerActionExecuted);
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
