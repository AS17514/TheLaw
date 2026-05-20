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
    Exchange,
    ObserveBefore,
    ObserveLeft,
    TurnLeft,
    ObserveRight,
    TurnRight,
    Memories,
    WhoseWish,
    WhoseFigure,
    TheStarsDrawn,
    FragmentedGlowingLight,
    Analysis,
    WhyAreTheirFacesSoRepulsive,
    TheChildWhoMadeAPromiseToMe,
    TheSoundOfFriction
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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire, -2);
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
            LastTriggerSuccess = result;

        }
    }
    public void NeverRespond(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(2);
        ProgressManager.Instance.player.BeAttacked(2);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire, -3);
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
            LastTriggerSuccess = result;

        }
    }
    public void NeverRespond(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(2);
        ProgressManager.Instance.player.BeAttacked(2);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire, -3);
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
            LastTriggerSuccess = result;

        }
    }
    public void NeverRespond(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(Math.Abs(ProgressManager.Instance.player.hp -
                                                                     ProgressManager.Instance.nowEntities[1].hp));
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire, 3);
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
            LastTriggerSuccess = result;

        }
    }
    public void NeverRespond(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(Math.Abs(ProgressManager.Instance.player.hp -
                                                                     ProgressManager.Instance.nowEntities[1].hp));
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
    }
}

