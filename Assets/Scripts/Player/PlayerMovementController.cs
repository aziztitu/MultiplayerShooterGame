using System.Collections;
using System.Collections.Generic;
using Bolt;
using Cinemachine;
using UnityEngine;

public class PlayerMovementController : Bolt.EntityBehaviour<IPlayerState>
{
    [Header("Movement Properties")] public float maxWalkSpeed = 0.1f;
    public float maxRunSpeed = 0.2f;
    public float acceleration = 2f;
    public float deceleration = 2f;
    public float turnSpeed = 3f;

//    public float rotationSpeed = 3;
//    public float dodgeSpeed = 10;
//    public float dodgeDuration = 3;

    private PlayerModel _playerModel;
    private CharacterController _characterController;
    private Animator _animator;

    private float curSpeed = 0;

    void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
    }

    // Use this for initialization
    void Start()
    {
    }

    void Update()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }

    public override void SimulateOwner()
    {
        base.SimulateOwner();
        
        PlayerInputController.PlayerInput playerInput = _playerModel.playerInputController.GetPlayerInput();
        Move(playerInput);
    }

    void ApplyGravityIfNeeded()
    {
    }

    void Move(PlayerInputController.PlayerInput playerInput)
    {
        float targetSpeed = 0;
        switch (CinemachineCameraManager.Instance.CurrentState)
        {
            case CinemachineCameraManager.CinemachineCameraState.FirstPerson:
                FirstPersonMove(playerInput, out targetSpeed);
                break;
            case CinemachineCameraManager.CinemachineCameraState.ThirdPerson:
                ThirdPersonMove(playerInput, out targetSpeed);
                break;
            default:
                return;
        }
        
        bool isSprinting = IsSprinting(playerInput);

        // Move Animation
        Vector3 animVector = new Vector3(playerInput.strafe, 0, playerInput.forward);
        if (!isSprinting)
        {
            animVector /= 2;
        }

        float curSpeedFactor = HelperUtilities.Remap(curSpeed, 0, targetSpeed, 0, 1);
        animVector *= curSpeedFactor;

//        Debug.Log("Strafe: " + animVector.x);
//        Debug.Log("Forward: " + animVector.z);

        _animator.SetFloat("Strafe", animVector.x);
        _animator.SetFloat("Forward", animVector.z);
//        _animator.SetBool("IsMoving", curSpeedFactor > 0);
        _animator.SetBool("IsGrounded", true);
        _animator.SetBool("IsSprinting", isSprinting);
    }

    void FirstPersonMove(PlayerInputController.PlayerInput playerInput, out float targetSpeed)
    {
        Vector3 moveVector = new Vector3(playerInput.strafe, 0, playerInput.forward);

        CinemachineVirtualCameraBase curVirtualCamera =
            CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera.VirtualCamera;
        Vector3 forwardDir = curVirtualCamera.transform.forward;
        Vector3 rightDir = curVirtualCamera.transform.right;

        forwardDir.y = 0;
        rightDir.y = 0;

//        transform.forward = forwardDir;
//        transform.right = rightDir;

        transform.LookAt(transform.position + (forwardDir * 5), Vector3.up);

        moveVector = forwardDir * playerInput.forward;
        moveVector += rightDir * playerInput.strafe;

        if (moveVector.magnitude > 1)
        {
            moveVector.Normalize();
        }

        bool isSprinting = IsSprinting(playerInput);

        targetSpeed = 0;
        if (moveVector.magnitude > 0)
        {
            targetSpeed = isSprinting ? maxRunSpeed : maxWalkSpeed;
        }

        if (targetSpeed > curSpeed)
        {
            curSpeed += acceleration * BoltNetwork.FrameDeltaTime;
            curSpeed = Mathf.Min(curSpeed, targetSpeed);
        }
        else if (targetSpeed < curSpeed)
        {
            curSpeed -= deceleration * BoltNetwork.FrameDeltaTime;
            curSpeed = Mathf.Max(curSpeed, targetSpeed);
        }

        _characterController.Move(moveVector * curSpeed);
    }

    void ThirdPersonMove(PlayerInputController.PlayerInput playerInput, out float targetSpeed)
    {
//        Debug.Log(thirdPersonPlayerCamera);

        Vector3 moveVector = new Vector3(playerInput.strafe, 0, playerInput.forward);

        CinemachineVirtualCameraBase curVirtualCamera =
            CinemachineCameraManager.Instance.CurrentStatefulCinemachineCamera.VirtualCamera;
        Vector3 forwardDir = curVirtualCamera.transform.forward;
        Vector3 rightDir = curVirtualCamera.transform.right;

        forwardDir.y = 0;
        rightDir.y = 0;

        moveVector = forwardDir * playerInput.forward;
        moveVector += rightDir * playerInput.strafe;

        if (moveVector.magnitude > 1)
        {
            moveVector.Normalize();
        }

        bool isSprinting = IsSprinting(playerInput);

        targetSpeed = 0;
        if (moveVector.magnitude > 0)
        {
            targetSpeed = isSprinting ? maxRunSpeed : maxWalkSpeed;
        }

        if (targetSpeed > curSpeed)
        {
            curSpeed += acceleration * BoltNetwork.FrameDeltaTime;
            curSpeed = Mathf.Min(curSpeed, targetSpeed);
        }
        else if (targetSpeed < curSpeed)
        {
            curSpeed -= deceleration * BoltNetwork.FrameDeltaTime;
            curSpeed = Mathf.Max(curSpeed, targetSpeed);
        }

        /*if (moveVector.magnitude > 0)
        {
            Debug.Log(thirdPersonPlayerCamera);
            Vector3 lookAtTarget = transform.position + (thirdPersonPlayerCamera.VirtualCamera.transform.forward * 5);
            lookAtTarget.y = transform.position.y;

            Vector3 targetForward = lookAtTarget - transform.position;
            targetForward.Normalize();

            transform.forward = Vector3.Lerp(transform.forward, targetForward, turnSpeed * Time.fixedDeltaTime);
        }*/

        _characterController.Move(moveVector * curSpeed);
    }

    bool IsSprinting(PlayerInputController.PlayerInput playerInput)
    {
        return (playerInput.sprint && !playerInput.aim && ((playerInput.forward > 0) ||
                                                                         (!(playerInput.forward < 0) &&
                                                                          Mathf.Abs(playerInput.strafe) > 0)));
    }
}