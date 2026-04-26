using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoManager : ManagerMonoBase<MonoManager>
{
    // 只实现了update，其他自行添加实现
    event UnityAction updateEvent;
    void Update()
    {
        updateEvent?.Invoke();
    }

    public void AddUpdateListener(UnityAction updateFun)
    {
        updateEvent += updateFun;
    }
    public void RemoveUpdateListener(UnityAction updateFun)
    {
        updateEvent -= updateFun;
    }
}
