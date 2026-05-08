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
    Observe,
    Push,
    Ignore,
    ThatIsNotYourDisguise,
    Slander
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
                if(EventCenter.Instance.IsEventListenersNull(E_EventType.Logic_PlayerActionExecuted, NeverRespondToScorchingSun))
                    ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters, 1);
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToScorchingSun);
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToStress);
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
        if (ProgressManager.Instance.player.GetBuff(E_BuffType.Desire) > 0)
        {
            if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
                if(entity2.GetBuff(E_BuffType.Desire)>0)
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

        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToScorchingSun);

    }

    public void NeverRespondToStress(object info = null)
    {
        int atk = ProgressManager.Instance.player.GetBuff(E_BuffType.Tatters);
        int playerDesire = ProgressManager.Instance.player.GetBuff(E_BuffType.Desire);
        if (playerDesire > 0)
        {
            if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
                if(entity2.GetBuff(E_BuffType.Desire)>0)
                    ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, -1);
        }
        else
        {
            ProgressManager.Instance.player.BeAttacked(atk);
        }
        //取消该选项的显示。
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);

        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToStress);

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
                isOptionAskCouldUse = true;
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][3].IsVisible = true;
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][4].IsVisible = true;
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

    public override string OptionDescription { get; protected set; } = "（正常状态下才能执行）将对象的“行动”轮换至下一个";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;

    public override bool IsVisible
    {
        get
        {
            if (StateManager.Instance.currentState is E_StateType_2.normal
                && EventManager.Instance.optionPool[E_OptionType.Level2_Option][1] is EntityEvent_2_02 e2)
            {
                if (e2.isOptionAskCouldUse)
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

    public override string OptionDescription { get; protected set; } = "对象的“欲望”-1，自己“欲望”+1";

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
                    entity2.AddBuff(E_BuffType.Desire, -1);
                }

                ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, 1);
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

    public override string OptionDescription { get; protected set; } = "?";

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

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Praise;

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
                        ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters, 2);
                        entity2.ashamed_Action_Stress();
                    }
                }
                DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
                EventManager.Instance.optionPool[E_OptionType.Level2_Option][5].IsVisible = true;
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

    public override string OptionDescription { get; protected set; } = "?";

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

    public bool isEncourageCouldUse = false;

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
                            ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters, -1);
                            ProgressManager.Instance.AddTimeProgress(3);
                        }
                    }

                    if (StateManager.Instance.currentState is E_StateType_2.ashamed)
                    {
                        if (ProgressManager.Instance.nowEntities[1] is Part2_1 part2_1)
                        {
                            part2_1.hp = 1;
                            part2_1.isDestroyed = false;
                            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP);
                        }

                        StateManager.Instance.ChangeState(E_StateType_2.normal);
                        ProgressManager.Instance.timeProgress = 0;
                        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeProgress);
                    }

                    if (StateManager.Instance.currentState is E_StateType_2.hysterial)
                    {
                        //无效果
                    }
                    isEncourageCouldUse = true;
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

