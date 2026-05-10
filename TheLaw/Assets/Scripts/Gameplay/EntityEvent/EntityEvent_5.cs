using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum E_EntityEvent_5
{
    TakeAdvantageOfTheSituation,
    AvoidIt,
    Action,
    Mind,
}

public class EntityEvent_5_01 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "趁势";
    public override string OptionDescription { get; protected set; } = "对象“欲望”-2，对“他人”造成2点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 2, E_CompareType.Greater),
                new DiceCondition(E_DiceType.Action, 0, E_CompareType.Any),
            };
        }
    }

    #endregion

    #region 本身属性
    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.TakeAdvantageOfTheSituation;


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
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire,-2);
                }
                ProgressManager.Instance.nowEntities[1].BeAttacked(2);
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
    }
    public void NeverRespond(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(2);
        ProgressManager.Instance.player.BeAttacked(2);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
    }
}
public class EntityEvent_5_02 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 2;
    public override string OptionName { get; protected set; } = "避开";
    public override string OptionDescription { get; protected set; } = "受到1点伤害，对象“欲望”-3";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 4, E_CompareType.Less),
            };
        }
    }

    #endregion

    #region 本身属性
    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.AvoidIt;


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
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire,-3);
                }
                ProgressManager.Instance.player.BeAttacked(1);
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
    }
    public void NeverRespond(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(2);
        ProgressManager.Instance.player.BeAttacked(2);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
    }
    
}

public class EntityEvent_5_03 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 3;
    public override string OptionName { get; protected set; } = "动";
    public override string OptionDescription { get; protected set; } = "受到伤害-3，对象“欲望”-3";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 4, E_CompareType.Less),
            };
        }
    }

    #endregion

    #region 本身属性
    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.Action;


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
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire,-3);
                }
                ProgressManager.Instance.player.BeAttacked(Math.Abs(Math.Abs(ProgressManager.Instance.player.hp -
                                                                             ProgressManager.Instance.nowEntities[1]
                                                                                 .hp) - 3));
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
    }
    public void NeverRespond(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(Math.Abs(ProgressManager.Instance.player.hp -
                                                                     ProgressManager.Instance.nowEntities[1].hp));
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
    }
}

public class EntityEvent_5_04 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 4;
    public override string OptionName { get; protected set; } = "静";
    public override string OptionDescription { get; protected set; } = "受到伤害-3，对象“欲望”+3";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 2, E_CompareType.Greater),
            };
        }
    }

    #endregion

    #region 本身属性
    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.Mind;


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
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire,3);
                }
                ProgressManager.Instance.player.BeAttacked(Math.Abs(Math.Abs(ProgressManager.Instance.player.hp -
                                                                             ProgressManager.Instance.nowEntities[1]
                                                                                 .hp) - 3));
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
    }
    public void NeverRespond(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(Math.Abs(ProgressManager.Instance.player.hp -
                                                                     ProgressManager.Instance.nowEntities[1].hp));
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespond);
    }
}
