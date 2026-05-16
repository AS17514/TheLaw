using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class OptionBase
{
    #region 属性定义区
    // 外部可读，但仅允许子类在内部赋值
    public virtual int LevelID { get; protected set; }//我新增的，想着说用来标记怪物的选项，然后它到底是属于第几关的怪物
    public virtual int OptionID { get; protected set; }
    public virtual string OptionName { get; protected set; }
    public virtual string OptionDescription { get; protected set; }
    public virtual E_OptionType OptionType { get; protected set; }
    //public virtual bool IsVisible { get;  set; }=false;
    public virtual bool IsDiceConditionsHave{ get; protected set; }=true;
    public virtual E_ComboType ComboType { get; protected set; }
    public virtual bool IsUseDiceCombo{ get; protected set; }=false;

    public virtual DiceCondition[] DiceCost { get; protected set; } 
        

    public virtual bool IsSpecialConditionsHave{ get; protected set; }=false;

    public virtual E_SpecialOptionConditions specialConditions{ get; protected set; }
    public virtual int fatherID { get; protected set; }
    public virtual int sonID { get; protected set; }
    public virtual bool IsResponseOption { get; } = false;//专门用来进行应对选项有关的判断。
    // 新增：该选项绑定的新手引导类型（默认为 None，表示没有引导）
    public virtual E_TutorialType BindTutorial { get; } = E_TutorialType.None;
    #endregion

    #region 震撼亚洲的新框架的代码部分喵

    private bool _isVisible = false;
    public virtual bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            if (value && GetNeverRespondHandler() != null)
                EventManager.Instance.RegisterPendingResponse(this);
            else if (!value)
                EventManager.Instance.UnregisterPendingResponse(this);
        }
    }

    public virtual UnityAction<object> GetNeverRespondHandler() => null;  // 应对型 Option 重写返回 NeverRespond

    
    public OptionBase()
    {
        ExecuteLogic = ExecuteLogicImpl;
    }

    protected virtual void ExecuteLogicImpl(object info=null){}
    #endregion
    // --- 委托定义区 ---
    // 用 Action 储存无返回值的方法。如果需要传参，可以用 Action<T>
    public virtual Action<object> ExecuteLogic { get; protected set; }

    // --- 通用方法 ---
    // 外部（比如UI按钮点击后）统一调用这个方法
    // LastTriggerSuccess 记录上次调用是否成功执行（供 EventManager 读取）
    public bool LastTriggerSuccess { get; protected set; }
    public virtual void TriggerOption(OptionContext optionContext=null)
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
            if(result)
                ExecuteLogic?.Invoke(optionContext);
            else
            {
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
            LastTriggerSuccess = result;
        }
        else
        {
            LastTriggerSuccess = false;
        }
    }
}
