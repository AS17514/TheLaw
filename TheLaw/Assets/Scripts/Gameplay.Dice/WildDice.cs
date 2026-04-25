using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildDice : DiceBase
{
    public WildDice()
    {
        this.type = E_DiceType.Wild;
        this.sides = 0;
        Roll();//无意义
    }
    public override void Roll()
    {
        
    }
    public void Renew()
    {
        this.type = E_DiceType.Wild;
        this.sides = 0;
    }
}
