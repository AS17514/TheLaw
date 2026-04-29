using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity1 : Entity
{
    void Start() 
    {
        #region 状态管理器
        
        // 第一关怪物清空旧数据（或者在关卡管理器里清空）
        StateManager.Instance.ClearStates();

        // 把当前怪物的所有状态注册进去
        StateManager.Instance.RegisterStateData(
            E_StateType_1.normal, 
            new Action[] { normal_Action_Atk}, 
            new Action[] { }
        );
        
        // 默认进入初始状态
        StateManager.Instance.ChangeState(E_StateType_1.normal);
        
        #endregion

        #region Buff初始化

        AddBuff(E_BuffType.Desire,1);

        #endregion
    }

    #region 行动

    private void normal_Action_Atk()
    {
        int temp = this.buffs[E_BuffType.Desire];
        for (int i = 0; i < temp; i++)
        {
            DiceManager.Instance.AddEntityDice();
        }
        EventManager.Instance.optionPool[E_OptionType.Level1_Option][0].IsVisible=true;
    }
    #endregion
}

