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

    public override void SimulateOwner()
    {
        base.SimulateOwner();
        
        PlayerInputController.PlayerInput playerInput = _playerModel.playerInputController.GetPlayerInput();
        UpdateShooting(playerInput);
    }


    void UpdateShooting(PlayerInputController.PlayerInput playerInput)
    {
        switch (CinemachineCameraManager.Instance.CurrentState)
        {
            case CinemachineCameraManager.CinemachineCameraState.FirstPerson:
                FirstPersonShooting(playerInput);
                break;
            case CinemachineCameraManager.CinemachineCameraState.ThirdPerson:
                ThirdPersonShooting(playerInput);
                break;
            default:
                return;
        }

        _animator.SetBool("IsAiming", playerInput.aim);
        _animator.SetBool("IsShooting", playerInput.fire);
    }

    void FirstPersonShooting(PlayerInputController.PlayerInput playerInput)
    {
    }

    void ThirdPersonShooting(PlayerInputController.PlayerInput playerInput)
    {
    }
}