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
    private static readonly List<Action<T>> OnSingletonReadyListeners_OneShot = new List<Action<T>>();

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

        foreach (var action in OnSingletonReadyListeners_OneShot)
        {
            action(Instance);
        }
        OnSingletonReadyListeners_OneShot.Clear();
    }

    public static void AddOnSingletonReadyListener(Action<T> action, bool oneShot = false)
    {
        if (Instance != null)
        {
            action(Instance);

            if (oneShot)
            {
                return;
            }
        }

        if (oneShot)
        {
            OnSingletonReadyListeners_OneShot.Add(action);
        }
        else
        {
            OnSingletonReadyListeners.Add(action);
        }
    }
}