using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum E_EntityEvent_1
{
    CopeWith_Dodge,
    Communicate,
    Induce,
    Observe1,
    Observe2,
    Think,
    Snatch,
    Eat,
}
public class EntityEvent_1_01 : OptionBase
{
    #region OptionBase属性

    public override bool IsResponseOption { get; } = true;
    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "闪避";
    public override string OptionDescription { get; protected set; } = "行动>=投掷的单个骰子的点数,所有骰子可以单独应对，每个未成功应对的骰子将对自己造成4点伤害）";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level1_Option;

    #endregion

    #region 本身属性
    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.CopeWith_Dodge;


    #endregion

    public override void TriggerOption(OptionContext optionContext = null)
    {

        Debug.Log("闪避");

        bool result = true;
        DiceManager.Instance.SortSelectedByValue();
        DiceManager.Instance.SortEntityPoolByValue();
        foreach (var dice in DiceManager.Instance.selectedDice)
        {
            if (dice.type != E_DiceType.Action && dice.type != E_DiceType.Wild)
            {
                //DiceManager.Instance.ClearEntityPool();
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
                result = false;
                // 一旦清空了集合，必须立即跳出 foreach 循环
                break;
            }
            else if (dice.type == E_DiceType.Wild)
            {
                continue;
            }
            else
            {
                foreach (var entityDice in DiceManager.Instance.entityDicePool)
                {
                    if (!dice.isValid && !entityDice.isValid)
                    {
                        if (dice.value >= entityDice.value)
                        {
                            dice.isValid = true;
                            entityDice.isValid = true;
                        }
                    }
                }
            }
        }

        foreach (var dice in DiceManager.Instance.selectedDice)
        {
            if (dice.type == E_DiceType.Wild)
            {
                foreach (var entityDice in DiceManager.Instance.entityDicePool)
                {
                    if (!dice.isValid && !entityDice.isValid)
                    {

                        dice.isValid = true;
                        entityDice.isValid = true;
                        // 这个 break; 只会跳出内层循环
                        break;
                    }
                }
            }
        }

        if (result)
        {
            DiceManager.Instance.ConsumeValidEntityDice();
            if (DiceManager.Instance.entityDicePool != null && DiceManager.Instance.entityDicePool.Count > 0)
            {
                int hit = DiceManager.Instance.entityDicePool.Count;
                int tempAtk = 4;
                if (ProgressManager.Instance.nowEntities[1] != null)
                {
                    if (ProgressManager.Instance.nowEntities[1] is Part1_1 part1)
                    {
                        if (part1.isDestroyed == true)
                        {
                            tempAtk--;
                        }
                    }
                }

                if (ProgressManager.Instance.nowEntities[2] != null)
                {
                    if (ProgressManager.Instance.nowEntities[2] is Part1_2 part2)
                    {
                        if (part2.isDestroyed == true)
                        {
                            tempAtk--;
                        }
                    }
                }

                ProgressManager.Instance.player.BeAttacked(hit * tempAtk);
            }

            DiceManager.Instance.ConsumeValidSelectedDice(true);
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice);

            // 1. 伤害结算完后，清空怪物剩余的未格挡骰子
            DiceManager.Instance.ClearEntityPool();
            // 2. 既然玩家已经闪避应对过了，必须注销未响应的惩罚机制，防止二次挨打
            EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);

            //应对完之后取消该选项的显示。
            this.IsVisible = false;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
        }
    }

    public void NeverRespond(object info = null)
    {
        Debug.Log("没闪避");

        int hit = DiceManager.Instance.entityDicePool.Count;
        int tempAtk = 4;
        ProgressManager.Instance.player.BeAttacked(hit * tempAtk);
        //之后取消该选项的显示。
        this.IsVisible = false;

        DiceManager.Instance.ClearEntityPool();

        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);

        EventCenter.Instance.RemoveEventListener(E_EventType.Logic_PlayerActionExecuted, NeverRespond);

    }
}



public class EntityEvent_1_02 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 2;
    public override string OptionName { get; protected set; } = "交流";

    public override string OptionDescription { get; protected set; } =
        "???";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level1_Option;
    public override bool IsVisible { get; set; } = true;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action,1,E_CompareType.Any)
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.Communicate;
    public int communicationCount = 0;

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
                if (ProgressManager.Instance.nowEntities[0] != null &&
                    ProgressManager.Instance.nowEntities[0] is Entity1 entity1)
                {
                    if (entity1.isDesire_FeedUse)
                    {
                        if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][2] != null &&
                            EventManager.Instance.optionPool[E_OptionType.Level1_Option][2] is EntityEvent_1_03 e3)
                        {
                            e3.IsVisible = true;
                            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);
                            DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
                        }
                    }
                    else if (communicationCount == 0)
                    {
                        communicationCount++;
                        DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
                    }
                    else if (communicationCount >= 1) //第二次及以后执行
                    {
                        DiceManager.Instance.AddDice(E_DiceType.Mind); //投掷并获得一个思维骰子
                        DiceManager.Instance.ConsumeValidSelectedDice(); //消耗骰子
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
}

