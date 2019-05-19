using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class FlightModel : Bolt.EntityBehaviour<IFlightState>
{
    public Transform thirdPersonCamTarget;
    public Transform thirdPersonCamFollow;
//    public Transform firstPersonCamTransform;

    [Header("During Test Scenes Only")] [SerializeField] [CanBeNull]
    private PlayerModel _controllingPlayer;

    public FlightInputController flightInputController { get; private set; }
    public FlightMovementController flightMovementController { get; private set; }
    public FlightCombatController flightCombatController { get; private set; }
    public FlightAvatar flightAvatar { get; private set; }

    [CanBeNull]
    public PlayerModel controllingPlayer
    {
        get { return _controllingPlayer; }
        set { _controllingPlayer = value; }
    }

    private void Awake()
    {
        flightInputController = GetComponent<FlightInputController>();
        flightMovementController = GetComponent<FlightMovementController>();
        flightCombatController = GetComponent<FlightCombatController>();
        flightAvatar = GetComponentInChildren<FlightAvatar>(false);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void Attached()
    {
        base.Attached();
        SetupState();
    }

    void SetupState()
    {
        state.SetTransforms(state.FlightTransform, transform);
    }
}