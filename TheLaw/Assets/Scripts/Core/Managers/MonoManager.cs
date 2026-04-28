using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoManager : ManagerMonoBase<MonoManager>
{
    // 只实现了update，其他自行添加实现
    // 此管理器只能实现让不继承mono的脚本使用循环更新类unity生命周期函数
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
