using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerModel : Bolt.EntityBehaviour<IPlayerState>
{
    public Transform thirdPersonCamTarget;
    public Transform firstPersonCamFollow;
    public GameObject playerAvatar;

    [Header("During Test Scenes Only")] [SerializeField] [CanBeNull]
    private FlightModel _flightModelInControl;

    #region Accessors/Mutators

    public PlayerInputController playerInputController { get; private set; }
    public PlayerMovementController playerMovementController { get; private set; }
    public PlayerCombatController playerCombatController { get; private set; }
    public PlayerHUDController playerHUDController { get; private set; }
    public Health health { get; private set; }
    public FirstPersonCamera firstPersonCamera { get; private set; }
    public ThirdPersonPlayerCamera thirdPersonPlayerCamera { get; private set; }
    public InteractionController interactionController { get; private set; }

    [CanBeNull]
    public FlightModel flightModelInControl
    {
        get { return _flightModelInControl; }
        private set { _flightModelInControl = value; }
    }

    #endregion

    private CharacterController _characterController;
    private Animator _animator;

    void Awake()
    {
        _characterController = GetComponentInChildren<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        playerMovementController = GetComponent<PlayerMovementController>();
        playerInputController = GetComponent<PlayerInputController>();
        playerCombatController = GetComponent<PlayerCombatController>();
        playerHUDController = GetComponentInChildren<PlayerHUDController>();
        interactionController = GetComponentInChildren<InteractionController>();
        health = GetComponent<Health>();

        health.OnDeath.AddListener((() =>
        {
            if (entity.IsOwner)
            {
                BoltNetwork.Destroy(gameObject);

                Transform prevCameraTransform = CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera
                    .VirtualCamera.transform;
                CinemachineCameraManager.Instance.SwitchCameraState(CinemachineCameraManager.CinemachineCameraState
                    .FreeFly);
                /*CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera.VirtualCamera.transform.position =
                    prevCameraTransform.position;
                CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera.VirtualCamera.transform.rotation =
                    prevCameraTransform.rotation;*/

                if (LevelManager.Instance != null)
                {
                    LevelManager.Instance.LocalPlayerModel = null;
                }
            }
        }));
    }

    // Use this for initialization
    void Start()
    {
        FindRequiredCameras();
    }

    public override void Attached()
    {
        base.Attached();
        SetupState();

        playerHUDController.gameObject.SetActive(entity.IsOwner);
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

    public void OnTakenFlightControl(FlightModel flightModel)
    {
        flightModelInControl = flightModel;
        gameObject.SetActive(false);
//        playerHUDController.Show(false);
        
        CinemachineCameraManager.Instance.SwitchCameraState(CinemachineCameraManager.CinemachineCameraState
            .ThirdPerson);
    }

    public void OnFlightControlRevoked()
    {
        if (flightModelInControl == null)
        {
            return;
        }

        _characterController.enabled = false;
        transform.position = flightModelInControl.transform.position + (flightModelInControl.transform.up * 3f);
        _characterController.enabled = true;


        flightModelInControl = null;
        gameObject.SetActive(true);
//        playerHUDController.Show(true);
        
        CinemachineCameraManager.Instance.SwitchCameraState(CinemachineCameraManager.CinemachineCameraState
            .FirstPerson);
    }
}