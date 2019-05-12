/*
 * A Simple Generic StateMachine by Azee.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class StateMachine<T>
{
    private readonly T _owner;
    private State _previousState;
    private State _currentState;

    private readonly List<OnStateSwitchedCallback> _onStateSwitchedCallbacks = new List<OnStateSwitchedCallback>();

    public StateMachine(T owner)
    {
        _owner = owner;
    }

    public State GetCurrentState()
    {
        return _currentState;
    }

    public State GetPreviousState()
    {
        return _previousState;
    }

    public void AddOnStateSwitchedCallback(OnStateSwitchedCallback callback)
    {
        _onStateSwitchedCallbacks.Add(callback);
    }

    public void SwitchState(State newState, params object[] args)
    {
        if (_currentState != null)
        {
            _currentState.Exit(_owner);
            _previousState = _currentState;
        }

        _currentState = newState;
        _currentState.Enter(_owner, args);

        foreach (OnStateSwitchedCallback onStateSwitchedCallback in _onStateSwitchedCallbacks)
        {
            onStateSwitchedCallback(this);
        }
    }

    public void Update()
    {
        if (_currentState != null)
        {
            _currentState.Update(_owner);
        }
    }


    public delegate void OnStateSwitchedCallback(StateMachine<T> stateMachine);


    public interface State
    {
        void Enter(T owner, params object[] args);
        void Update(T owner);
        void Exit(T owner);
    }

    /*
     * ==============
     *  Sample State
     * ==============

    namespace OwnerStates
    {
        public class SampleState : StateMachine<T>.State
        {
            public class StateData
            {
                public int data1;
                public Object data2;
            }
            

            private static SampleState _instance;

            public static SampleState Instance
            {
                get { return _instance ?? (_instance = new SampleState()); }
            }

            private SampleState()
            {
            }

            public void Enter(T owner, params object[] args)
            {

            }

            public void Update(T owner)
            {
                
            }

            public void Exit(T owner)
            {
                
            }
        }
    }
    
    */
}