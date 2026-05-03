using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity1 : Entity
{
    public override void InitEntity(int initialDesire = 0, int maxHp = 10)
    {
        AddBuff(E_BuffType.Desire, initialDesire);
        this.maxHp = maxHp;
        this.hp = maxHp;
    }
    public override void ManualInit()
    {
        base.ManualInit(); // 必须先调用父类，把自己注册进 BuffManager

        #region 状态管理器
        StateManager.Instance.ClearStates();

        StateManager.Instance.RegisterStateData(
            E_StateType_1.normal,
            new ActionNode[] { new ActionNode(E_IntentType.Entity1_Atk, normal_Action_Atk) },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity1_FilledWithFood,normal_Desire_FilledWithFood),
                new DesireNode(E_DesireType.Entity1_Urgent,normal_Desire_Urgent)
            }
        );
        StateManager.Instance.RegisterStateData(
            E_StateType_1.exhausted,
            new ActionNode[] { new ActionNode(E_IntentType.Entity1_Eat, exhausted_Action_Eat) },
            new DesireNode[]
            {
                new DesireNode(E_DesireType.Entity1_Feed,exhausted_Desire_Feed)
            }
        );

        StateManager.Instance.ChangeState(E_StateType_1.normal);
        #endregion

        // 最后进行数值初始化
        InitEntity(1, 25);
    }


    #region 行动

    public void normal_Action_Atk()
    {
        int temp = this.buffs[E_BuffType.Desire];
        for (int i = 0; i < temp; i++)
        {
            DiceManager.Instance.AddEntityDice();
        }
        EventManager.Instance.optionPool[E_OptionType.Level1_Option][0].IsVisible = true;
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
    }

    public void exhausted_Action_Eat()
    {
        this.hp = Math.Clamp(this.hp + 4, 0, maxHp);

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
                        //怪物切换状态的话，取消诱导选项的可选。
                        #region Induce
                        if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][2] != null &&
                            EventManager.Instance.optionPool[E_OptionType.Level1_Option][2] is EntityEvent_1_03 e3)
                        {
                            if (e3.IsVisible == true)
                            {
                                e3.IsVisible = false;
                                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                                isEntity_1_03HadFound = true;
                            }
                        }
                        #endregion
                        #region Think

                        if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][5] != null &&
                            EventManager.Instance.optionPool[E_OptionType.Level1_Option][5] is EntityEvent_1_06 e6)
                        {
                            e6.IsVisible = false;
                        }
                        #endregion

                        #region Snatch

                        if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][6] != null &&
                            EventManager.Instance.optionPool[E_OptionType.Level1_Option][6] is EntityEvent_1_07 e7)
                        {
                            e7.IsVisible = false;
                        }
                        #endregion
                    }
                }
            }
        }

        #endregion
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP, hp);
    }
    public bool isEntity_1_03HadFound = false;
    #endregion

    #region 欲望

    public void normal_Desire_FilledWithFood()
    {
        AddBuff(E_BuffType.Desire, 1);
        isDesire_UrgentUse = false;//回正
    }

    public void normal_Desire_Urgent()
    {
        isDesire_UrgentUse = true;
        ProgressManager.Instance.SetCurrentTimeProgress(3);

        if (isEntity_1_03HadFound)//怪物重新许下愿望的话，恢复选项的可选。
        {
            #region Induce
            if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][2] != null &&
                EventManager.Instance.optionPool[E_OptionType.Level1_Option][2] is EntityEvent_1_03 e3)
            {
                e3.IsVisible = true;
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
            }
            #endregion

            #region Think
            if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][5] != null &&
                EventManager.Instance.optionPool[E_OptionType.Level1_Option][5] is EntityEvent_1_06 e6)
            {
                if (e6.isThinkHadUse == false)
                {
                    e6.IsVisible = true;
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                }
            }
            #endregion

            #region Snatch
            if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][5] != null &&
                EventManager.Instance.optionPool[E_OptionType.Level1_Option][5] is EntityEvent_1_06 e6_2)
            {
                if (e6_2.isSnatchHadFound)
                {
                    if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][6] != null &&
                        EventManager.Instance.optionPool[E_OptionType.Level1_Option][6] is EntityEvent_1_07 e7)
                    {
                        e7.IsVisible = true;
                        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                    }
                }
            }
            #endregion

        }

    }
    public bool isDesire_UrgentUse = false;//用于诱导选项的标记是否已解锁

    public void exhausted_Desire_Feed()
    {
        isDesire_FeedUse = true;
        ProgressManager.Instance.SetCurrentTimeProgress(2);
    }
    public bool isDesire_FeedUse = false;
    #endregion
}

