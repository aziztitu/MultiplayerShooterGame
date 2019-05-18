using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightInputController: Bolt.EntityBehaviour<IFlightState>
{
    [Serializable]
    public class FlightInputSettings
    {
        public float lookXSensitivity = 3.0f;
        public float lookYSensitivity = 3.0f;
        public bool invertLookY = false;
    }

    [Serializable]
    public struct FlightInput
    {
        [Header("Movement")] public float forward;
        public float strafe;
        public bool sprint;

        [Header("Combat")] public bool fire;
        public bool aim;
    }

    [SerializeField] private FlightInputSettings flightInputSettings = new FlightInputSettings();
    [ReadOnly] [SerializeField] private FlightInput flightInput = new FlightInput();

    void Update()
    {
    }

    public override void SimulateOwner()
    {
        UpdateFlightInput();
    }

    public FlightInput GetFlightInput()
    {
        return flightInput;
    }

    void UpdateFlightInput()
    {
        UpdateMoveInput();
    }

    void UpdateMoveInput()
    {
        flightInput.strafe = Input.GetAxis("Horizontal");
        flightInput.forward = Input.GetAxis("Vertical");
        flightInput.sprint = Input.GetButton("Sprint");
        flightInput.fire = Input.GetButton("Fire");
        flightInput.aim = Input.GetButton("Aim");
    }
}
