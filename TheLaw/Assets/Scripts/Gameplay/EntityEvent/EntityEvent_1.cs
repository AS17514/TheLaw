using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum E_EntityEvent_1
{
    CopeWith_Dodge,
    Communicate,
    Induce,
    Observe,
    AtkEntiyi,
    Think,
    Snatch,
    Eat,
}
public class EntityEvent_1_01:OptionBase
{
    #region OptionBase属性
    public override int LevelID { get; protected set; } = 1;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "闪避";
    public override string OptionDescription { get; protected set; }="行动>=投掷的单个骰子的点数,所有骰子可以单独应对，每个未成功应对的骰子将对自己造成4点伤害）";
    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level1_Option;
    #endregion

    #region 本身属性
    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.CopeWith_Dodge;
    
 
    #endregion

    public override void TriggerOption(OptionContext optionContext = null)
    {
        bool result = false;
        DiceManager.Instance.SortSelectedByValue();
        DiceManager.Instance.SortEntityPoolByValue();
        foreach (var dice in DiceManager.Instance.selectedDice)
        {
            if (dice.type != E_DiceType.Action)
            {
                DiceManager.Instance.ClearEntityPool();
                DiceManager.Instance.ClearSelected();
                EventCenter.Instance.EventTrigger(E_EventType.UI_Update_IsConditionNotMet);
            }
            else
            {
                foreach (var entityDice in DiceManager.Instance.entityDicePool)
                {
                    if(!dice.isValid&&!entityDice.isValid)
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

        DiceManager.Instance.ConsumeValidEntityDice();
        if (DiceManager.Instance.entityDicePool!=null||DiceManager.Instance.entityDicePool.Count>0)
        {
            int hit = DiceManager.Instance.entityDicePool.Count;
            int tempAtk = 5;
            if (ProgressManager.Instance.nowEntities[1]!=null)
            {
                if (ProgressManager.Instance.nowEntities[1] is Part1_1 part1)
                {
                    if (part1.isDestroyed = true)
                    {
                        tempAtk--;
                    }
                }
            }
            if (ProgressManager.Instance.nowEntities[2]!=null)
            {
                if (ProgressManager.Instance.nowEntities[2] is Part1_2 part2)
                {
                    if (part2.isDestroyed = true)
                    {
                        tempAtk--;
                    }
                }
            }
            ProgressManager.Instance.player.BeAttacked(hit*tempAtk);
        }
        DiceManager.Instance.ConsumeValidSelectedDice();
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice);
        
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

    public override  void TriggerOption(OptionContext optionContext = null)
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
                            e3.IsVisible=true;
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
    public override bool IsVisible { get; set; } = true;
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
                        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntiyiDied);
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
