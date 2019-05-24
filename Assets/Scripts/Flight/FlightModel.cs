using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class FlightModel : BoltGameObjectEntity<IFlightState>, IWeaponOwner
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
    public FlightHUDController flightHudController { get; private set; }
    public InteractionController interactionController { get; private set; }
    public Health health { get; private set; }

    [CanBeNull]
    public PlayerModel controllingPlayer
    {
        get { return _controllingPlayer; }
        set { _controllingPlayer = value; }
    }

    public bool controllable
    {
        get
        {
            bool cameraIsStable = CinemachineCameraManager.Instance &&
                                  !CinemachineCameraManager.Instance.CinemachineBrain.IsBlending;

            return _controllingPlayer != null && cameraIsStable;
        }
    }

    private bool isInArena => ArenaDataManager.Instance != null;

    public int? playerId => _controllingPlayer != null ? _controllingPlayer.playerId : null;

    private Shootable _shootable;

    private void Awake()
    {
        flightInputController = GetComponent<FlightInputController>();
        flightMovementController = GetComponent<FlightMovementController>();
        flightCombatController = GetComponent<FlightCombatController>();
        flightAvatar = GetComponentInChildren<FlightAvatar>(false);
        flightHudController = GetComponentInChildren<FlightHUDController>(false);
        interactionController = GetComponentInChildren<InteractionController>();
        health = GetComponent<Health>();
        _shootable = GetComponentInChildren<Shootable>();
        
        flightAvatar.flightWeapon.AssignWeaponOwner(this);

        health.onDamageTaken += (float damage, int attackerPlayerId) =>
        {
            ArenaDataManager.Instance.PlayerAttacked(attackerPlayerId, playerId ?? -1, damage);
        };
        
        health.OnDeath.AddListener((killerPlayerId) =>
        {
            if (entity.IsOwner)
            {
                if (controllingPlayer != null)
                {
                    controllingPlayer.health.TakeDamage(controllingPlayer.health.maxhealth, killerPlayerId);
                    RevokePlayerControl();
                }

                BoltNetwork.Destroy(gameObject);
            }
        });

        _shootable.onShotEvent.AddListener(OnShot);
    }

    // Start is called before the first frame update
    void Start()
    {
        flightHudController.Show(controllingPlayer != null);
    }

    // Update is called once per frame
    public override void SimulateOwner()
    {
        base.SimulateOwner();

        if (!controllable)
        {
            return;
        }

        FlightInputController.FlightInput flightInput = flightInputController.GetFlightInput();
        if (controllingPlayer != null && flightInput.exitFlight && LevelManager.Instance.levelPlayerType !=
            LevelManager.LevelPlayerType.FlightOnly)
        {
            RevokePlayerControl();
        }
    }

    public override void Attached()
    {
        base.Attached();
        SetupState();
    }

    void SetupState()
    {
        state.SetTransforms(state.FlightTransform, transform);
//        state.SetTransforms(state.AvatarTransform, flightAvatar.transform);
    }

    private void OnShot(float damage, Vector3 hitPos, IWeaponOwner weaponOwner)
    {
        int attackerPlayerId = -1;
        if (weaponOwner != null)
        {
            attackerPlayerId = weaponOwner.playerId ?? -1;
        }
        
        health.TakeDamage(damage, attackerPlayerId);
    }

    public void RequestControl(InteractionController otherInteractionController)
    {
        PlayerModel playerModel = otherInteractionController.GetComponent<PlayerModel>();
        if (playerModel)
        {
            RequestControl(playerModel);
        }
    }

    public void RequestControl(PlayerModel playerModel)
    {
        if (controllingPlayer != null)
        {
            return;
        }

        controllingPlayer = playerModel;
        HelperUtilities.SetLayerRecursively(gameObject, LayerMask.NameToLayer("LocalPlayer"));
        playerModel.OnTakenFlightControl(this);
        flightHudController.Show();
    }

    private void RevokePlayerControl()
    {
        if (controllingPlayer == null)
        {
            return;
        }

        flightHudController.Show(false);
        HelperUtilities.SetLayerRecursively(gameObject, LayerMask.NameToLayer("Default"));
        controllingPlayer.OnFlightControlRevoked();
        controllingPlayer = null;
    }

    public void Show(bool show)
    {
        if (entity.IsAttached)
        {
            state.IsGameObjectActive = show;
        }
        else
        {
            gameObject.SetActive(show);
        }
    }
}