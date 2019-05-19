using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightMovementController : Bolt.EntityBehaviour<IFlightState>
{
    public float maxRegularSpeed = 1;
    public float maxBoostSpeed = 3;
    public float acceleration = 5;
    public float deceleration = 5;
    public float alignToCameraSpeed = 10;
    public float maxStrafeRoll = 45;
    public float strafeRollSpeed = 5;
    public bool enableVerticalMovement = false;

    [Header("Trail Settings")] public float engineTrailChangeSpeed = 5;
    public float restEngineTrailLength = 0.4f;
    public float regularEngineTrailLength = 1f;
    public float boostEngineTrailLength = 1.4f;

    [Header("Debug Info")] [SerializeField] [ReadOnly]
    private float curSpeed = 0;

    private float targetSpeed = 0;
    private FlightModel _flightModel;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _flightModel = GetComponent<FlightModel>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (_flightModel.controllingPlayer == null)
        {
            
        }
        
        FlightInputController.FlightInput flightInput = _flightModel.flightInputController.GetFlightInput();
        UpdateEngineTrails(flightInput);
    }

    void FixedUpdate()
    {
        ResetVelocity();
        
        if (_flightModel.controllingPlayer == null)
        {
            return;
        }

        if (LevelManager.Instance.interactingWithUI)
            return;

        if (entity.IsOwner)
        {
            FlightInputController.FlightInput flightInput = _flightModel.flightInputController.GetFlightInput();
            Move(flightInput);
        }
    }


    void Move(FlightInputController.FlightInput flightInput)
    {
        Vector3 inputVector = new Vector3(flightInput.strafeHorizontal, 0, flightInput.forward);

        Camera outputCamera = CinemachineCameraManager.Instance.OutputCamera;
        transform.forward = Vector3.Lerp(transform.forward, outputCamera.transform.forward,
            BoltNetwork.FrameDeltaTime * alignToCameraSpeed);
        transform.LookAt(transform.position + (transform.forward * 5), outputCamera.transform.up);

        Vector3 forwardDir = transform.forward;
        Vector3 rightDir = transform.right;
        Vector3 upDir = transform.up;

        if (!enableVerticalMovement)
        {
            forwardDir.y = 0;
            rightDir.y = 0;
        }

        inputVector = forwardDir * flightInput.forward;
        inputVector += rightDir * flightInput.strafeHorizontal;
        inputVector += upDir * flightInput.strafeVertical;

        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }

        targetSpeed = 0;
        if (inputVector.magnitude > 0)
        {
            targetSpeed = flightInput.boost ? maxBoostSpeed : maxRegularSpeed;
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

        transform.position += inputVector * curSpeed;

        float roll = -flightInput.strafeHorizontal * maxStrafeRoll;
        Quaternion avatarLocalRot = _flightModel.flightAvatar.transform.localRotation;
        avatarLocalRot = Quaternion.Euler(avatarLocalRot.x, avatarLocalRot.y, roll);
        _flightModel.flightAvatar.transform.localRotation =
            Quaternion.Slerp(_flightModel.flightAvatar.transform.localRotation, avatarLocalRot,
                Time.deltaTime * strafeRollSpeed);
    }

    void ResetVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    void UpdateEngineTrails(FlightInputController.FlightInput flightInput)
    {
        Vector3 newLocalScale = _flightModel.flightAvatar.trailObject.transform.localScale;
        newLocalScale.z = Mathf.Lerp(newLocalScale.z,
            HelperUtilities.Remap(curSpeed, 0, flightInput.boost ? maxBoostSpeed : maxRegularSpeed,
                restEngineTrailLength,
                flightInput.boost ? boostEngineTrailLength : regularEngineTrailLength),
            Time.deltaTime * engineTrailChangeSpeed);

        _flightModel.flightAvatar.trailObject.transform.localScale = newLocalScale;
    }
}