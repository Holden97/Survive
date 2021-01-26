using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo
{

}

public class EventInfo<T> : IEventInfo
{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action)
    {
        actions += action;
    }
}


public class EventInfo : IEventInfo
{
    public UnityAction actions;

    public EventInfo(UnityAction action)
    {
        actions += action;
    }
}
/// <summary>
/// 事件中心模块
/// 1.委托
/// 2.Dictionary
/// 3.观察者设计模式
/// </summary>
public class EventCenter : BaseManager<EventCenter>
{
    /// <summary>
    /// 事件中心模块的Dictionary容器
    /// key对应事件名称
    /// value对应监听事件对应的函数委托
    /// </summary>
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加事件监听（需要参数）
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="action">执行的方法</param>
    public void AddEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }
    /// <summary>
    /// 添加事件监听（不需要参数）
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="action">执行的方法</param>
    public void AddEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }
    /// <summary>
    /// 移除事件监听（需要参数）
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="action">执行的方法</param>
    public void RemoveEventListener<T>(string name, UnityAction<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }
    /// <summary>
    /// 移除事件监听（不需要参数）
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="action">执行的方法</param>
    public void RemoveEventListener(string name, UnityAction action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions -= action;
        }
    }
    /// <summary>
    /// 触发事件（需要参数）
    /// </summary>
    /// <param name="name">事件名称</param>
    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).actions?.Invoke(info);
        }
    }
    /// <summary>
    /// 触发事件（不需要参数）
    /// </summary>
    /// <param name="name">事件名称</param>
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).actions?.Invoke();
            //也可以写成eventDic[name].Invoke(info);
        }
    }

    /// <summary>
    /// 清除容器内容
    /// 主要用于场景转换时完全清空容器
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();

    }


}
