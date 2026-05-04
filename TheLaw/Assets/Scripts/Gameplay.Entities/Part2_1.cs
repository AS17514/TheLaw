using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part2_1 : Part
{
    public void InitPart()
    {
        partName = "clothesline";
        id = 1;
        maxHp = 1;
        hp = maxHp;
    }

    public void Awake()
    {
        InitPart();
    }

    /// <summary>
    /// 不是对象身上的部位，不会令对象失去血量
    /// </summary>
    /// <param name="atk"></param>
    public override void BeAttacked(int atk)
    {
        TakeDamage( atk);
        EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPart, ProgressManager.Instance.nowEntities);
        StateManager.Instance.ChangeState(E_StateType_2.ashamed);
    }


    public override bool IsCouldBeAttacked()
    {
        return base.IsCouldBeAttacked()&&
               !(StateManager.Instance.currentState is E_StateType_2.hysterial);
    }

    /// <summary>
    /// 静态方法，外部需要生成部位的时候调用。
    /// </summary>
    public static void PartApear()
    {

        GameObject managerObj = new GameObject("Part2_1");

        // 关键修改 1：把新生成的组件存进一个局部变量 newPart 里
        Part2_1 newPart = managerObj.AddComponent<Part2_1>();

        // 用新变量赋值给管理器
        ProgressManager.Instance.nowEntities[1] = newPart;
        
        newPart.IsVisible = true;

        if (ProgressManager.Instance.nowEntities[0] != null &&
            ProgressManager.Instance.nowEntities[0] is Entity entity2)
        {
            // 关键修改 2：把原本的 owner 改成 newPart.owner
            newPart.owner = entity2;

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

