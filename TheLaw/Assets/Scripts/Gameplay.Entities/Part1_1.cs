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
    }

    public void Awake()
    {
        InitPart();
    }

    public void PartApear()
    {
        GameObject managerObj = new GameObject("Part1_1");
        ProgressManager.Instance.nowEntities[1] = managerObj.AddComponent<Part1_1>();
        if (ProgressManager.Instance.nowEntities[0] != null &&
            ProgressManager.Instance.nowEntities[0] is Entity entity1)
        {
            owner = entity1;
            owner.parts.Add(this);
            EventCenter.Instance.EventTrigger(E_EventType.UI_Update_EntityPartIsVisible,ProgressManager.Instance.nowEntities);
        }
        else if(ProgressManager.Instance.nowEntities[0]==null)
        {
            Debug.Log("ProgressManager.Instance.nowEntities[0]为空,为何啊........");
        }
    }
}
