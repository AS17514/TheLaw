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
    public override bool IsDiceConditionsHave
    {
        get { return false; } 
    }

    public override void TriggerOption(OptionContext optionContext = null)
    {
        if (!IsVisible) return;

        // 1. 基础条件安检
        bool result = IsSpecialConditionsHave ? EventManager.Instance.IsSpecialConditionsMet(specialConditions) : true;

        if (result)
        {
            var selected = DiceManager.Instance.selectedDice;

            // 2. 检查玩家是否选了骰子，以及是否混入了违规骰子
            if (selected == null || selected.Count == 0)
            {
                result = false;
            }
            else
            {
                foreach (var dice in selected)
                {
                    if (dice.type == E_DiceType.Time1 || dice.type == E_DiceType.Time2 || 
                        dice.type == E_DiceType.Time3 || dice.type == E_DiceType.Time4 ||
                        dice.type == E_DiceType.Wild)
                    {
                        result = false;
                        break; 
                    }
                }
            }

            // 3. 执行核心转化逻辑
            if (result)
            {
                // 确保正确传入了包含目标点数的 Context
                if (optionContext is ColorfullOptionContext colorfullCtx && colorfullCtx.i > 0 && colorfullCtx.i < 7)
                {
                    // 使用 HashSet 记录需要刷新的骰子类型，避免同类型重复刷新
                    HashSet<E_DiceType> typesToUpdate = new HashSet<E_DiceType>();

                    // 批量修改点数
                    foreach (var dice in selected)
                    {
                        dice.value = colorfullCtx.i;
                        typesToUpdate.Add(dice.type);
                    }

                    // 触发相关的事件和 UI 更新
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToUnavailable);
                    
                    // 清空选中框，让骰子退回池子展示区
                    DiceManager.Instance.ClearSelected();

                    // 通知 DiceManager 重新排序并刷新这些受影响的类型的 UI
                    foreach (var type in typesToUpdate)
                    {
                        DiceManager.Instance.SortPoolByValue(type);
                    }

                    // 触发玩家执行了动作的事件
                    EventCenter.Instance.EventTrigger(E_EventType.Logic_PlayerActionExecuted);
                    
                    if (ProgressManager.Instance.level == 5 &&
                        StateManager.Instance.currentState is E_StateType_5.unbalance)
                    {
                        StateManager.Instance.SetDesireIndex(2);
                    }
                    return; // 成功执行，直接结束方法
                }
                else
                {
                    Debug.Log("未传入正确的 ColorfullOptionContext，或者点数超出 1~6 范围！");
                    result = false; // 参数不对，强行判定为失败
                }
            }
        }

        // 4. 失败分支：退回骰子并提示条件不足
        DiceManager.Instance.ClearSelected();
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
    
    }
}
