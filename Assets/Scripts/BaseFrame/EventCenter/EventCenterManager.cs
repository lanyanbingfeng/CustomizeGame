
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EventInfoBase{} //事件数据父类

/// <summary>
/// 带一个参数的事件数据
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventInfo<T> : EventInfoBase
{
    public UnityAction<T> action;
    public EventInfo(UnityAction<T> action) { this.action = action; }
}

/// <summary>
/// 无参数事件数据
/// </summary>
public class EventInfo : EventInfoBase
{
    public UnityAction action;
    public EventInfo(UnityAction action) { this.action = action; }
}

/// <summary>
/// 事件中心管理器
/// </summary>
public class EventCenterManager : SingletonManager<EventCenterManager>
{
    private EventCenterManager(){}
    
    private Dictionary<E_EventType,EventInfoBase> eventDic = new();

    //是否打印事件信息
    private bool isDebugInfo = true;

    /// <summary>
    /// 有一个参数事件触发器
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="info"></param>
    /// <typeparam name="T"></typeparam>
    public void EventTrigger<T>(E_EventType eventName, T info)
    {
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo<T>)eventDic[eventName]).action?.Invoke(info);
            if (isDebugInfo) Debug.Log($"触发{eventName}事件成功，当前事件数量：" + eventDic.Count);
        }
        else if (isDebugInfo) Debug.LogWarning("没有 " + eventName + " 事件");
    }
    /// <summary>
    /// 无参数事件触发器
    /// </summary>
    /// <param name="eventName"></param>
    public void EventTrigger(E_EventType eventName)
    {
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo)eventDic[eventName]).action?.Invoke();
            if (isDebugInfo) Debug.Log($"触发{eventName}事件成功，当前事件数量：" + eventDic.Count);
        }
        else if (isDebugInfo) Debug.LogWarning("没有 " + eventName + " 事件");
    }
    /// <summary>
    /// 有一个参数事件添加器
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    /// <typeparam name="T"></typeparam>
    public void AddEventListener<T>(E_EventType eventName, UnityAction<T> listener)
    {
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo<T>)eventDic[eventName]).action += listener;
            if (isDebugInfo) Debug.Log("添加事件成功，当前事件数量：" + eventDic.Count);
        }
        else eventDic.Add(eventName, new EventInfo<T>(listener));
    }
    /// <summary>
    /// 无参数事件添加器
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public void AddEventListener(E_EventType eventName, UnityAction listener)
    {
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo)eventDic[eventName]).action += listener;
            if (isDebugInfo) Debug.Log("添加事件成功，当前事件数量：" + eventDic.Count);
        }
        else eventDic.Add(eventName, new EventInfo(listener));
    }
    /// <summary>
    /// 有一个参数事件删除器
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    /// <typeparam name="T"></typeparam>
    public void RemoveEventListener<T>(E_EventType eventName, UnityAction<T> listener)
    {
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo<T>)eventDic[eventName]).action -= listener;
            if (isDebugInfo) Debug.Log("删除事件成功，当前事件数量：" + eventDic.Count);
        }
    }
    /// <summary>
    /// 无参数事件删除器
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public void RemoveEventListener(E_EventType eventName, UnityAction listener)
    {
        if (eventDic.ContainsKey(eventName))
        {
            ((EventInfo)eventDic[eventName]).action -= listener;
            if (isDebugInfo) Debug.Log("删除事件成功，当前事件数量：" + eventDic.Count);
        }
    }
    //清空事件
    public void ClearEvent(E_EventType eventName){if (eventDic.ContainsKey(eventName)) eventDic.Remove(eventName);}
    public void ClearEvent() {eventDic.Clear();}
}
