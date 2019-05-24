using System;
using System.Collections;
using System.Collections.Generic;
using BasicTools.ButtonInspector;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;


public class CinemachineCameraManager : SingletonMonoBehaviour<CinemachineCameraManager>
{
    [Serializable]
    public class OnCinemachineCameraStateUpdated : UnityEvent<CinemachineCameraState>
    {
    }

    public enum CinemachineCameraState
    {
        None,
        FirstPerson,
        ThirdPerson,
        OverTheShoulder,
        EventLock,
        FreeFly,
    }

    public CinemachineCameraState CurrentState => _currentState;

    public StatefulCinemachineCamera CurrentStatefulCinemachineCamera
    {
        get
        {
            foreach (StatefulCinemachineCamera statefulCinemachineCamera in _statefulCinemachineCameras)
            {
                if (statefulCinemachineCamera.cinemachineCameraState == CurrentState)
                {
                    return statefulCinemachineCamera;
                }
            }

            return null;
        }
    }

    public CinemachineBrain CinemachineBrain { get; private set; }

    public Camera OutputCamera { get; private set; }

    [SerializeField] private CinemachineCameraState _currentState = CinemachineCameraState.ThirdPerson;
    [SerializeField] private CinemachineCameraState _prevReturnableState = CinemachineCameraState.None;

    private Dictionary<CinemachineCameraState, bool> _returnableStates = new Dictionary<CinemachineCameraState, bool>
    {
        {CinemachineCameraState.FirstPerson, true},
        {CinemachineCameraState.ThirdPerson, true},
        {CinemachineCameraState.OverTheShoulder, true},
    };

    [SerializeField]
    private List<StatefulCinemachineCamera> _statefulCinemachineCameras = new List<StatefulCinemachineCamera>();

    public OnCinemachineCameraStateUpdated onCinemachineCameraStateUpdated;

    [SerializeField] [Button("Refresh Stateful Cameras", "RefreshStatefulCameras")]
    private bool _refreshStatefulCameras;

    new void Awake()
    {
        OutputCamera = GetComponentInChildren<Camera>();
        CinemachineBrain = GetComponentInChildren<CinemachineBrain>();

        base.Awake();
        RefreshStatefulCameras();
    }

    // Start is called before the first frame update
    void Start()
    {
        CheckAndSwitchCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.End))
        {
            HelperUtilities.UpdateCursorLock(Cursor.visible);
        }
    }

    private void OnDestroy()
    {
        HelperUtilities.UpdateCursorLock(false);
    }

    void CheckAndSwitchCamera(object stateData = null)
    {
        List<StatefulCinemachineCamera> camerasToDeactivate = new List<StatefulCinemachineCamera>();
        List<StatefulCinemachineCamera> camerasToActivate = new List<StatefulCinemachineCamera>();

        foreach (StatefulCinemachineCamera statefulCinemachineCamera in _statefulCinemachineCameras)
        {
            if (statefulCinemachineCamera.cinemachineCameraState == _currentState)
            {
                /*if (!statefulCinemachineCamera.IsActive)
                {
                    camerasToActivate.Add(statefulCinemachineCamera);
                }*/
                camerasToActivate.Add(statefulCinemachineCamera);
            }
            else
            {
                if (statefulCinemachineCamera.IsActive)
                {
                    camerasToDeactivate.Add(statefulCinemachineCamera);
                }
            }
        }

        if (camerasToActivate.Count > 0 || camerasToDeactivate.Count > 0)
        {
            onCinemachineCameraStateUpdated.Invoke(_currentState);
        }

        foreach (StatefulCinemachineCamera statefulCinemachineCamera in camerasToDeactivate)
        {
            statefulCinemachineCamera.Deactivate();
        }

        foreach (StatefulCinemachineCamera statefulCinemachineCamera in camerasToActivate)
        {
            statefulCinemachineCamera.Activate(stateData);
        }
    }

    void RefreshStatefulCameras()
    {
        _statefulCinemachineCameras.Clear();
        _statefulCinemachineCameras.AddRange(GetComponentsInChildren<StatefulCinemachineCamera>(true));
    }

    public void SwitchCameraState(CinemachineCameraState cinemachineCameraState, object stateData = null)
    {
        if (_returnableStates.ContainsKey(_currentState) && _returnableStates[_currentState])
        {
            _prevReturnableState = _currentState;
        }

        _currentState = cinemachineCameraState;
        CheckAndSwitchCamera(stateData);
    }

    public void SwitchToPreviousCameraState()
    {
        SwitchCameraState(_prevReturnableState);
    }

    public void OnCameraCut(CinemachineBrain cinemachineBrain)
    {
        Debug.Log("Camera Cut");
    }

    public void OnCameraActivated(ICinemachineCamera to, ICinemachineCamera from)
    {
        if (from != null)
        {
            Debug.Log("From: " + from.Name);
        }

        if (to != null)
        {
            Debug.Log("To: " + to.Name);
        }
    }

    public List<StatefulCinemachineCamera> FindCamerasForState(CinemachineCameraState cameraState)
    {
        List<StatefulCinemachineCamera> cameras = new List<StatefulCinemachineCamera>();

        foreach (StatefulCinemachineCamera statefulCinemachineCamera in _statefulCinemachineCameras)
        {
            if (statefulCinemachineCamera.cinemachineCameraState == cameraState)
            {
                cameras.Add(statefulCinemachineCamera);
            }
        }

        return cameras;
    }

    public List<StatefulCinemachineCamera> GetStatefulCinemachineCameras()
    {
        return _statefulCinemachineCameras;
    }
}