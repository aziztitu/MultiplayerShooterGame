using System.Collections;
using System.Collections.Generic;
using Bolt;
using Cinemachine;
using UnityEngine;

public class PlayerCombatController : Bolt.EntityBehaviour<IPlayerState>
{
    private PlayerModel _playerModel;
    private CharacterController _characterController;
    private Animator _animator;

    private RangeWeapon rangeWeapon;

    void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponentInChildren<Animator>();
        rangeWeapon = GetComponentInChildren<RangeWeapon>();
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
        bool shotFired = false;
        switch (CinemachineCameraManager.Instance.CurrentState)
        {
            case CinemachineCameraManager.CinemachineCameraState.FirstPerson:
                FirstPersonShooting(playerInput, out shotFired);
                break;
            case CinemachineCameraManager.CinemachineCameraState.ThirdPerson:
                ThirdPersonShooting(playerInput, out shotFired);
                break;
            default:
                return;
        }

        _animator.SetBool("IsAiming", playerInput.aim);
        _animator.SetBool("IsShooting", playerInput.fire && shotFired);
    }

    void FirstPersonShooting(PlayerInputController.PlayerInput playerInput, out bool shotFired)
    {
        shotFired = false;
        
        if (playerInput.aim)
        {
            
        }
        else
        {
            
        }
        
        if (playerInput.fire)
        {
            shotFired = rangeWeapon.Shoot();
        }
    }

    void ThirdPersonShooting(PlayerInputController.PlayerInput playerInput, out bool shotFired)
    {
        shotFired = false;
        
        if (playerInput.fire)
        {
            shotFired = rangeWeapon.Shoot();
        }
    }
}