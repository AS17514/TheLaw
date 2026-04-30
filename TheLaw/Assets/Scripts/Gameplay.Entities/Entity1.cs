using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity1 : Entity
{
    public override void InitEntity(int initialDesire = 0, int maxHp = 10)
    {
        AddBuff(E_BuffType.Desire,initialDesire);
        this.maxHp = maxHp;
        this.hp = maxHp;
    }
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
        StateManager.Instance.RegisterStateData(
            E_StateType_1.exhausted, 
            new Action[] { exhausted_Action_Eat}, 
            new Action[] { }
        );
        
        // 默认进入初始状态
        StateManager.Instance.ChangeState(E_StateType_1.normal);
        
        #endregion

        InitEntity(1,25);
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

    public void exhausted_Action_Eat()
    {
        this.hp=Math.Clamp(this.hp+4,0,maxHp);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP,hp);
    }
    #endregion

    #region 欲望

    public void normal_Desire_FilledWithFood()
    {
        AddBuff(E_BuffType.Desire,1);
    }

    public void normal_Desire_Urgent()
    {
        isDesire_UrgentUse=true;
        ProgressManager.Instance.SetCurrentTimeProgress(3);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_MaxTimeProgress,3);
    }
    public bool isDesire_UrgentUse=false;

    #endregion
}

