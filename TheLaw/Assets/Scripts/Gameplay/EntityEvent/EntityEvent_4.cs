using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum E_EntityEvent_4
{
    contentment,         // 01 知足
    exchange,            // 02 交流
    echo,                // 03 回声
    reverberate,         // 04 回响
    observe,             // 05 观察
    door1,               // 06 1号门
    door2,               // 07 2号门
    door3,               // 08 3号门
    exit,                // 09 出口
    decideToLeave,       // 10 决定离开
    leaveThen,           // 11 那么离开吧
    notTired,            // 12 不疲倦吗？
    trappedByYourself,   // 13 困住你的是你自己
}

public class EntityEvent_4_01 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "知足";
    public override string OptionDescription { get; protected set; } = "";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 2, E_CompareType.Equal),
                new DiceCondition(E_DiceType.Mind, 4, E_CompareType.Equal),
            };
        }
    }

    public override UnityAction<object> GetNeverRespondHandler() => NeverRespond;
    
    
    #endregion

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.contentment;


    #endregion
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        EventCenter.Instance.RemoveEventListener(
            E_EventType.Logic_PlayerActionExecuted, NeverRespond);
        IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }
    
    public void NeverRespond(object info = null)
    {
        if(ProgressManager.Instance.nowEntities[0] is Entity entity)
        {
            int length = 0;
            length += entity.GetBuff(E_BuffType.Want0) > 0 ? 0 : 1;
            length += entity.GetBuff(E_BuffType.Want1) > 0 ? 0 : 1;
            length += entity.GetBuff(E_BuffType.Want2) > 0 ? 0 : 1;
            length += entity.GetBuff(E_BuffType.Want3) > 0 ? 0 : 1;
            ProgressManager.Instance.player.BeAttacked(length);
        }
        this.IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);
    }
}
public class EntityEvent_4_02 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 2;
    public override string OptionName { get; protected set; } = "交流";
    public override string OptionDescription { get; protected set; } = "";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;

    public override int sonID { get; protected set; } = 3;

    public override bool IsVisible { get; set; } = true;

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
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.exchange;


    #endregion
    
}

