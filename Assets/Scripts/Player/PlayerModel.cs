using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Bolt.EntityBehaviour<IPlayerState>
{
    public Transform thirdPersonCamTarget;
    public Transform firstPersonCamTransform;

    #region Accessors/Mutators

    public PlayerInputController playerInputController
    {
        get
        {
            if (_playerInputController == null)
            {
                _playerInputController = GetComponent<PlayerInputController>();
            }

            return _playerInputController;
        }
    }

    public PlayerMovementController playerMovementController
    {
        get
        {
            if (_playerMovementController == null)
            {
                _playerMovementController = GetComponent<PlayerMovementController>();
            }

            return _playerMovementController;
        }
    }

    public FirstPersonCamera firstPersonCamera { get; private set; }
    public ThirdPersonPlayerCamera thirdPersonPlayerCamera { get; private set; }

    #endregion

    private PlayerInputController _playerInputController;
    private PlayerMovementController _playerMovementController;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        FindRequiredCameras();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void Attached()
    {
        base.Attached();
        SetupState();
    }

    public void SetupState()
    {
        state.SetTransforms(state.PlayerTransform, transform);
        state.SetAnimator(_animator);
    }

    private void FindRequiredCameras()
    {
        List<StatefulCinemachineCamera> statefulCinemachineCameras =
            CinemachineCameraManager.Instance.GetStatefulCinemachineCameras();

        foreach (StatefulCinemachineCamera statefulCinemachineCamera in statefulCinemachineCameras)
        {
            ThirdPersonPlayerCamera tppc = statefulCinemachineCamera as ThirdPersonPlayerCamera;
            FirstPersonCamera fpc = statefulCinemachineCamera as FirstPersonCamera;

            if (tppc != null)
            {
                thirdPersonPlayerCamera = tppc;
            }

            if (fpc != null)
            {
                firstPersonCamera = fpc;
            }
        }
    }
}