public class EntityEvent_2_07 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 7;
    public override string OptionName { get; protected set; } = "鼓励";

    public override string OptionDescription { get; protected set; } = "？";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 1, E_CompareType.Any),
                new DiceCondition(E_DiceType.Mind, 1, E_CompareType.Any),
                new DiceCondition(E_DiceType.Mind, 6, E_CompareType.Equal),
            };
        }
    }

    public override bool IsVisible
    {
        get
        {
            if (EventManager.Instance.optionPool[E_OptionType.Level2_Option][5] is EntityEvent_2_06 entityEvent_2_06)
                return entityEvent_2_06.isEncourageCouldUse
                       && (StateManager.Instance.currentState is E_StateType_2.ashamed);
            return false;
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
                //对象时间进度变为1
                ProgressManager.Instance.AddTimeProgress(1 - ProgressManager.Instance.timeProgress);
                StateManager.Instance.ChangeState(E_StateType_2.composed);

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

public class EntityEvent_2_08 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 8;
    public override string OptionName { get; protected set; } = "那都是你独一无二的装饰";

    public override string OptionDescription { get; protected set; } = "？";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;

    //“破布”*7
    public override bool IsDiceConditionsHave { get; protected set; } = false;
    public override bool IsSpecialConditionsHave { get; protected set; } = true;

    public override E_SpecialOptionConditions specialConditions { get; protected set; } =
        E_SpecialOptionConditions.Tatters1;
    //

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
                ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters, -7);
                //获得胜利
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
public class EntityEvent_2_09 : OptionBase
{
    #region OptionBase属性


    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 9;
    public override string OptionName { get; protected set; } = "观察";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;

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

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Observe;

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
                Part2_1.PartApear();
                IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);//更新事件列表
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

public class EntityEvent_2_10 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 10;
    public override string OptionName { get; protected set; } = "推动";

    public override string OptionDescription { get; protected set; } = "破坏“晾衣绳”，消耗一个“破布”";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;


    //public override bool IsDiceConditionsHave{ get; protected set; } =true;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 2, E_CompareType.Greater),
                new DiceCondition(E_DiceType.Action, 3, E_CompareType.Less),
            };
        }
    }

    public override bool IsVisible
    {
        get
        {
            if (StateManager.Instance.currentState is E_StateType_2.normal)
                if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
                {
                    if (entity2.isHiddenBehindTheClothesUse)
                        return true;
                }
            return false;

        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Push;

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
                if (ProgressManager.Instance.nowEntities[1] == null)
                    Part2_1.PartApear();

                ProgressManager.Instance.nowEntities[1].BeAttacked(1);
                if (ProgressManager.Instance.player.GetBuff(E_BuffType.Tatters) > 0)
                    ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters, -1);

                DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子

                isPushUse = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }

    public bool isPushUse = false;
}

public class EntityEvent_2_11 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 11;
    public override string OptionName { get; protected set; } = "无视";

    public override string OptionDescription { get; protected set; } = "消耗二个“破布”";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;


    //public override bool IsDiceConditionsHave{ get; protected set; } =true;

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

    public override bool IsVisible
    {
        get
        {
            if (StateManager.Instance.currentState is E_StateType_2.ashamed)
                if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
                {
                    if (entity2.isHiddenBehindTheClothesUse)
                        if (EventManager.Instance.optionPool[E_OptionType.Level2_Option][9] is EntityEvent_2_10 entityEvent_2_10)
                            if (entityEvent_2_10.isPushUse)
                                return true;
                }
            return false;

        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Ignore;

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
                if (ProgressManager.Instance.player.GetBuff(E_BuffType.Tatters) > 1)
                    ProgressManager.Instance.player.AddBuff(E_BuffType.Tatters, -2);
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

public class EntityEvent_2_12 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 12;
    public override string OptionName { get; protected set; } = "那不是你的伪装";

    public override string OptionDescription { get; protected set; } = "对象的“欲望”-3";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;


    //public override bool IsDiceConditionsHave{ get; protected set; } =true;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 1, E_CompareType.Greater),
                new DiceCondition(E_DiceType.Action, 5, E_CompareType.Less),
            };
        }
    }

    public override bool IsVisible
    {
        get
        {
            if (ProgressManager.Instance.nowEntities[0] is Entity2 entity2)
            {
                if (entity2.isIfThatCountsAsMyClothesTooUse)
                    return true;
            }
            return false;

        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.ThatIsNotYourDisguise;

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
                    entity2.AddBuff(E_BuffType.Desire, -3);

                EventManager.Instance.optionPool[E_OptionType.Level2_Option][12].IsVisible = true;
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

public class EntityEvent_2_13 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 13;
    public override string OptionName { get; protected set; } = "中伤";

    public override string OptionDescription { get; protected set; } = "消耗一个“破布”,对象的“欲望”-1，自己“欲望”+1";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;


    //public override bool IsDiceConditionsHave{ get; protected set; } =true;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 4, E_CompareType.Greater),
                new DiceCondition(E_DiceType.Action, 2, E_CompareType.Equal),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_2 entityEvent_2Type = E_EntityEvent_2.Slander;

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
                    if (entity2.GetBuff(E_BuffType.Desire) > 0)
                    {
                        entity2.AddBuff(E_BuffType.Desire, -1);
                    }
                    ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, 1);
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