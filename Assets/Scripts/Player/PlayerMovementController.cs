using System.Collections;
using System.Collections.Generic;
using Bolt;
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

    public override void SimulateController()
    {
        base.SimulateController();
//        Move();
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        base.ExecuteCommand(command, resetState);

        PlayerCommand playerCommand = (PlayerCommand) command;

        if (resetState)
        {
            Debug.Log("Reset Position: ");
            Debug.Log(playerCommand.Result.Position);
            // TODO: TBI
//            transform.position = playerCommand.Result.Position;
            _characterController.Move(playerCommand.Result.Position - transform.position);
        }
        else
        {
            float targetSpeed = 0;
            Move(playerCommand.Input, out targetSpeed);

            if (playerCommand.IsFirstExecution)
            {
                bool isSprinting = IsSprinting(playerCommand.Input);

                // Move Animation
                Vector3 animVector = new Vector3(playerCommand.Input.Strafe, 0, playerCommand.Input.Forward);
                if (!isSprinting)
                {
                    animVector /= 2;
                }

                float curSpeedFactor = HelperUtilities.Remap(curSpeed, 0, targetSpeed, 0, 1);
                animVector *= curSpeedFactor;

                Debug.Log("Strafe: " + animVector.x);
                Debug.Log("Forward: " + animVector.z);

                _animator.SetFloat("Strafe", animVector.x);
                _animator.SetFloat("Forward", animVector.z);
//        _animator.SetBool("IsMoving", curSpeedFactor > 0);
                _animator.SetBool("IsGrounded", true);
                _animator.SetBool("IsSprinting", isSprinting);
            }

            playerCommand.Result.Position = transform.position;
//            playerCommand.Result.Velocity = _characterController.velocity;
        }
    }

    void ApplyGravityIfNeeded()
    {
    }

    void Move(IPlayerCommandInput playerCommandInput, out float targetSpeed)
    {
        targetSpeed = 0;
        switch ((CinemachineCameraManager.CinemachineCameraState) playerCommandInput.CamState)
        {
            case CinemachineCameraManager.CinemachineCameraState.FirstPerson:
                FirstPersonMove(playerCommandInput, out targetSpeed);
                break;
            case CinemachineCameraManager.CinemachineCameraState.ThirdPerson:
                ThirdPersonMove(playerCommandInput, out targetSpeed);
                break;
            default:
                return;
        }
    }

    void FirstPersonMove(IPlayerCommandInput playerCommandInput, out float targetSpeed)
    {
        Vector3 moveVector = new Vector3(playerCommandInput.Strafe, 0, playerCommandInput.Forward);

        Vector3 forwardDir = playerCommandInput.CamForward;
        Vector3 rightDir = playerCommandInput.CamRight;

        forwardDir.y = 0;
        rightDir.y = 0;

//        transform.forward = forwardDir;
//        transform.right = rightDir;

        transform.LookAt(transform.position + (forwardDir * 5), Vector3.up);

        moveVector = forwardDir * playerCommandInput.Forward;
        moveVector += rightDir * playerCommandInput.Strafe;

        if (moveVector.magnitude > 1)
        {
            moveVector.Normalize();
        }

        bool isSprinting = IsSprinting(playerCommandInput);

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

    void ThirdPersonMove(IPlayerCommandInput playerCommandInput, out float targetSpeed)
    {
//        Debug.Log(thirdPersonPlayerCamera);


        Vector3 moveVector = new Vector3(playerCommandInput.Strafe, 0, playerCommandInput.Forward);

        Vector3 forwardDir = playerCommandInput.CamForward;
        Vector3 rightDir = playerCommandInput.CamRight;

        forwardDir.y = 0;
        rightDir.y = 0;

        moveVector = forwardDir * playerCommandInput.Forward;
        moveVector += rightDir * playerCommandInput.Strafe;

        if (moveVector.magnitude > 1)
        {
            moveVector.Normalize();
        }

        bool isSprinting = IsSprinting(playerCommandInput);

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

    bool IsSprinting(IPlayerCommandInput playerCommandInput)
    {
        return (playerCommandInput.Sprint && !playerCommandInput.Aim && ((playerCommandInput.Forward > 0) ||
                                                                         (!(playerCommandInput.Forward < 0) &&
                                                                          Mathf.Abs(playerCommandInput.Strafe) > 0)));
    }
}