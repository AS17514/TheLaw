using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part1_1 : Part
{
    public void InitPart()
    {
        name = "corner of the table";
        id = 1;
        maxHp = 5;
        hp = maxHp;
    }

    public void Awake()
    {
        InitPart();
    }

    public override void Die()
    {
        base.Die();
        if (ProgressManager.Instance.nowEntities[2] != null && ProgressManager.Instance.nowEntities[2] is Part1_2 part2)
        {
            if (part2.isDestroyed)
            {
                StateManager.Instance.ChangeState(E_StateType_1.exhausted);
            }
        }
    }

    /// <summary>
    /// 静态方法，外部需要生成部位的时候调用。
    /// </summary>
    public static void PartApear()
    {

        GameObject managerObj = new GameObject("Part1_1");

        // 关键修改 1：把新生成的组件存进一个局部变量 newPart 里
        Part1_1 newPart = managerObj.AddComponent<Part1_1>();

        // 用新变量赋值给管理器
        ProgressManager.Instance.nowEntities[1] = newPart;

        if (ProgressManager.Instance.nowEntities[0] != null &&
            ProgressManager.Instance.nowEntities[0] is Entity entity1)
        {
            // 关键修改 2：把原本的 owner 改成 newPart.owner
            newPart.owner = entity1;

            // 关键修改 3：把原本的 this 改成 newPart
            newPart.owner.parts.Add(newPart);

            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart, ProgressManager.Instance.nowEntities);
        }
        else if (ProgressManager.Instance.nowEntities[0] == null)
        {
            Debug.Log("ProgressManager.Instance.nowEntities[0]为空,为何啊........");
        }
    }
}
