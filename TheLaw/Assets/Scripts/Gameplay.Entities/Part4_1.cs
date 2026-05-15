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
        ProgressManager.Instance.TryGetEntity().AddBuff(E_BuffType.Desire,-1);
        base.Die();
    }

    /// <summary>
    /// 静态方法，外部需要生成部位的时候调用。
    /// </summary>
    public static void PartApear()
    {
        if (ProgressManager.Instance.nowEntities[1] != null)
        {
            ((Part)ProgressManager.Instance.nowEntities[1]).IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
            return;
        }

        GameObject managerObj = new GameObject("Part4_1");

        Part4_1 newPart = managerObj.AddComponent<Part4_1>();

        ProgressManager.Instance.nowEntities[1] = newPart;

        newPart.IsVisible = true;

        if (ProgressManager.Instance.nowEntities[0] != null &&
            ProgressManager.Instance.nowEntities[0] is Entity entity4)
        {
            newPart.owner = entity4;

            newPart.owner.parts.Add(newPart);

            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
        }
        else if (ProgressManager.Instance.nowEntities[0] == null)
        {
            Debug.Log("ProgressManager.Instance.nowEntities[0]为空,为何啊........");
        }
    }
}
