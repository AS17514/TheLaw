using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDice : DiceBase
{
    public TimeDice()
    {
        this.type = E_DiceType.Time1;
        this.sides = 4;
        this.value = 1;
    }

    public void ChangeTimeDiceValue(int change )
    {
        if (change == 0)
        {return;}
        int newValue=this.value+change;
        this.value=Mathf.Clamp(newValue, 1, 4);
        switch (this.value)
        {
            case 1:
                this.type=E_DiceType.Time1;
                break;
            case 2:
                this.type=E_DiceType.Time2;
                break;
            case 3:
                this.type=E_DiceType.Time3;
                break;
            case 4:
                this.type=E_DiceType.Time4;
                break;
        }
    }
    public override void Roll()
    {
        this.value = Random.Range(1, 5);//右开区间
    }
}
