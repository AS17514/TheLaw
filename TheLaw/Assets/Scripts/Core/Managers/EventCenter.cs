using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 事件中心模块
/// </summary>
public class EventCenter : ManagerBase<EventCenter>
{
    //存储事件对应的委托
    private Dictionary<E_EventType, UnityAction<object>> eventDic = new Dictionary<E_EventType, UnityAction<object>>();

    private EventCenter() { }

    /// <summary>
    /// 向事件中心通知此事件被触发
    /// </summary>
    /// <param name="eventName">对应事件</param>
    /// <param name="info">为事件监听者传回的参数</param>
    public void EventTrigger(E_EventType eventName, object info = null)
    {
        //判断是否记录了对应事件的委托
        if (eventDic.ContainsKey(eventName))
        {
            // 判断是否为空后，执行委托
            // Debug.Log($"invoke {eventName}");
            eventDic[eventName]?.Invoke(info);
        }
    }

    /// <summary>
    /// 向事件中心添加事件监听
    /// </summary>
    /// <param name="eventName">想要监听的事件</param>
    /// <param name="func">事件触发后要执行的委托</param>
    public void AddEventListener(E_EventType eventName, UnityAction<object> func)
    {
        //判断是否已经存储了对应事件，没有则新建
        if (eventDic.ContainsKey(eventName))
            eventDic[eventName] += func;
        else
        {
            eventDic.Add(eventName, null);
            eventDic[eventName] += func;
        }

    }

    /// <summary>
    /// 移除对应事件监听中的某委托（仅一次）
    /// </summary>
    /// <param name="eventName">想要移除对应委托的事件</param>
    /// <param name="func">想要从中移除的委托</param>
    public void RemoveEventListener(E_EventType eventName, UnityAction<object> func)
    {
        if (eventDic.ContainsKey(eventName))
            eventDic[eventName] -= func;
    }

    /// <summary>
    /// 清空所有事件的监听
    /// </summary>
    public void ClearAllEvents()
    {
        eventDic.Clear();
    }

    /// <summary>
    /// 清除指定某一个事件的所有监听
    /// </summary>
    /// <param name="eventName">事件名</param>
    public void ClearEventListeners(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
            eventDic.Remove(eventName);
    }
    
    /// <summary>
    /// 检查某事件监听是否为空，或是否仅包含特定的委托
    /// </summary>
    public bool IsEventListenersNull(E_EventType eventName, UnityAction<object> func)
    {
        // 修复1：必须先判断字典中是否包含该 Key，否则直接索引会报 KeyNotFoundException 错误
        if (!eventDic.ContainsKey(eventName))
        {
            return true;
        }

        // 修复2：判断当前事件绑定的委托是否为空
        if (eventDic[eventName] == null)
        {
            return true;
        }
        
        // 修复3：is 关键字是用来判断【类型】的，不能用来对比两个【变量】
        // 如果你想判断这个事件绑定的委托是不是正好等于 func 这个委托，应该用 ==
        if (eventDic[eventName] == func)
        {
            return true;
        }

        return false;
    }

}
