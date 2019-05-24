using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerModel : BoltGameObjectEntity<IPlayerState>
{
    public enum PlayerType
    {
        Blue,
        Red
    }

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

    public bool controllable
    {
        get
        {
            bool cameraIsStable = CinemachineCameraManager.Instance &&
                                  !CinemachineCameraManager.Instance.CinemachineBrain.IsBlending;

            return flightModelInControl == null && cameraIsStable;
        }
    }

    private bool isInArena => ArenaDataManager.Instance != null;

    #endregion

    private FlightModel privateFlightModel;
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
                try
                {
                    DestroyPlayer();

                    Transform prevCameraTransform = CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera
                        .VirtualCamera.transform;
                    CinemachineCameraManager.Instance.SwitchCameraState(CinemachineCameraManager.CinemachineCameraState
                        .FreeFly);
                    /*CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera.VirtualCamera.transform.position =
                        prevCameraTransform.position;
                    CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera.VirtualCamera.transform.rotation =
                        prevCameraTransform.rotation;*/
                }
                catch (Exception e)
                {
                    Debug.LogError(e.StackTrace);
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

        if (entity.IsOwner)
        {
            var playerType = PlayerType.Blue;

            if (isInArena)
            {
                playerType = ArenaLevelManager.Instance.GetLocalPlayerType();
            }

            var privateFlightGameObject = BoltNetwork.Instantiate(PlayerTypeMapping.flightPrefabs[playerType]);
            privateFlightModel = privateFlightGameObject.GetComponent<FlightModel>();
            privateFlightModel.Show(false);

            Debug.Log("Level Player Type: " + LevelManager.Instance.levelPlayerType);
            
            if (LevelManager.Instance.levelPlayerType == LevelManager.LevelPlayerType.FlightOnly)
            {
                TakeControlOfPrivateFlight();
            }
        }
    }

    public void SetupState()
    {
        state.SetTransforms(state.PlayerTransform, transform);
        state.SetAnimator(_animator);

        if (entity.IsOwner && isInArena)
        {
            state.ArenaPlayerId = ArenaDataManager.Instance.localPlayerId;
        }
    }

    public override void SimulateOwner()
    {
        base.SimulateOwner();

        if (!controllable)
        {
            return;
        }

        PlayerInputController.PlayerInput playerInput = playerInputController.GetPlayerInput();
        if (playerInput.enterFlight && privateFlightModel && LevelManager.Instance.levelPlayerType !=
            LevelManager.LevelPlayerType.PlayerOnly)
        {
            TakeControlOfPrivateFlight();
        }
    }

    void TakeControlOfPrivateFlight()
    {
        if (flightModelInControl == null && privateFlightModel)
        {
            privateFlightModel.transform.position = transform.position;
            privateFlightModel.transform.rotation = transform.rotation;
            privateFlightModel.RequestControl(this);
            privateFlightModel.Show(true);
        }
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
        state.IsGameObjectActive = false;
//        playerHUDController.Show(false);

        CinemachineCameraManager.AddOnSingletonReadyListener((cinemachineCameraManagerInstance) =>
        {
            CinemachineCameraManager.Instance.SwitchCameraState(CinemachineCameraManager.CinemachineCameraState
                .ThirdPerson);
        });
    }

    public void OnFlightControlRevoked()
    {
        if (flightModelInControl == null)
        {
            return;
        }

        _characterController.enabled = false;
        transform.position = flightModelInControl.transform.position;

        Vector3 forwardDir = flightModelInControl.transform.forward;
        forwardDir.y = 0;

        transform.LookAt(transform.position + (forwardDir * 5), Vector3.up);

        _characterController.enabled = true;

        if (flightModelInControl == privateFlightModel)
        {
            flightModelInControl.Show(false);
        }

        flightModelInControl = null;
        state.IsGameObjectActive = true;
//        playerHUDController.Show(true);

        CinemachineCameraManager.Instance.SwitchCameraState(CinemachineCameraManager.CinemachineCameraState
            .FirstPerson);
    }

    public void DestroyPlayer()
    {
        BoltNetwork.Destroy(gameObject);

        if (privateFlightModel != null)
        {
            BoltNetwork.Destroy(privateFlightModel.gameObject);
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LocalPlayerModel = null;
        }
    }
}