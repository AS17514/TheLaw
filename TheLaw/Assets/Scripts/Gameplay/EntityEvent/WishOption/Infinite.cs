using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infinite : OptionBase
{
    public override string OptionName { get; protected set; } = "无限";

    public override int OptionID
    {
        get { return 9; }
        
    }
    public override string OptionDescription
    {
        get { return "此时间段首次执行动时，将返还消耗的行动和思维骰(不返还百搭骰子）"; }
    }

    public override bool IsVisible 
    {
        get
        {
            if(ProgressManager.Instance.level>3)
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

    public bool isInfinite=false;
    
    public override void TriggerOption(OptionContext optionContext = null)
    {
        isInfinite=true;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_WishToUnavailable);
        if (ProgressManager.Instance.level == 5 &&
            StateManager.Instance.currentState is E_StateType_5.unbalance)
        {
            StateManager.Instance.SetDesireIndex(3);
        }
    }
}
