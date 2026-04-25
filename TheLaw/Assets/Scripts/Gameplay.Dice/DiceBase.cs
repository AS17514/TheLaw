using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DiceBase
{
    public E_DiceType type;
    public int sides;
    public int value;
    public int index;
    public int isValid;
    public abstract void Roll();
}
