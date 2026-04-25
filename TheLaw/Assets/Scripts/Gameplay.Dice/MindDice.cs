using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MindDice : DiceBase
{
    public MindDice()
    {
        this.type = E_DiceType.Mind;
        this.sides = 6;
        Roll();
    }
    public override void Roll()
    {
        this.value = Random.Range(1, 7);//右开区间
    }
}
