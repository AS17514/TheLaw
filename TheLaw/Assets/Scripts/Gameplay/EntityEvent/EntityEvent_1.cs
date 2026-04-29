using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum E_EntityEvent_1
{
    CopeWith_Dodge,
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
            ProgressManager.Instance.player.BeAttacked(hit*5);
        }
        DiceManager.Instance.ConsumeValidSelectedDice();
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_SelectedDice);
        
    }
}
