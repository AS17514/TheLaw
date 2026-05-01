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
            new Action[] { normal_Desire_FilledWithFood,normal_Desire_Urgent}
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_1.exhausted, 
            new Action[] { exhausted_Action_Eat}, 
            new Action[] { exhausted_Desire_Feed}
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

        #region 欲望效果

        if (isDesire_FeedUse)
        {
            AddBuff(E_BuffType.Desire, -1);
            if (ProgressManager.Instance.nowEntities[1] != null &&
                ProgressManager.Instance.nowEntities[1] is Part1_1 part1)
            {
                if (part1.isDestroyed)
                {
                    part1.isDestroyed = false;
                    part1.hp = Math.Clamp(this.hp + 5, 0, maxHp);
                    this.hp = Math.Clamp(this.hp + 5, 0, maxHp);
                }
                else if (ProgressManager.Instance.nowEntities[2] != null &&
                         ProgressManager.Instance.nowEntities[2] is Part1_2 part2)
                {
                    if (part2.isDestroyed)
                    {
                        part2.isDestroyed = false;
                        part2.hp = Math.Clamp(this.hp + 5, 0, maxHp);
                        this.hp = Math.Clamp(this.hp + 5, 0, maxHp);
                    }
                    else if (!part2.isDestroyed)
                    {
                        isDesire_FeedUse = true;//回正
                        StateManager.Instance.ChangeState(E_StateType_1.normal);
                        ProgressManager.Instance.SetCurrentTimeProgress(5);
                    }
                }
            }
        }

        #endregion
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP,hp);
    }
    #endregion

    #region 欲望

    public void normal_Desire_FilledWithFood()
    {
        AddBuff(E_BuffType.Desire,1);
        isDesire_UrgentUse=false;//回正
    }

    public void normal_Desire_Urgent()
    {
        isDesire_UrgentUse=true;
        ProgressManager.Instance.SetCurrentTimeProgress(3);
    }
    public bool isDesire_UrgentUse=false;

    public void exhausted_Desire_Feed()
    {
        isDesire_FeedUse=true;
        ProgressManager.Instance.SetCurrentTimeProgress(2);
    }
    public bool isDesire_FeedUse=false;
    #endregion
}

