using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EntityEvent_2
{
    Escape,
}

public class EntityEvent_2_01 : OptionBase
{
    #region OptionBase属性

    public override int LevelID { get; protected set; } = 2;
    public override int OptionID { get; protected set; } = 1;
    public override string OptionName { get; protected set; } = "逃避";

    public override string OptionDescription { get; protected set; } = "逃避";

    public override E_OptionType OptionType { get; protected set; } = E_OptionType.Level2_Option;
    // public override bool IsVisible { get; set; } = true;
    public override DiceCondition[] DiceCost
    {
        get
        {
            return new DiceCondition[]
            {
                new DiceCondition(E_DiceType.Action, 3, E_CompareType.Less),
                new DiceCondition(E_DiceType.Action, 4, E_CompareType.Less),
            };
        }
    }

    #endregion

    #region 本身属性

    E_EntityEvent_1 entityEvent_1Type = E_EntityEvent_1.Eat;

    #endregion

}
