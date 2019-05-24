using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public abstract class StatefulCinemachineCamera : MonoBehaviour
{
    [Serializable]
    public class StatefulCinemachineCameraEvent : UnityEvent<StatefulCinemachineCamera>
    {
    }

    public CinemachineCameraManager.CinemachineCameraState cinemachineCameraState;
    public StatefulCinemachineCameraEvent OnActivatedEvent;
    public StatefulCinemachineCameraEvent OnDeactivatedEvent;

    public object stateData { get; private set; }

    private CinemachineVirtualCameraBase _virtualCamera;

    public CinemachineVirtualCameraBase VirtualCamera => _virtualCamera;

    public bool IsActive { get; private set; }

    private bool isInitialized = false;
    protected event Action onInitialization;

    protected void Awake()
    {
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCameraBase>();
        _virtualCamera.enabled = false;
        Debug.Log("Assigning Virtual Camera: " + VirtualCamera);

        isInitialized = true;

        onInitialization?.Invoke();
        if (onInitialization != null)
        {
            foreach (Delegate d in onInitialization.GetInvocationList())
            {
                onInitialization -= (Action) d;
            }
        }
    }

    protected void Start()
    {
    }

    public void Activate(object stateData = null)
    {
        if (!isInitialized)
        {
            onInitialization += () =>
            {
                Activate(stateData);
            };
            return;
        }
        
        IsActive = true;
        this.stateData = stateData;
        _virtualCamera.enabled = true;
        OnActivated();
        OnActivatedEvent.Invoke(this);
    }

    public void Deactivate()
    {
        if (!isInitialized)
        {
            onInitialization += Deactivate;
            return;
        }
        
        IsActive = false;
        stateData = null;
        _virtualCamera.enabled = false;
        OnDeactivated();
        OnDeactivatedEvent.Invoke(this);
    }

    protected abstract void OnActivated();
    protected abstract void OnDeactivated();
}