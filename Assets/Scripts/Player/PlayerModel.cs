using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerModel : Bolt.EntityBehaviour<IPlayerState>
{
    public Transform thirdPersonCamTarget;
    public Transform firstPersonCamTransform;

    #region Accessors/Mutators

    public PlayerInputController playerInputController { get; private set; }
    public PlayerMovementController playerMovementController { get; private set; }
    public PlayerCombatController playerCombatController { get; private set; }
    public PlayerHUDController playerHUDController { get; private set; }
    public Health health { get; private set; }
    public FirstPersonCamera firstPersonCamera { get; private set; }
    public ThirdPersonPlayerCamera thirdPersonPlayerCamera { get; private set; }

    #endregion

    private PlayerInputController _playerInputController;
    private PlayerMovementController _playerMovementController;
    private Animator _animator;

    void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        playerMovementController = GetComponent<PlayerMovementController>();
        playerInputController = GetComponent<PlayerInputController>();
        playerCombatController = GetComponent<PlayerCombatController>();
        playerHUDController = GetComponentInChildren<PlayerHUDController>();
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

    // Update is called once per frame
    void Update()
    {
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
}