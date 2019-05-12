using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;

public class PlayerCombatController : Bolt.EntityBehaviour<IPlayerState>
{
    private PlayerModel _playerModel;
    private CharacterController _characterController;
    private Animator _animator;

    void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public override void SimulateController()
    {
        base.SimulateController();
//        UpdateShooting();
    }

    public override void ExecuteCommand(Command command, bool resetState)
    {
        base.ExecuteCommand(command, resetState);

        PlayerCommand playerCommand = (PlayerCommand) command;
        if (resetState)
        {
        }
        else
        {
            UpdateShooting(playerCommand.Input);
        }
    }

    void UpdateShooting(IPlayerCommandInput playerCommandInput)
    {
        switch ((CinemachineCameraManager.CinemachineCameraState) playerCommandInput.CamState)
        {
            case CinemachineCameraManager.CinemachineCameraState.FirstPerson:
                FirstPersonShooting(playerCommandInput);
                break;
            case CinemachineCameraManager.CinemachineCameraState.ThirdPerson:
                ThirdPersonShooting(playerCommandInput);
                break;
            default:
                return;
        }

        _animator.SetBool("IsAiming", playerCommandInput.Aim);
        _animator.SetBool("IsShooting", playerCommandInput.Fire);
    }

    void FirstPersonShooting(IPlayerCommandInput playerCommandInput)
    {
    }

    void ThirdPersonShooting(IPlayerCommandInput playerCommandInput)
    {
    }
}