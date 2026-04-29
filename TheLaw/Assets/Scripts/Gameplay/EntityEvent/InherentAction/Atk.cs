using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atk : OptionBase
{
    public override string OptionName { get; protected set; } = "攻击";
    public override int OptionID
    {
        get { return 3; } 
    }

    public override string OptionDescription
    {
        get { return "选择消耗自己2个行动骰子，造成差值的伤害(无法使用百搭骰子)"; }
    }

    public override bool IsVisible 
    {
        get { return true; }
    }

    public override E_OptionType OptionType
    {
        get { return E_OptionType.Player_InherentAction; }
    }

    // 这里其实不写 DiceCost 也可以，因为我们要在下面完全接管判定
    public override DiceCondition[] DiceCost
    {
        get { return null; } 
    }

    public override void TriggerOption(OptionContext optionContext = null)
    {
        if (!IsVisible) return;

        // 1. 特殊条件检测(应该不需要)
        bool result = IsSpecialConditionsHave ? EventManager.Instance.IsSpecialConditionsMet(specialConditions) : true;
        
        List<DiceBase> selected = DiceManager.Instance.selectedDice;

        // 2. 验证：必须刚好选中 2 颗骰子
        if (result && selected != null && selected.Count == 2&& optionContext is AtkOptionContext atkCtx)
        {
            DiceBase dice1 = selected[0];
            DiceBase dice2 = selected[1];

            // 3. 判定：两颗骰子都必须是Action
            // 只要不是 Action，无论是 Wild 还是 Time，统统不放行！
            if (dice1.type == E_DiceType.Action && dice2.type == E_DiceType.Action)
            {
                // 4. 计算差值伤害 (用 Mathf.Abs 取绝对值，防止负数)
                int damage = Mathf.Abs(dice1.value - dice2.value);
                if (atkCtx.index<5&&atkCtx.index>0&&ProgressManager.Instance.nowEntities[atkCtx.index]!=null)
                {
                    // 扣血逻辑 
                    ProgressManager.Instance.nowEntities[atkCtx.index].BeAttacked(damage);

                    // 5. 将选中的骰子标记为“合法”，然后调用管理器的统一消耗方法
                    dice1.isValid = true;
                    dice2.isValid = true;
                    DiceManager.Instance.ConsumeValidSelectedDice();

                    return; // 技能顺利执行完毕，退出
                }
                else
                {
                    Debug.Log("传的参数必须是0~4");
                    return;
                }
            }
        }

        // 只要数量不对、或者混进了百搭/其他骰子，统一触发报错提示
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
    }
}