public class EntityEvent_1_03 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 3;
    public override string OptionName { get; protected set; } = "诱导";

    public override string OptionDescription { get; protected set; } =
        "对象将“吃掉”自己，你将获得胜利";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level1_Option;
    //public override bool IsVisible { get; set; } = true;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action,1,E_CompareType.Any),
                new DiceCondition(E_DiceType.Mind,1,E_CompareType.Equal),
                new DiceCondition(E_DiceType.Mind,2,E_CompareType.Equal),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.Induce;

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
                if (ProgressManager.Instance.nowEntities[0] != null &&
                    ProgressManager.Instance.nowEntities[0] is Entity1 entity1)
                {
                    if (entity1.isDesire_FeedUse)
                    {
                        entity1.hp = 0;
                        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityHP);
                        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityDied);
                    }
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

public class EntityEvent_1_04 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 4;
    public override string OptionName { get; protected set; } = "观察";

    public override string OptionDescription { get; protected set; } =
        "观察";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level1_Option;
    public override bool IsVisible { get; set; } = true;

    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 2, E_CompareType.Greater),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.Observe1;

    #endregion

    public override void TriggerOption(OptionContext optionContext = null)
    {

        Debug.Log("观察执行");

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
                Part1_1.PartApear();
                IsVisible = false;
                if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][4] != null &&
                    EventManager.Instance.optionPool[E_OptionType.Level1_Option][4] is EntityEvent_1_05 e5)
                {
                    e5.IsVisible = true;
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);//更新事件列表
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
public class EntityEvent_1_05 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 5;
    public override string OptionName { get; protected set; } = "观察";

    public override string OptionDescription { get; protected set; } =
    "观察";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level1_Option;
    // public override bool IsVisible { get; set; } = true;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 3, E_CompareType.Greater),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.Observe2;

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
                Part1_2.PartApear();
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

public class EntityEvent_1_06 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 6;
    public override string OptionName { get; protected set; } = "思考";

    public override string OptionDescription { get; protected set; } = "思考";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level1_Option;
    // public override bool IsVisible { get; set; } = true;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Mind, 4, E_CompareType.Less),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.Think;
    public bool isThinkHadUse = false;
    public bool isSnatchHadFound = false;
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
                IsVisible = false;
                isThinkHadUse = true;
                isSnatchHadFound = true;
                if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][6] != null &&
                    EventManager.Instance.optionPool[E_OptionType.Level1_Option][6] is EntityEvent_1_07 e7)
                {
                    e7.IsVisible = true;
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);//更新事件列表
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

public class EntityEvent_1_07 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 7;
    public override string OptionName { get; protected set; } = "抢夺";

    public override string OptionDescription { get; protected set; } = "抢夺";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level1_Option;
    // public override bool IsVisible { get; set; } = true;
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

    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.Snatch;

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
                DiceManager.Instance.AddDice(E_DiceType.Action);
                DiceManager.Instance.AddDice(E_DiceType.Action);//投掷并获得二个行动骰子
                ProgressManager.Instance.AddTimeProgress(1);//对象时间进度+1
                if (EventManager.Instance.optionPool[E_OptionType.Level1_Option][7] != null &&
                    EventManager.Instance.optionPool[E_OptionType.Level1_Option][7] is EntityEvent_1_08 e8)
                {
                    e8.IsVisible = true;
                    EventCenter.Instance.EventTrigger(E_EventType.UI_Update_Events);//更新事件列表
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

public class EntityEvent_1_08 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 8;
    public override string OptionName { get; protected set; } = "吃";

    public override string OptionDescription { get; protected set; } = "吃";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level1_Option;
    // public override bool IsVisible { get; set; } = true;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 1, E_CompareType.Greater),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.Eat;

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
                //自己恢复3点生命
                ProgressManager.Instance.player.hp = Math.Clamp(ProgressManager.Instance.player.hp + 3,
                    ProgressManager.Instance.player.hp, ProgressManager.Instance.player.maxHp);
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_PlayerHP, ProgressManager.Instance.player.hp);
                IsVisible = false;
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