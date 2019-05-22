using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SingletonMonoBehaviour<T>: MonoBehaviour where T: SingletonMonoBehaviour<T>
{
    public static T Instance { get; private set; }

    public static U Get<U>() where U: T
    {
        return Instance as U;
    }

    private static readonly List<Action<T>> OnSingletonReadyListeners = new List<Action<T>>();

    protected void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            Instance = this as T;
            CallOnSingletonReadyListeners();
        } 
    }

    private static void CallOnSingletonReadyListeners()
    {
        foreach (var onSingletonReadyListener in OnSingletonReadyListeners)
        {
            onSingletonReadyListener(Instance);
        }
    }

    public static void AddOnSingletonReadyListener(Action<T> action)
    {
        if (Instance != null)
        {
            action(Instance);
        }
        
        OnSingletonReadyListeners.Add(action);
    }
}