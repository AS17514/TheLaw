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
}
