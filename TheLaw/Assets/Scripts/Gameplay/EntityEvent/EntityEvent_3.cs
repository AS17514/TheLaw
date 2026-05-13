using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum E_EntityEvent_3
{
    BounceBack,
    Hug,
    Run,
    Exchange,
    Observe,
    Hearken,
    Shortcut,
    ExploreTogether,
    YourCuriosityWillAlwaysBeYourFreedom,
}

public class EntityEvent_3_01_OptionContext : OptionContext
{
    public int index;//1~5
}
public class EntityEvent_3_01 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "弹回";//球
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsUseDiceCombo { get; protected set; } = true;
    public override E_ComboType ComboType { get; protected set; } = E_ComboType.DoubleAction;
    public override bool IsDiceConditionsHave { get; protected set; } = true;

    #endregion

    #region 本身属性
    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.BounceBack;


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

            bool ifNeedOptionContext = false;
            foreach (var characterBase in ProgressManager.Instance.nowEntities)
            {
                if (characterBase == null)
                {
                    continue; 
                }
                if (characterBase is Entity3 e3)
                {
                    continue;
                }
                else
                {
                    if (ProgressManager.Instance.level == 3)
                    {
                        if(characterBase.hp!=0)
                            ifNeedOptionContext=true;
                    }
                }
            }
            if (ifNeedOptionContext)
            {
                if (optionContext is EntityEvent_3_01_OptionContext ctx0)
                {
                    if (ctx0.index > 5 || ctx0.index < 1)
                    {
                        result = false;
                        Debug.Log("ctx0.index>5||ctx0.index<1");
                    }
                    else if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空，无法选择。");
                    }
                    else
                    {
                        if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                        {
                            result = false;
                            Debug.Log("这个部位为空。");
                        }
                    }

                }
                else
                {
                    // 拦截没有传入目标参数的情况
                    result = false;
                    Debug.Log("缺少目标部位上下文。");
                }
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToMarble);
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToBall);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null&&ctx.index!=0)
                    {
                        if(ProgressManager.Instance.nowEntities[ctx.index].hp>0)
                            ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice(IsResponseOption); //消耗骰子

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
    public void NeverRespondToMarble(object info = null)
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
                                                                 - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)));
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToMarble);
    }
    public void NeverRespondToBall(object info = null)
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
                                                                 - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)));
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToBall);
    }
}
public class EntityEvent_3_02 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 2;
    public override string OptionName { get; protected set; } = "拥抱";
    public override string OptionDescription { get; protected set; } = "应对成功时，若双方“欲望”差值<=4，获得胜利";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsUseDiceCombo { get; protected set; } = true;
    public override E_ComboType ComboType { get; protected set; } = E_ComboType.Quadruple;
    public override bool IsDiceConditionsHave { get; protected set; } = true;

    #endregion

    #region 本身属性
    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.Hug;


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
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToGravity);
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice(IsResponseOption); //消耗骰子
                if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
                {
                    if (Math.Abs(e3.GetBuff(E_BuffType.Desire) -
                                 ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)) <= 4)
                    {
                        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
                    }
                }

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
    public void NeverRespondToGravity(object info = null)
    {
        DiceManager.Instance.SetAllTimeDiceToFour();
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToGravity);
    }
}
public class EntityEvent_3_03 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 3;
    public override string OptionName { get; protected set; } = "弹回";//毛球_1
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    //public override bool IsUseDiceCombo { get; protected set; } = true;
    //public override E_ComboType ComboType { get; protected set; } = E_ComboType.DoubleAction;
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

    #endregion

    #region 本身属性
    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.BounceBack;


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

            bool ifNeedOptionContext = false;
            foreach (var characterBase in ProgressManager.Instance.nowEntities)
            {
                if (characterBase == null)
                {
                    continue; 
                }
                if (characterBase is Entity3 e3)
                {
                    continue;
                }
                else
                {
                    if (ProgressManager.Instance.level == 3)
                    {
                        if(characterBase.hp!=0)
                            ifNeedOptionContext=true;
                    }
                }
            }
            if (ifNeedOptionContext)
            {
                if (optionContext is EntityEvent_3_01_OptionContext ctx0)
                {
                    if (ctx0.index > 5 || ctx0.index < 1)
                    {
                        result = false;
                        Debug.Log("ctx0.index>5||ctx0.index<1");
                    }
                    else if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空，无法选择。");
                    }
                    else
                    {
                        if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                        {
                            result = false;
                            Debug.Log("这个部位为空。");
                        }
                    }

                }
                else
                {
                    // 拦截没有传入目标参数的情况
                    result = false;
                    Debug.Log("缺少目标部位上下文。");
                }
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToFurBall1);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null&&ctx.index!=0)
                    {
                        if(ProgressManager.Instance.nowEntities[ctx.index].hp>0)
                            ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice(IsResponseOption); //消耗骰子

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }

    public void NeverRespondToFurBall1(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(1);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToFurBall1);
    }

}
public class EntityEvent_3_04 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 4;
    public override string OptionName { get; protected set; } = "弹回";//毛球_2
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    //public override bool IsUseDiceCombo { get; protected set; } = true;
    //public override E_ComboType ComboType { get; protected set; } = E_ComboType.DoubleAction;
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

    #endregion

    #region 本身属性
    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.BounceBack;


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

            bool ifNeedOptionContext = false;
            foreach (var characterBase in ProgressManager.Instance.nowEntities)
            {
                if (characterBase == null)
                {
                    continue; 
                }
                if (characterBase is Entity3 e3)
                {
                    continue;
                }
                else
                {
                    if (ProgressManager.Instance.level == 3)
                    {
                        if(characterBase.hp!=0)
                            ifNeedOptionContext=true;
                    }
                }
            }
            if (ifNeedOptionContext)
            {
                if (optionContext is EntityEvent_3_01_OptionContext ctx0)
                {
                    if (ctx0.index > 5 || ctx0.index < 1)
                    {
                        result = false;
                        Debug.Log("ctx0.index>5||ctx0.index<1");
                    }
                    else if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空，无法选择。");
                    }
                    else
                    {
                        if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                        {
                            result = false;
                            Debug.Log("这个部位为空。");
                        }
                    }

                }
                else
                {
                    // 拦截没有传入目标参数的情况
                    result = false;
                    Debug.Log("缺少目标部位上下文。");
                }
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToFurBall2);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null&&ctx.index!=0)
                    {
                        if(ProgressManager.Instance.nowEntities[ctx.index].hp>0)
                            ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice(IsResponseOption); //消耗骰子

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }

    public void NeverRespondToFurBall2(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(1);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToFurBall2);
    }

}
public class EntityEvent_3_05 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 5;
    public override string OptionName { get; protected set; } = "弹回";//毛球_3
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    //public override bool IsUseDiceCombo { get; protected set; } = true;
    //public override E_ComboType ComboType { get; protected set; } = E_ComboType.DoubleAction;
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

    #endregion

    #region 本身属性
    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.BounceBack;


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

            bool ifNeedOptionContext = false;
            foreach (var characterBase in ProgressManager.Instance.nowEntities)
            {
                if (characterBase == null)
                {
                    continue; 
                }
                if (characterBase is Entity3 e3)
                {
                    continue;
                }
                else
                {
                    if (ProgressManager.Instance.level == 3)
                    {
                        if(characterBase.hp!=0)
                            ifNeedOptionContext=true;
                    }
                }
            }
            if (ifNeedOptionContext)
            {
                if (optionContext is EntityEvent_3_01_OptionContext ctx0)
                {
                    if (ctx0.index > 5 || ctx0.index < 1)
                    {
                        result = false;
                        Debug.Log("ctx0.index>5||ctx0.index<1");
                    }
                    else if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空，无法选择。");
                    }
                    else
                    {
                        if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                        {
                            result = false;
                            Debug.Log("这个部位为空。");
                        }
                    }

                }
                else
                {
                    // 拦截没有传入目标参数的情况
                    result = false;
                    Debug.Log("缺少目标部位上下文。");
                }
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToFurBall3);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null&&ctx.index!=0)
                    {
                        if(ProgressManager.Instance.nowEntities[ctx.index].hp>0)
                            ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice(IsResponseOption); //消耗骰子

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }

    public void NeverRespondToFurBall3(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(1);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToFurBall3);
    }

}
public class EntityEvent_3_06 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 6;
    public override string OptionName { get; protected set; } = "弹回";//毛球_4
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    //public override bool IsUseDiceCombo { get; protected set; } = true;
    //public override E_ComboType ComboType { get; protected set; } = E_ComboType.DoubleAction;
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

    #endregion

    #region 本身属性
    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.BounceBack;


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

            bool ifNeedOptionContext = false;
            foreach (var characterBase in ProgressManager.Instance.nowEntities)
            {
                if (characterBase == null)
                {
                    continue; 
                }
                if (characterBase is Entity3 e3)
                {
                    continue;
                }
                else
                {
                    if (ProgressManager.Instance.level == 3)
                    {
                        if(characterBase.hp!=0)
                            ifNeedOptionContext=true;
                    }
                }
            }
            if (ifNeedOptionContext)
            {
                if (optionContext is EntityEvent_3_01_OptionContext ctx0)
                {
                    if (ctx0.index > 5 || ctx0.index < 1)
                    {
                        result = false;
                        Debug.Log("ctx0.index>5||ctx0.index<1");
                    }
                    else if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空，无法选择。");
                    }
                    else
                    {
                        if (ProgressManager.Instance.nowEntities[ctx0.index] == null)
                        {
                            result = false;
                            Debug.Log("这个部位为空。");
                        }
                    }

                }
                else
                {
                    // 拦截没有传入目标参数的情况
                    result = false;
                    Debug.Log("缺少目标部位上下文。");
                }
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToFurBall4);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null&&ctx.index!=0)
                    {
                        if(ProgressManager.Instance.nowEntities[ctx.index].hp>0)
                            ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice(IsResponseOption); //消耗骰子

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }

    public void NeverRespondToFurBall4(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(1);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespondToFurBall4);
    }

}
public class EntityEvent_3_07 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 7;
    public override string OptionName { get; protected set; } = "奔跑";

    public override string OptionDescription { get; protected set; } = "时间进度减少时间骰子的点数，投掷一个骰子，自己的“欲望”+对应骰子的点数";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsVisible { get; set; } = true;//
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.TimeAny, 0, E_CompareType.Any),
            };
        }
    }
    #endregion

    #region 本身属性

    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.Run;

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

                EntityDice dice = DiceManager.Instance.AddEntityDice();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDice);
                ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, dice.value);

                // 任务 1：推进时间
                ProgressManager.Instance.AdvancePhase(1);
                ProgressManager.Instance.AddTimeProgress(DiceManager.Instance.selectedDice[0].value);
                // 任务 2：消耗骰子
                DiceManager.Instance.ConsumeValidSelectedDice();
                // 刷新时间骰ui
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_TimeDice);

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
public class EntityEvent_3_08 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 8;
    public override string OptionName { get; protected set; } = "交流";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

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

    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.Exchange;

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
                if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
                {
                    e3.AddBuff(E_BuffType.Desire, 1);
                }
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
public class EntityEvent_3_09 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 9;
    public override string OptionName { get; protected set; } = "观察";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

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

    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.Observe;

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
                Part3_1.PartApear();
                this.IsVisible = false;
                EventManager.Instance.optionPool[E_OptionType.Level3_Option][9].IsVisible = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