public class EntityEvent_5_05 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 5;
    public override string OptionName { get; protected set; } = "交流";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible { get; set; } = true;//
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 0, E_CompareType.Any),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.Exchange;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_06 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 6;
    public override string OptionName { get; protected set; } = "观察·前";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible { get; set; } = true;//
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 0, E_CompareType.Any),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.ObserveBefore;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                IsVisible = false;
                Part5_1.PartApear();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_07 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 7;
    public override string OptionName { get; protected set; } = "观察·左";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible { get; set; } = true;//
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 0, E_CompareType.Any),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.ObserveLeft;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                IsVisible = false;
                EventManager.Instance.optionPool[E_OptionType.Level5_Option][7].IsVisible = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_08 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 8;
    public override string OptionName { get; protected set; } = "向左";

    public override string OptionDescription { get; protected set; } = "对象的“欲望”-行动骰的点数";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 0, E_CompareType.Any),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.TurnLeft;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire, -DiceManager.Instance.selectedDice[0].value);
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_09 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 9;
    public override string OptionName { get; protected set; } = "观察·右";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible { get; set; } = true;//
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 0, E_CompareType.Any),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.ObserveRight;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                IsVisible = false;
                EventManager.Instance.optionPool[E_OptionType.Level5_Option][9].IsVisible = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_10 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 10;
    public override string OptionName { get; protected set; } = "向右";

    public override string OptionDescription { get; protected set; } = "对象的“欲望”+思维骰的点数";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 0, E_CompareType.Any),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.TurnRight;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire, DiceManager.Instance.selectedDice[0].value);
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_11 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 11;
    public override string OptionName { get; protected set; } = "回忆";

    public override string OptionDescription { get; protected set; } = "？";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible
    {
        get
        {
            if (StateManager.Instance.currentState is E_StateType_5.unbalance && !IsItUse)
                return true;
            else
            {
                return false;
            }
        }
    }

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 3, E_CompareType.Less),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.Memories;

    public bool IsItUse = false;
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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                IsItUse = true;
                DiceManager.Instance.AddDice(E_DiceType.Action);
                DiceManager.Instance.AddDice(E_DiceType.Mind);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire, 2);
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_12 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 12;
    public override string OptionName { get; protected set; } = "谁的愿望";

    public override string OptionDescription { get; protected set; } = "？";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible
    {
        get
        {
            if (StateManager.Instance.currentState is E_StateType_5.unbalance && !IsItUse
                                                                            && EventManager.Instance.optionPool[E_OptionType.Level5_Option][10] is EntityEvent_5_11 e11)
            {
                if (e11.IsItUse)
                    return true;
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 4, E_CompareType.Greater),
                new DiceCondition(E_DiceType.Action, 3, E_CompareType.Less),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.WhoseWish;

    public bool IsItUse = false;
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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                IsItUse = true;
                ProgressManager.Instance.player.hp = Math.Clamp(ProgressManager.Instance.player.hp + 7,
                    ProgressManager.Instance.player.hp,
                    ProgressManager.Instance.player.maxHp);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerHP);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_13 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 13;
    public override string OptionName { get; protected set; } = "谁的身影";

    public override string OptionDescription { get; protected set; } = "？";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible
    {
        get
        {
            if (StateManager.Instance.currentState is E_StateType_5.unbalance && !IsItUse
                                                                            && EventManager.Instance.optionPool[E_OptionType.Level5_Option][11] is EntityEvent_5_12 e12)
            {
                if (e12.IsItUse)
                    return true;
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public override bool IsDiceConditionsHave { get; protected set; } = true;

    public override bool IsUseDiceCombo { get; protected set; } = true;
    public override E_ComboType ComboType { get; protected set; } = E_ComboType.TripleMind;

    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.WhoseFigure;

    public bool IsItUse = false;
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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                IsItUse = true;
                DiceManager.Instance.AddDice(E_DiceType.Action);
                DiceManager.Instance.AddDice(E_DiceType.Action);
                DiceManager.Instance.AddDice(E_DiceType.Action);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire, -e5.GetBuff(E_BuffType.Desire));
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_14 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 14;
    public override string OptionName { get; protected set; } = "画下的星星";

    public override string OptionDescription { get; protected set; } = "？";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible
    {
        get
        {
            bool result = false;
            if (StateManager.Instance.currentState is E_StateType_5.equipoise
               && EventManager.Instance.optionPool[E_OptionType.Level5_Option][12] is EntityEvent_5_13 e13)
            {
                if (e13.IsItUse)
                    result = true;
                
            }
            if (ProgressManager.Instance.TryGetPart(1, out Part part)&& part.isDestroyed)
            {
                result = true;
            }
            return result;
        }
    }
    public override bool IsDiceConditionsHave { get; protected set; } = true;
    public override bool IsUseDiceCombo { get; protected set; } = true;

    public override E_ComboType ComboType { get; protected set; } = E_ComboType.TripleMind;

    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.TheStarsDrawn;

    public bool IsItUse = false;
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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {

                DiceManager.Instance.AddDice(E_DiceType.Wild);
                IsItUse = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_15 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 15;
    public override string OptionName { get; protected set; } = "破碎地发光";

    public override string OptionDescription { get; protected set; } = "获得胜利";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible
    {
        get
        {
            if (StateManager.Instance.currentState is E_StateType_5.equipoise
               && EventManager.Instance.optionPool[E_OptionType.Level5_Option][13] is EntityEvent_5_14 e14)
            {
                if (e14.IsItUse)
                    return true;
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }

    public override bool IsDiceConditionsHave { get; protected set; } = true;
    public override bool IsUseDiceCombo { get; protected set; } = true;

    public override E_ComboType ComboType { get; protected set; } = E_ComboType.TripleMind;
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.FragmentedGlowingLight;

    public bool IsItUse = false;
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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_16 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 16;
    public override string OptionName { get; protected set; } = "分析";

    public override string OptionDescription { get; protected set; } = "获得3个点数为1的动般，对象“欲望”+5";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override bool IsVisible { get; set; } = true;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 0, E_CompareType.Any),
                new DiceCondition(E_DiceType.Mind, 0, E_CompareType.Any),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.Analysis;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                ActionDice dice1 = new ActionDice();
                ActionDice dice2 = new ActionDice();
                ActionDice dice3 = new ActionDice();
                dice1.value = 1;
                dice2.value = 1;
                dice3.value = 1;
                DiceManager.Instance.AddDice(E_DiceType.Action, dice1);
                DiceManager.Instance.AddDice(E_DiceType.Action, dice2);
                DiceManager.Instance.AddDice(E_DiceType.Action, dice3);
                if (ProgressManager.Instance.nowEntities[0] is Entity5 e5)
                {
                    e5.AddBuff(E_BuffType.Desire, 5);
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_17 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 17;
    public override string OptionName { get; protected set; } = "为什么他们的面容如此可憎呢？";

    public override string OptionDescription { get; protected set; } = "时间进度+8";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 6, E_CompareType.Equal),
                new DiceCondition(E_DiceType.Mind, 6, E_CompareType.Equal),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.WhyAreTheirFacesSoRepulsive;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                ProgressManager.Instance.AddTimeProgress(8);
                EventManager.Instance.optionPool[E_OptionType.Level5_Option][17].IsVisible = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                        }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_18 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 18;
    public override string OptionName { get; protected set; } = "那个和我许下承诺的女孩";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 1, E_CompareType.Equal),
                new DiceCondition(E_DiceType.Mind, 1, E_CompareType.Equal),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.TheChildWhoMadeAPromiseToMe;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                if (ProgressManager.Instance.nowEntities[1] is Part5_1 part5_1)
                {
                    part5_1.isHpLocked = false;
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
            LastTriggerSuccess = result;

        }
    }
}
public class EntityEvent_5_19 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 5;
    public override int OptionID { get; protected set; } = 19;
    public override string OptionName { get; protected set; } = "摩擦的声音";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level5_Option;
    public override int fatherID { get; protected set; } = 5;

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

    E_EntityEvent_5 entityEvent_5Type = E_EntityEvent_5.TheSoundOfFriction;

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
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(ComboType)
                    : true;
            }
            else
            {
                result &= IsDiceConditionsHave
                    ? DiceManager.Instance.IsSelectionValid(DiceCost)
                    : true;
            }

            if (result)
            {
                DiceManager.Instance.AddDice(E_DiceType.Action);
                DiceManager.Instance.AddDice(E_DiceType.Action);
                if (ProgressManager.Instance.player.GetBuff(E_BuffType.Right) > 0)//玩家消耗时间骰时时间进度额外-1
                    ProgressManager.Instance.AddTimeProgress(-3);
                else
                {
                    ProgressManager.Instance.AddTimeProgress(-2);
                }
                ProgressManager.Instance.TryGetEntity().AddBuff(E_BuffType.Desire,-2);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
            LastTriggerSuccess = result;

        }
    }
}
