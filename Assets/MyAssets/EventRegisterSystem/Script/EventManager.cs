using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EventManager : Singleton<EventManager>
{
    public delegate void EventHandler(params object[] args);
    private Dictionary<string, EventHandler> eventListenters = new();
    public Dictionary<CharacterData, Dictionary<string, EventHandler>> CharacterListenerList { get; set; }
    private void Start()
    {
        CharacterListenerList = new();
    }
    public void AddEventRegister(string eventName, EventHandler handler)
    {
        if (handler == null)
        {
            return;
        }
        if (eventListenters.ContainsKey(eventName))
        {
            eventListenters[eventName] += handler;
        }
        else
        {
            eventListenters.Add(eventName, handler);
        }
    }
    public void AddEventRegister(string eventName, CharacterData character, EventHandler handler)
    {
        if (handler == null)
        {
            return;
        }
        if (eventListenters.ContainsKey(eventName))
        {
            eventListenters[eventName] += handler;
        }
        else
        {
            eventListenters.Add(eventName, handler);
        }
        // 角色特定事件註冊
        if (CharacterListenerList.ContainsKey(character))
        {
            if (CharacterListenerList[character].ContainsKey(eventName))
            {
                // 將 handler 添加到角色的指定事件
                CharacterListenerList[character][eventName] += handler;
            }
            else
            {
                // 新增角色的指定事件
                CharacterListenerList[character].Add(eventName, handler);
            }
        }
        else
        {
            // 為角色新增事件列表，並添加該事件及 handler
            CharacterListenerList[character] = new Dictionary<string, EventHandler>
            {
                { eventName, handler }
            };
        }
    }
    public void RemoveEventRegister(string eventName, EventHandler handler)
    {
        if (handler == null)
        {
            return;
        }
        if (eventListenters.ContainsKey(eventName))
        {
            eventListenters[eventName] -= handler;
            if (eventListenters[eventName] == null)
            {
                eventListenters.Remove(eventName);
            }
        }
    }

    public void DispatchEvent(string eventName, params object[] objs)
    {
        if (eventListenters.ContainsKey(eventName))
        {
            EventHandler eventHandler = eventListenters[eventName];
            if (eventHandler != null)
            {
                eventHandler(objs);
            }
        }
    }

    //刪除觸發事件對應的所有執行事件
    public void ClearEvents(string eventName)
    {
        if (eventListenters.ContainsKey(eventName))
        {
            eventListenters.Remove(eventName);
        }
    }
    public void ClearEvents(CharacterData characterData)
    {
        if (!CharacterListenerList.ContainsKey(characterData))
        {
            return;
        }
        for (int i = 0; i < CharacterListenerList[characterData].Count; i++)
        {
            string key = CharacterListenerList[characterData].ElementAt(i).Key;
            RemoveEventRegister(key, CharacterListenerList[characterData][key]);
        }
        if (CharacterListenerList.ContainsKey(characterData))
        {
            CharacterListenerList.Remove(characterData);
        }
    }
}