public class EntityEvent_3_10 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 10;
    public override string OptionName { get; protected set; } = "观察";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    //public override bool IsVisible { get; set; } = true;//
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

    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.Observe;

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
                Part3_2.PartApear();
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
public class EntityEvent_3_11 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 11;
    public override string OptionName { get; protected set; } = "观察";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsVisible
    {
        get
        {
            return StateManager.Instance.currentState is E_StateType_3.weightless && !this.isItuse;
        }
    }
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

    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.Observe;
    public bool isItuse = false;
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
                Part3_3.PartApear();
                Part3_4.PartApear();
                isItuse = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
public class EntityEvent_3_12 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 12;
    public override string OptionName { get; protected set; } = "聆听";

    public override string OptionDescription { get; protected set; } = "对象“欲望”+2";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsVisible
    {
        get
        {
            if (!IsItuse)
            {
                if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
                {
                    if (e3.ForOption_IsRumorsAboutDisinterestUse
                        && e3.ForOption_IsRunAwayHopingThatYourRunningWonNotBeGivenThatNameAgainUse)
                        return true;
                }
            }

            return false;
        }
    }

    public override E_ComboType ComboType { get; protected set; } = E_ComboType.MindActionPair;
    public override bool IsDiceConditionsHave { get; protected set; } = true;
    public override bool IsUseDiceCombo { get; protected set; } = true;

    #endregion

    #region 本身属性

    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.Hearken;
    public bool IsItuse = false;
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
                if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
                {
                    e3.AddBuff(E_BuffType.Desire, 2);
                }
                IsItuse = true;
                EventManager.Instance.optionPool[E_OptionType.Level3_Option][12].IsVisible = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
