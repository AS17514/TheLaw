using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part4_1 : Part
{
    public void InitPart()
    {
        partName = "1号门";
        id = 1;
        maxHp = 9;
        hp = maxHp;
    }

    public void Awake()
    {
        InitPart();
    }
    
    
    public override void BeAttacked(int atk)
    {
        TakeDamage( atk- ProgressManager.Instance.TryGetEntity().GetDamageReduction());
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
    }

    public override void Die()
    {
        ProgressManager.Instance.PartStateChange(1,false);
        ProgressManager.Instance.TryGetEntity().AddBuff(E_BuffType.Desire,-1);
        base.Die();
    }

    /// <summary>
    /// 静态方法，外部需要生成部位的时候调用。
    /// </summary>
    public static void PartApear()=> Part.SpawnPart<Part4_1>(1, "Part4_1");
}