public class EntityEvent_4_03OptionContext : OptionBase
{
    public int index;//1<=index<=4
}
public class EntityEvent_4_03 : OptionBase
{
    #region OptionBase属性
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 3;
    public override string OptionName { get; protected set; } = "回声";
    public override string OptionDescription { get; protected set; } = "选择一个部位，清除对象对应部位数字的“渴望”效果";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
    
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 1, E_CompareType.Equal),
                new DiceCondition(E_DiceType.Action, 3, E_CompareType.Equal),
                new DiceCondition(E_DiceType.Action, 5, E_CompareType.Equal),
            };
        }
    }

    
    
    
    #endregion
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        if(info is EntityEvent_4_03OptionContext ctx)
        {
            switch (ctx.index)
            {
                case 1:
                    ProgressManager.Instance.TryGetEntity().RemoveBuff(E_BuffType.Want0);
                    break;
                case 2:
                    ProgressManager.Instance.TryGetEntity().RemoveBuff(E_BuffType.Want1);
                    break;
                case 3:
                    ProgressManager.Instance.TryGetEntity().RemoveBuff(E_BuffType.Want2);
                    break;
                case 4:
                    ProgressManager.Instance.TryGetEntity().RemoveBuff(E_BuffType.Want3);
                    break;
            }
        }
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.echo;


    #endregion
    
}
public class EntityEvent_4_04 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 4;
    public override string OptionName { get; protected set; } = "回响";
    public override string OptionDescription { get; protected set; } = "对象“欲望”-1";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
    
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 2, E_CompareType.Equal),
                new DiceCondition(E_DiceType.Mind, 4, E_CompareType.Equal),
                new DiceCondition(E_DiceType.Mind, 6, E_CompareType.Equal),
            };
        }
    }

    
    
    
    #endregion
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        ProgressManager.Instance.TryGetEntity().AddBuff(E_BuffType.Desire,-1);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.reverberate;


    #endregion
    
}
public class EntityEvent_4_05 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 5;
    public override string OptionName { get; protected set; } = "观察";
    public override string OptionDescription { get; protected set; } = "房间，三扇奇怪的和背后的出口";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
    public override bool IsVisible { get; set; } = true;
    public override int sonID { get; protected set; } = 3;
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
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        EventManager.Instance.UnLockOption(E_OptionType.Level4_Option, 6);//2号门
        EventManager.Instance.UnLockOption(E_OptionType.Level4_Option, 7);//3号门
        EventManager.Instance.UnLockOption(E_OptionType.Level4_Option, 8);//出口
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.observe;


    #endregion
    
}
public class EntityEvent_4_06 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 6;
    public override string OptionName { get; protected set; } = "1号门";
    public override string OptionDescription { get; protected set; } = "";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 1, E_CompareType.Equal),
                
            };
        }
    }

    
    
    
    #endregion
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        Part4_1.PartApear();
        IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.door1;


    #endregion
    
}
public class EntityEvent_4_07 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 7;
    public override string OptionName { get; protected set; } = "2号门";
    public override string OptionDescription { get; protected set; } = "";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 3, E_CompareType.Equal),
                
            };
        }
    }

    
    
    
    #endregion
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        Part4_1.PartApear();
        IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.door2;


    #endregion
    
}
public class EntityEvent_4_08 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 8;
    public override string OptionName { get; protected set; } = "3号门";
    public override string OptionDescription { get; protected set; } = "";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 5, E_CompareType.Equal),
                
            };
        }
    }

    
    
    
    #endregion
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        Part4_3.PartApear();
        IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.door3;


    #endregion
    
}
public class EntityEvent_4_09 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 9;
    public override string OptionName { get; protected set; } = "出口";
    public override string OptionDescription { get; protected set; } = "";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
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
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.exit;


    #endregion
    
}
public class EntityEvent_4_10 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 10;
    public override string OptionName { get; protected set; } = "决定离开，所以拿出了钥匙";
    public override string OptionDescription { get; protected set; } = "";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
    public override int sonID { get; protected set; } = 11;

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
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        IsVisible = false;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.decideToLeave;


    #endregion
    
}
public class EntityEvent_4_11 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 11;
    public override string OptionName { get; protected set; } = "那么离开吧";
    public override string OptionDescription { get; protected set; } = "获得胜利";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
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
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.leaveThen;


    #endregion
    
}
public class EntityEvent_4_12 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 12;
    public override string OptionName { get; protected set; } = "不疲倦吗？";
    public override string OptionDescription { get; protected set; } = "（回应是沉默）";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 0, E_CompareType.Any),
                new DiceCondition(E_DiceType.Mind, 0, E_CompareType.Any),
                
            };
        }
    }

    
    
    
    #endregion
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        if (ProgressManager.Instance.TryGetEntity().GetBuff(E_BuffType.Desire) == 0)
            EventManager.Instance.UnLockOption(E_OptionType.Level4_Option,12);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.notTired;


    #endregion
    
}
public class EntityEvent_4_13 : OptionBase
{
    #region OptionBase属性
    
    public override int LevelID { get; protected set; } = 4;
    public override int OptionID { get; protected set; } = 13;
    public override string OptionName { get; protected set; } = "困住你的是你自己，你可以离开";
    public override string OptionDescription { get; protected set; } = "获得胜利";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level4_Option;
    public override bool IsDiceConditionsHave { get; protected set; }=false;

    #endregion
    
    protected override void ExecuteLogicImpl(object info=null)
    {
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        DiceManager.Instance.ConsumeValidSelectedDice();
    }

    #region 本身属性
    E_EntityEvent_4 entityEvent_4Type = E_EntityEvent_4.trappedByYourself;


    #endregion
    
}