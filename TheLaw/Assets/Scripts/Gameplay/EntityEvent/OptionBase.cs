using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OptionBase
{
    #region 属性定义区
    // 外部可读，但仅允许子类在内部赋值
    public virtual int OptionID { get; protected set; }
    public virtual string OptionName { get; protected set; }
    public virtual bool IsVisible { get; protected set; }
    public virtual bool IsDiceConditionsHave
    {
        get
        {
            return true;
        }
        protected set
        {
            IsSpecialConditionsHave=true;
        }
    }
    public E_ComboType ComboType { get; protected set; }
    public virtual bool IsUseDiceCombo
    {
        get
        {
            return false;
        }
        protected set
        {
            IsSpecialConditionsHave=false;
        }
    }
    public virtual DiceCondition DiceCost { get; protected set; }

    public virtual bool IsSpecialConditionsHave
    {
        get
        {
            return false;
        }
        protected set
        {
            IsSpecialConditionsHave=false;
        }
    }
    public virtual E_SpecialOptionConditions specialConditions{ get; protected set; }
    public virtual int fatherID { get; protected set; }
    public virtual int sonID { get; protected set; }
    #endregion
    // --- 委托定义区 ---
    // 用 Action 储存无返回值的方法。如果需要传参，可以用 Action<T>
    public virtual Action ExecuteLogic { get; protected set; }

    // --- 通用方法 ---
    // 外部（比如UI按钮点击后）统一调用这个方法
    public virtual void TriggerOption()
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
            if(result)
                ExecuteLogic?.Invoke();
            else
            {
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
        }
    }
}
