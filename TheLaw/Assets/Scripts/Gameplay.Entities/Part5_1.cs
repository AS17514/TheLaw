using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part5_1 : Part
{
    public void InitPart()
    {
        partName = "他人";
        id = 1;
        maxHp = 10;
        hp = maxHp;
    }

    public void Awake()
    {
        InitPart();
    }

    public bool isHpLocked = true;
    
    public override void BeAttacked(int atk)
    {
        if (ProgressManager.Instance.player.GetBuff(E_BuffType.Down) > 0)
            atk--;
        if(isHpLocked&&atk>=hp)
        {
            base.BeAttacked(hp - 1);
            owner.AddBuff(E_BuffType.Desire, 1-hp);
            StateManager.Instance.ChangeState(E_StateType_5.throwupthem);
        }
        else
        {
            base.BeAttacked(atk);
            owner.AddBuff(E_BuffType.Desire, atk);
        }
    }

    public override void Die()
    {
        DiceManager.Instance.AddDice(E_DiceType.Wild);
        base.Die();
    }

    /// <summary>
    /// 静态方法，外部需要生成部位的时候调用。
    /// </summary>
    public static void PartApear()
    {
        if (ProgressManager.Instance.nowEntities[1] != null)
        {
            ProgressManager.Instance.nowEntities[1].IsVisible = true;
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
            return;
        }

        GameObject managerObj = new GameObject("Part5_1");

        Part5_1 newPart = managerObj.AddComponent<Part5_1>();

        ProgressManager.Instance.nowEntities[1] = newPart;

        newPart.IsVisible = true;

        if (ProgressManager.Instance.nowEntities[0] != null &&
            ProgressManager.Instance.nowEntities[0] is Entity entity5)
        {
            newPart.owner = entity5;

            newPart.owner.parts.Add(newPart);

            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
        }
        else if (ProgressManager.Instance.nowEntities[0] == null)
        {
            Debug.Log("ProgressManager.Instance.nowEntities[0]为空,为何啊........");
        }
    }
}
