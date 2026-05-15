using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part4_2 : Part
{
    public void InitPart()
    {
        partName = "2号门";
        id = 2;
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
        if (ProgressManager.Instance.nowEntities[2] != null)
        {
            ((Part)ProgressManager.Instance.nowEntities[2]).IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
            return;
        }

        GameObject managerObj = new GameObject("Part4_2");

        Part4_2 newPart = managerObj.AddComponent<Part4_2>();

        ProgressManager.Instance.nowEntities[2] = newPart;

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
