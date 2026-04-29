using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDice : DiceBase
{
    public EntityDice()
    {
        this.type = E_DiceType.EntityDice;
        this.sides = 6;
        Roll();
    }
    public override void Roll()
    {
        this.value = Random.Range(1, 7);//右开区间
    }

}
