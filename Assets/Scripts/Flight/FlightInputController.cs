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
        public float strafeHorizontal;
        public float strafeVertical;
        public bool boost;

        [Header("Combat")] public bool fire;
        public bool aim;

        [Header("Others")] public bool exitFlight;
    }

    [SerializeField] private FlightInputSettings flightInputSettings = new FlightInputSettings();
    [ReadOnly] [SerializeField] private FlightInput flightInput = new FlightInput();
    
    private FlightModel _flightModel;

    private void Awake()
    {
        _flightModel = GetComponent<FlightModel>();
    }

    void Update()
    {
    }

    public override void SimulateOwner()
    {
        if (!_flightModel.controllable)
        {
            ResetFlightInput();
            return;
        }
        
        UpdateFlightInput();
    }

    public FlightInput GetFlightInput()
    {
        return flightInput;
    }

    void UpdateFlightInput()
    {
        flightInput.strafeHorizontal = Input.GetAxis("Horizontal");
        flightInput.forward = Input.GetAxis("Vertical");
        flightInput.strafeVertical = Input.GetAxis("StrafeVertical");
        flightInput.boost = Input.GetButton("Sprint");
        
        flightInput.fire = Input.GetButton("Fire");
        flightInput.aim = Input.GetButton("Aim");
        
        flightInput.exitFlight = Input.GetButtonDown("EnterExit Flight");
    }

    void ResetFlightInput()
    {
        flightInput = new FlightInput();
    }
}
