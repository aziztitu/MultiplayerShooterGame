using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightCombatController : Bolt.EntityBehaviour<IFlightState>
{
    private FlightModel _flightModel;
    
    private void Awake()
    {
        _flightModel = GetComponent<FlightModel>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public override void SimulateOwner()
    {
        base.SimulateOwner();
        
        if (!_flightModel.controllable)
        {
            return;
        }
        
        if (LevelManager.Instance.interactingWithUI)
            return;
        
        FlightInputController.FlightInput flightInput = _flightModel.flightInputController.GetFlightInput();
        UpdateShooting(flightInput);
    }


    void UpdateShooting(FlightInputController.FlightInput flightInput)
    {
        if (flightInput.fire)
        {
            _flightModel.flightAvatar.flightWeapon.Shoot();
        }
    }
}