public class EntityEvent_3_13 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 13;
    public override string OptionName { get; protected set; } = "捷径";

    public override string OptionDescription { get; protected set; } = "投掷一个骰子，自己的“欲望”+对应骰子的点数";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override DiceCondition[] DiceCost { get; protected set; } = null;
    #endregion

    #region 本身属性

    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.Shortcut;
    public bool IsItuse = false;
    #endregion
    public override void TriggerOption(OptionContext optionContext = null)
    {
        if (IsVisible)
        {
            IsItuse = true;
            EntityDice dice = DiceManager.Instance.AddEntityDice();
            ProgressManager.Instance.player.AddBuff(E_BuffType.Desire, dice.value);
            this.IsVisible = false;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        }
    }
}
public class EntityEvent_3_14 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 14;
    public override string OptionName { get; protected set; } = "一同探究";

    public override string OptionDescription { get; protected set; } = "对象“欲望”-2，时间进度+2";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override E_ComboType ComboType { get; protected set; } = E_ComboType.DoubleMind;
    public override bool IsDiceConditionsHave { get; protected set; } = true;
    public override bool IsUseDiceCombo { get; protected set; } = true;
    #endregion

    #region 本身属性

    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.ExploreTogether;
    public bool IsItuse = false;
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
                if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
                {
                    if (e3.GetBuff(E_BuffType.Desire) > 2)
                        e3.AddBuff(E_BuffType.Desire, -2);
                    else
                        e3.AddBuff(E_BuffType.Desire, -e3.GetBuff(E_BuffType.Desire));
                }

                IsItuse = true;
                ProgressManager.Instance.AddTimeProgress(2);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice();

            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}
public class EntityEvent_3_15 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 3;
    public override int OptionID { get; protected set; } = 15;
    public override string OptionName { get; protected set; } = "你的好奇永远是你的自由";

    public override string OptionDescription { get; protected set; } = "?";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsVisible
    {
        get
        {
            if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][11] is EntityEvent_3_12 e12)
            {
                if (EventManager.Instance.optionPool[E_OptionType.Level3_Option][13] is EntityEvent_3_14 e14)
                    if (e12.IsItuse && e14.IsItuse)
                        return true;
            }
            return false;
        }
    }
    public override E_ComboType ComboType { get; protected set; } = E_ComboType.Quadruple;
    public override bool IsDiceConditionsHave { get; protected set; } = true;
    public override bool IsUseDiceCombo { get; protected set; } = true;
    #endregion

    #region 本身属性

    E_EntityEvent_3 entityEvent_3Type = E_EntityEvent_3.YourCuriosityWillAlwaysBeYourFreedom;

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
            }
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }

        }
    }
}

