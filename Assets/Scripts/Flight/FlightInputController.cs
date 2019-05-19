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
        if (_flightModel.controllingPlayer == null)
        {
            ResetMoveInput();
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
        UpdateMoveInput();
    }

    void ResetMoveInput()
    {
        flightInput = new FlightInput();
    }

    void UpdateMoveInput()
    {
        flightInput.strafeHorizontal = Input.GetAxis("Horizontal");
        flightInput.forward = Input.GetAxis("Vertical");
        flightInput.strafeVertical = Input.GetAxis("StrafeVertical");
        flightInput.boost = Input.GetButton("Sprint");
        flightInput.fire = Input.GetButton("Fire");
        flightInput.aim = Input.GetButton("Aim");
    }
}
