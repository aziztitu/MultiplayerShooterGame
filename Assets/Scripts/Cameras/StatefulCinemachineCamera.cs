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

    protected void Awake()
    {
        _virtualCamera = GetComponentInChildren<CinemachineVirtualCameraBase>();
        _virtualCamera.enabled = false;
        Debug.Log("Assigning Virtual Camera: " + VirtualCamera);
    }

    protected void Start()
    {
    }

    public void Activate(object stateData = null)
    {
        IsActive = true;
        this.stateData = stateData;
        _virtualCamera.enabled = true;
        OnActivated();
        OnActivatedEvent.Invoke(this);
    }

    public void Deactivate()
    {
        IsActive = false;
        stateData = null;
        _virtualCamera.enabled = false;
        OnDeactivated();
        OnDeactivatedEvent.Invoke(this);
    }

    protected abstract void OnActivated();
    protected abstract void OnDeactivated();
}