using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MessageCenter
{
    private static Dictionary<int,List<Action>> messageDic = new Dictionary<int, List<Action>>();
    

    public static void RegisterMessage(int type, Action action)
    {
        if (messageDic.ContainsKey(type))
        {
            if (messageDic[type].Equals(action))
            {
                Debug.LogWarning("消息重复添加");
            }
            else
            {
                messageDic[type].Add(action);
            }
        }
        else
        {
            messageDic.Add(type, new List<Action>() { action });
        }
    }

    public static void UnRegisterMessage(int type, Action action)
    {
        if (messageDic.ContainsKey(type))
        {
            if (messageDic[type].Equals(action))
            {
                messageDic[type].Remove(action);
            }
            else
            {
                Debug.LogWarning("注销的消息不存在");
            }
        }
        else
        {
            Debug.LogWarning("注销的消息不存在");
        }
    }

    public static void SendMessage(int type)
    {
        if (messageDic.ContainsKey(type))
        {
            var actionList = messageDic[type];
            foreach (var tempAction in actionList)
            {
                if(tempAction!=null)
                {
                    tempAction();
                }
            }
        }
        else
        {
            Debug.LogWarning("监听消息者不存在");
        }
    }
}

public static class MessageCenter<T>
{
    private static Dictionary<int, List<Action<T>>> messageDic = new Dictionary<int, List<Action<T>>>();


    public static void RegisterMessage(int type, Action<T> action)
    {
        if (messageDic.ContainsKey(type))
        {
            if (messageDic[type].Equals(action))
            {
                Debug.LogWarning("消息重复添加");
            }
            else
            {
                messageDic[type].Add(action);
            }
        }
        else
        {
            messageDic.Add(type, new List<Action<T>>() { action });
        }
    }

    public static void UnRegisterMessage(int type,Action<T> action)
    {
        if (messageDic.ContainsKey(type))
        {
            if (messageDic[type].Equals(action))
            {
                messageDic[type].Remove(action);
            }
            else
            {
                Debug.LogWarning("注销的消息不存在");
            }
        }
        else
        {
            Debug.LogWarning("注销的消息不存在");
        }
    }

    public static void SendMessage(int type,T arg)
    {
        if (messageDic.ContainsKey(type))
        {
            var actionList = messageDic[type];
            foreach (var tempAction in actionList)
            {
                if (tempAction != null)
                {
                    tempAction(arg);
                }
            }
        }
        else
        {
            Debug.LogWarning("监听消息者不存在");
        }
    }
}

public static class MessageCenter<T1,T2>
{
    private static Dictionary<int, List<Action<T1,T2>>> messageDic = new Dictionary<int, List<Action<T1,T2>>>();


    public static void RegisterMessage(int type, Action<T1,T2> action)
    {
        if (messageDic.ContainsKey(type))
        {
            if (messageDic[type].Equals(action))
            {
                Debug.LogWarning("消息重复添加");
            }
            else
            {
                messageDic[type].Add(action);
            }
        }
        else
        {
            messageDic.Add(type, new List<Action<T1, T2>>() { action });
        }
    }

    public static void UnRegisterMessage(int type, Action<T1, T2> action)
    {
        if (messageDic.ContainsKey(type))
        {
            if (messageDic[type].Equals(action))
            {
                messageDic[type].Remove(action);
            }
            else
            {
                Debug.LogWarning("注销的消息不存在");
            }
        }
        else
        {
            Debug.LogWarning("注销的消息不存在");
        }
    }

    public static void SendMessage(int type, T1 arg1, T2 arg2)
    {
        if (messageDic.ContainsKey(type))
        {
            var actionList = messageDic[type];
            foreach (var tempAction in actionList)
            {
                if (tempAction != null)
                {
                    tempAction(arg1, arg2);
                }
            }
        }
        else
        {
            Debug.LogWarning("监听消息者不存在");
        }
    }
}

public static class MessageCenter<T1, T2, T3>
{
    private static Dictionary<int, List<Action<T1, T2, T3>>> messageDic = new Dictionary<int, List<Action<T1, T2, T3>>>();


    public static void RegisterMessage(int type, Action<T1, T2, T3> action)
    {
        if (messageDic.ContainsKey(type))
        {
            if (messageDic[type].Equals(action))
            {
                Debug.LogWarning("消息重复添加");
            }
            else
            {
                messageDic[type].Add(action);
            }
        }
        else
        {
            messageDic.Add(type, new List<Action<T1, T2, T3>>() { action });
        }
    }

    public static void UnRegisterMessage(int type, Action<T1, T2, T3> action)
    {
        if (messageDic.ContainsKey(type))
        {
            if (messageDic[type].Equals(action))
            {
                messageDic[type].Remove(action);
            }
            else
            {
                Debug.LogWarning("注销的消息不存在");
            }
        }
        else
        {
            Debug.LogWarning("注销的消息不存在");
        }
    }

    public static void SendMessage(int type, T1 arg1, T2 arg2, T3 arg3)
    {
        if (messageDic.ContainsKey(type))
        {
            var actionList = messageDic[type];
            foreach (var tempAction in actionList)
            {
                if (tempAction != null)
                {
                    tempAction(arg1, arg2, arg3);
                }
            }
        }
        else
        {
            Debug.LogWarning("监听消息者不存在");
        }
    }
}
