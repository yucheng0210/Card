using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public delegate void EventHandler(params object[] args);
    private Dictionary<string, EventHandler> eventListenters = new Dictionary<
        string,
        EventHandler
    >();

    public void AddEventRegister(string eventName, EventHandler handler)
    {
        if (handler == null)
            return;
        if (eventListenters.ContainsKey(eventName))
            eventListenters[eventName] += handler;
        else
            eventListenters.Add(eventName, handler);
    }

    public void RemoveEventRegister(string eventName, EventHandler handler)
    {
        if (handler == null)
            return;
        if (eventListenters.ContainsKey(eventName))
        {
            eventListenters[eventName] -= handler;
            if (eventListenters[eventName] == null)
                eventListenters.Remove(eventName);
        }
    }

    public void DispatchEvent(string eventName, params object[] objs)
    {
        if (eventListenters.ContainsKey(eventName))
        {
            EventHandler eventHandler = eventListenters[eventName];
            if (eventHandler != null)
                eventHandler(objs);
        }
    }

    //刪除觸發事件對應的所有執行事件
    public void ClearEvents(string eventName)
    {
        if (eventListenters.ContainsKey(eventName))
            eventListenters.Remove(eventName);
    }
}
