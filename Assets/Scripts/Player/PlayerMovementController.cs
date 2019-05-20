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

//    public float turnSpeed = 3f;
    public float gravityForce = -9.81f;
    public float jumpForce = 40f;
    public float jumpForceDuration = 3;

//    public float rotationSpeed = 3;
//    public float dodgeSpeed = 10;
//    public float dodgeDuration = 3;

    private PlayerModel _playerModel;
    private CharacterController _characterController;
    private Animator _animator;

    private bool isSprinting;
    private float curSpeed = 0;
    private float lastJumpTime = float.MinValue;

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

    void FixedUpdate()
    {
        if (_playerModel.flightModelInControl != null)
        {
            return;
        }
        
        if (LevelManager.Instance.interactingWithUI)
            return;

        if (entity.IsOwner)
        {
            PlayerInputController.PlayerInput playerInput = _playerModel.playerInputController.GetPlayerInput();
            isSprinting = IsSprinting(playerInput);

            Vector3 moveVector = Move(playerInput) + AddJumpForceAsNeeded(playerInput) + ApplyGravityIfNeeded();
            _characterController.Move(moveVector);
        }
    }

    public override void SimulateOwner()
    {
        base.SimulateOwner();
        
        if (_playerModel.flightModelInControl != null)
        {
            return;
        }
    }

    Vector3 ApplyGravityIfNeeded()
    {
        if (!_characterController.isGrounded)
        {
            return (Vector3.up * gravityForce * BoltNetwork.FrameDeltaTime);
        }

        return Vector3.zero;
    }

    Vector3 Move(PlayerInputController.PlayerInput playerInput)
    {
        float targetSpeed = 0;
        Vector3 moveVector = Vector3.zero;
        switch (CinemachineCameraManager.Instance.CurrentState)
        {
            case CinemachineCameraManager.CinemachineCameraState.FirstPerson:
                moveVector = FirstPersonMove(playerInput, out targetSpeed);
                break;
            case CinemachineCameraManager.CinemachineCameraState.ThirdPerson:
                moveVector = ThirdPersonMove(playerInput, out targetSpeed);
                break;
            default:
                return moveVector;
        }

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
        _animator.SetBool("IsGrounded", _characterController.isGrounded);
        _animator.SetBool("IsSprinting", isSprinting);

        return moveVector;
    }

    Vector3 FirstPersonMove(PlayerInputController.PlayerInput playerInput, out float targetSpeed)
    {
        Vector3 inputVector = new Vector3(playerInput.strafe, 0, playerInput.forward);

        Camera outputCamera = CinemachineCameraManager.Instance.OutputCamera;
        Vector3 forwardDir = outputCamera.transform.forward;
        Vector3 rightDir = outputCamera.transform.right;

        forwardDir.y = 0;
        rightDir.y = 0;

//        transform.forward = forwardDir;
//        transform.right = rightDir;

        transform.LookAt(transform.position + (forwardDir * 5), Vector3.up);

        inputVector = forwardDir * playerInput.forward;
        inputVector += rightDir * playerInput.strafe;

        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }

        bool isSprinting = IsSprinting(playerInput);

        targetSpeed = 0;
        if (inputVector.magnitude > 0)
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

        return inputVector * curSpeed;
    }

    Vector3 ThirdPersonMove(PlayerInputController.PlayerInput playerInput, out float targetSpeed)
    {
//        Debug.Log(thirdPersonPlayerCamera);

        Vector3 inputVector = new Vector3(playerInput.strafe, 0, playerInput.forward);

        Camera outputCamera = CinemachineCameraManager.Instance.OutputCamera;
        Vector3 forwardDir = outputCamera.transform.forward;
        Vector3 rightDir = outputCamera.transform.right;

        forwardDir.y = 0;
        rightDir.y = 0;

        inputVector = forwardDir * playerInput.forward;
        inputVector += rightDir * playerInput.strafe;

        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }

        targetSpeed = 0;
        if (inputVector.magnitude > 0)
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

        return inputVector * curSpeed;
    }

    Vector3 AddJumpForceAsNeeded(PlayerInputController.PlayerInput playerInput)
    {
        if (Time.time - lastJumpTime < jumpForceDuration)
        {
            return (Vector3.up *
                    (jumpForce - HelperUtilities.Remap(Time.time - lastJumpTime, 0, jumpForceDuration, 0, jumpForce)) *
                    BoltNetwork.FrameDeltaTime);
        }

        if (_characterController.isGrounded && playerInput.jump)
        {
            lastJumpTime = Time.time;
        }

        return Vector3.zero;
    }

    bool IsSprinting(PlayerInputController.PlayerInput playerInput)
    {
        return (playerInput.sprint && !playerInput.aim && ((playerInput.forward > 0) ||
                                                           (!(playerInput.forward < 0) &&
                                                            Mathf.Abs(playerInput.strafe) > 0)));
    }
}