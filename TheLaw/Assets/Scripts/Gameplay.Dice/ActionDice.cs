using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionDice : DiceBase
{
    public ActionDice()
    {
        this.type = E_DiceType.Action;
        this.sides = 6;
        Roll();
    }
    public override void Roll()
    {
        this.value = Random.Range(1, 7);//右开区间
    }
}
