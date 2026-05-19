using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part3_2 : Part
{
    public void InitPart()
    {
        partName = "宇航服";
        id = 2;
        maxHp = 6;
        hp = maxHp;
    }

    public void Awake()
    {
        InitPart();
    }

    public override void Die()
    {
        base.Die();
        StateManager.Instance.ChangeState(E_StateType_3.weightless);
        
    }

    /// <summary>
    /// 静态方法，外部需要生成部位的时候调用。
    /// </summary>
    public static void PartApear()
    {

        GameObject managerObj = new GameObject("Part3_2");

        // 关键修改 1：把新生成的组件存进一个局部变量 newPart 里
        Part3_2 newPart = managerObj.AddComponent<Part3_2>();

        // 用新变量赋值给管理器
        ProgressManager.Instance.nowEntities[2] = newPart;

        newPart.IsVisible = true;

        if (ProgressManager.Instance.nowEntities[0] != null &&
            ProgressManager.Instance.nowEntities[0] is Entity entity3)
        {
            // 关键修改 2：把原本的 owner 改成 newPart.owner
            newPart.owner = entity3;

            // 关键修改 3：把原本的 this 改成 newPart
            newPart.owner.parts.Add(newPart);

            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart);
        }
        else if (ProgressManager.Instance.nowEntities[0] == null)
        {
            Debug.Log("ProgressManager.Instance.nowEntities[0]为空,为何啊........");
        }
    }
}
