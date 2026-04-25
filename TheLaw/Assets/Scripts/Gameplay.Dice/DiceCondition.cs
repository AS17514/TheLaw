using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DiceCondition
{
    public E_DiceType type;
    public int value;
    public E_CompareType mode;

    public DiceCondition(E_DiceType type,int value,E_CompareType mode)
    {
        this.type = type;
        this.value = value;
        this.mode = mode;
    }
}
