using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum E_EntityEvent_3
{
    BounceBack,
}

public class EntityEvent_3_01_OptionContext : OptionContext
{
    public int index;//1~5
}
public class EntityEvent_3_01 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "弹回";//球
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsUseDiceCombo { get; protected set; } = true;
    public override E_ComboType ComboType { get; protected set; } = E_ComboType.DoubleAction;
    public override bool IsDiceConditionsHave { get; protected set; }=false;

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

            if (optionContext is EntityEvent_3_01_OptionContext ctx0)
            {
                if(ctx0.index>5||ctx0.index<1)
                {
                    result = false;
                    Debug.Log("ctx0.index>5||ctx0.index<1");
                }
                else
                {
                    if(ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空。");
                    }
                }
                
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToMarble);
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToBall);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null)
                    {
                        ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
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
    public void NeverRespondToMarble(object info = null)
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
                                                                 - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)));
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToBall);
    }
    public void NeverRespondToBall(object info = null)
    {
        if (ProgressManager.Instance.nowEntities[0] is Entity3 e3)
            ProgressManager.Instance.player.BeAttacked(Mathf.Abs(e3.GetBuff(E_BuffType.Desire)
                                                                 - ProgressManager.Instance.player.GetBuff(E_BuffType.Desire)));
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToBall);
    }
}
public class EntityEvent_3_02 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 2;
    public override string OptionName { get; protected set; } = "拥抱";
    public override string OptionDescription { get; protected set; } = "应对成功时，若双方“欲望”差值<=4，获得胜利";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsUseDiceCombo { get; protected set; } = true;
    public override E_ComboType ComboType { get; protected set; } = E_ComboType.Quadruple;
    public override bool IsDiceConditionsHave { get; protected set; }=false;

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
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToGravity);
                this.IsVisible = false;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
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
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToGravity);
    }
}
public class EntityEvent_3_03 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 3;
    public override string OptionName { get; protected set; } = "弹回";//毛球_1
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsUseDiceCombo { get; protected set; } = true;
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

            if (optionContext is EntityEvent_3_01_OptionContext ctx0)
            {
                if(ctx0.index>5||ctx0.index<1)
                {
                    result = false;
                    Debug.Log("ctx0.index>5||ctx0.index<1");
                }
                else
                {
                    if(ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空。");
                    }
                }
                
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToFurBall1);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null)
                    {
                        ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
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

    public void NeverRespondToFurBall1(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(1);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToFurBall1);
    }
    
}
public class EntityEvent_3_04 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 4;
    public override string OptionName { get; protected set; } = "弹回";//毛球_2
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsUseDiceCombo { get; protected set; } = true;
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

            if (optionContext is EntityEvent_3_01_OptionContext ctx0)
            {
                if(ctx0.index>5||ctx0.index<1)
                {
                    result = false;
                    Debug.Log("ctx0.index>5||ctx0.index<1");
                }
                else
                {
                    if(ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空。");
                    }
                }
                
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToFurBall2);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null)
                    {
                        ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
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

    public void NeverRespondToFurBall2(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(1);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToFurBall2);
    }
    
}
public class EntityEvent_3_05 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 5;
    public override string OptionName { get; protected set; } = "弹回";//毛球_3
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsUseDiceCombo { get; protected set; } = true;
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

            if (optionContext is EntityEvent_3_01_OptionContext ctx0)
            {
                if(ctx0.index>5||ctx0.index<1)
                {
                    result = false;
                    Debug.Log("ctx0.index>5||ctx0.index<1");
                }
                else
                {
                    if(ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空。");
                    }
                }
                
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToFurBall3);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null)
                    {
                        ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
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

    public void NeverRespondToFurBall3(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(1);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToFurBall3);
    }
    
}
public class EntityEvent_3_06 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 6;
    public override string OptionName { get; protected set; } = "弹回";//毛球_4
    public override string OptionDescription { get; protected set; } = "应对成功时选择对象的一个部位造成1点伤害";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level3_Option;

    public override bool IsUseDiceCombo { get; protected set; } = true;
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

            if (optionContext is EntityEvent_3_01_OptionContext ctx0)
            {
                if(ctx0.index>5||ctx0.index<1)
                {
                    result = false;
                    Debug.Log("ctx0.index>5||ctx0.index<1");
                }
                else
                {
                    if(ProgressManager.Instance.nowEntities[ctx0.index] == null)
                    {
                        result = false;
                        Debug.Log("这个部位为空。");
                    }
                }
                
            }
            if (result)
            {
                EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToFurBall4);
                if (optionContext is EntityEvent_3_01_OptionContext ctx)
                {
                    if (ProgressManager.Instance.nowEntities[ctx.index] != null)
                    {
                        ProgressManager.Instance.nowEntities[ctx.index].BeAttacked(1);
                    }
                }
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

    public void NeverRespondToFurBall4(object info = null)
    {
        ProgressManager.Instance.player.BeAttacked(1);
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted,NeverRespondToFurBall4);
    }
    
}
