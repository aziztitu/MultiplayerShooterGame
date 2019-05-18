using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightMovementController : Bolt.EntityBehaviour<IFlightState>
{
    public float maxSpeed = 20;
    public float acceleration = 5;
    public float deceleration = 5;
    public float alignToCameraSpeed = 10;
    public bool enableVerticalMovement = false;

    public float engineTrailChangeSpeed = 5;

    [SerializeField] [ReadOnly] private float curSpeed = 0;
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
        UpdateEngineTrails();
    }

    void FixedUpdate()
    {
        ResetVelocity();

        if (LevelManager.Instance.interactingWithUI)
            return;

        if (entity.IsOwner)
        {
            FlightInputController.FlightInput flightInput = _flightModel.flightInputController.GetFlightInput();
//            isSprinting = IsSprinting(flightInput);

            Move(flightInput);
        }
    }


    void Move(FlightInputController.FlightInput flightInput)
    {
        Vector3 inputVector = new Vector3(flightInput.strafe, 0, flightInput.forward);

        Camera outputCamera = CinemachineCameraManager.Instance.OutputCamera;
        transform.forward = Vector3.Lerp(transform.forward, outputCamera.transform.forward,
            BoltNetwork.FrameDeltaTime * alignToCameraSpeed);

        Vector3 forwardDir = transform.forward;
        Vector3 rightDir = transform.right;

        if (!enableVerticalMovement)
        {
            forwardDir.y = 0;
            rightDir.y = 0;
        }

        inputVector = forwardDir * flightInput.forward;
        inputVector += rightDir * flightInput.strafe;

        if (inputVector.magnitude > 1)
        {
            inputVector.Normalize();
        }

        float targetSpeed = 0;
        if (inputVector.magnitude > 0)
        {
            targetSpeed = maxSpeed;
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
    }

    void ResetVelocity()
    {
        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    void UpdateEngineTrails()
    {
        Vector3 newLocalScale = _flightModel.flightAvatar.trailObject.transform.localScale;
        newLocalScale.z = Mathf.Lerp(newLocalScale.z, HelperUtilities.Remap01(curSpeed, 0, maxSpeed),
            Time.deltaTime * engineTrailChangeSpeed);

        _flightModel.flightAvatar.trailObject.transform.localScale = newLocalScale;
    }
}