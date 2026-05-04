using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EntityEvent_2
{
    Escape,
    Exchange,
    Ask,
    Require,
    Praise,
    Comfort,
    Encourage,
    
}

public class EntityEvent_2_01 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "逃避";

    public override string OptionDescription { get; protected set; } = "逃避";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;
    // public override bool IsVisible { get; set; } = true;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 3, E_CompareType.Less),
                new DiceCondition(E_DiceType.Action, 4, E_CompareType.Less),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Escape;

    #endregion

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
                ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters,1);
                //应对完之后取消该选项的显示。
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
            
        }
        else
        {
            Debug.Log("为什么这个选项不可见，还会发来选项执行的请求。。。出自怪物二的玩家应对行为逃避。");
        }
    }

    public void NeverRespondToScorchingSun(object info = null)
    {
        if (ProgressManager.Instance.player.GetBuff(E_BuffType.Desire) >0)
        {
            ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, -1);        
        }

        else if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
        {
            //造成对象“欲望”数的伤害
            ProgressManager.Instance.player.BeAttacked(entity2.GetBuff(E_BuffType.Desire));
        }
        //取消该选项的显示。
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToScorchingSun);
        
    }

    public void NeverRespondToStress(object info = null)
    {
        int atk=ProgressManager.Instance.player.GetBuff(E_BuffType.Tatters);
        int playerDesire=ProgressManager.Instance.player.GetBuff(E_BuffType.Desire);
        if (playerDesire > 0)
        {
            ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, -1);
        }
        else
        {
            ProgressManager.Instance.player.BeAttacked(atk);
        }
        //取消该选项的显示。
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToStress);

    }
}

public class EntityEvent_2_02 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 2;
    public override string OptionName { get; protected set; } = "交流";

    public override string OptionDescription { get; protected set; } = "交流";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;

    public override bool IsVisible { get; set; } = true;//
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 1, E_CompareType.Any),
            };
        }
    }
    #endregion
    
    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Exchange;
    public bool isOptionAskCouldUse = false;

    #endregion
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
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][2].IsVisible=true;
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][3].IsVisible=true;
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][4].IsVisible=true;
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][1].IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}

public class EntityEvent_2_03 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 3;
    public override string OptionName { get; protected set; } = "询问";

    public override string OptionDescription { get; protected set; } = "询问";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;

    public override bool IsVisible
    {
        get
        {
            if (StateManager.Instance.currentState is E_StateType_2.normal
                && EventManager.Instance.optionPool[E_OptionType.Level2_Option][1] is EntityEvent_2_02 e2)
            {
                if(e2.isOptionAskCouldUse)
                    return true;
            }
            
            return false;
            
        }
    }
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 3, E_CompareType.Less),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Ask;

    #endregion

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
                StateManager.Instance.ChangeCurrentActionToNextAction();
                DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}

public class EntityEvent_2_04 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 4;
    public override string OptionName { get; protected set; } = "请求";

    public override string OptionDescription { get; protected set; } = "请求";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;

    //public override bool IsVisible{get; set; } = true;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 3, E_CompareType.Less),
                new DiceCondition(E_DiceType.Mind, 2, E_CompareType.Greater),
                new DiceCondition(E_DiceType.Mind, 1, E_CompareType.Any),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Require;

    #endregion

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
                if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
                {
                    entity2.AddBuff(E_BuffType.Desire,-1);
                }
                    
                ProgressManager.Instance.player.AddBuff(E_BuffType.Desire,1);
                DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}

public class EntityEvent_2_05 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 5;
    public override string OptionName { get; protected set; } = "夸奖";

    public override string OptionDescription { get; protected set; } = "夸奖";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;
    
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 1, E_CompareType.Any),
                new DiceCondition(E_DiceType.Mind, 3, E_CompareType.Greater),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Encourage;

    #endregion

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
                if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
                {
                    if (StateManager.Instance.currentState is E_StateType_2.normal)
                    {
                        entity2.ashamed_Action_Stress();
                    }
                    
                    if (StateManager.Instance.currentState is E_StateType_2.ashamed)
                    {
                        ProgressManager.Instance.AddTimeProgress(3);
                    }
                    
                    if (StateManager.Instance.currentState is E_StateType_2.hysterial)
                    {
                        ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters,2);
                        entity2.ashamed_Action_Stress();
                    }
                }
                DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][5].IsVisible=true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
public class EntityEvent_2_06 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 6;
    public override string OptionName { get; protected set; } = "安抚";

    public override string OptionDescription { get; protected set; } = "安抚";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;
    
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 1, E_CompareType.Any),
                new DiceCondition(E_DiceType.Mind, 4, E_CompareType.Greater),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Comfort;

    #endregion

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
                if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
                {
                    if (StateManager.Instance.currentState is E_StateType_2.normal)
                    {
                        if (ProgressManager.Instance.player.GetBuff(E_BuffType.Tatters) > 0)
                        {
                            ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters,-1);
                            ProgressManager.Instance.AddTimeProgress(3);
                        }
                    }
                    
                    if (StateManager.Instance.currentState is E_StateType_2.ashamed)
                    {
                        //Todo:羞耻状态下：令“晾衣绳”回满血量并取消它的破坏，对象时间进度重置为4，并进入正常状态
                        StateManager.Instance.ChangeState(E_StateType_2.normal);
                    }
                    
                    if (StateManager.Instance.currentState is E_StateType_2.hysterial)
                    {
                        //无效果
                    }
                }
                DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
