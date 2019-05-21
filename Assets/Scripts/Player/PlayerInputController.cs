using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerInputController : Bolt.EntityBehaviour<IPlayerState>
{
    [Serializable]
    public class PlayerInputSettings
    {
        public float lookXSensitivity = 3.0f;
        public float lookYSensitivity = 3.0f;
        public bool invertLookY = false;
    }

    [Serializable]
    public struct PlayerInput
    {
        [Header("Movement")] public float forward;
        public float strafe;
        public bool sprint;
        public bool jump;

        [Header("Combat")] public bool fire;
        public bool aim;
        
        [Header("Others")] public bool enterFlight;
    }

    [SerializeField] private PlayerInputSettings _playerInputSettings = new PlayerInputSettings();
    [ReadOnly] [SerializeField] private PlayerInput _playerInput = new PlayerInput();
    private PlayerModel _playerModel;

    private void Awake()
    {
        _playerModel = GetComponent<PlayerModel>();
    }

    void Update()
    {
    }

    public override void SimulateOwner()
    {
        if (!_playerModel.controllable)
        {
            ResetPlayerInput();
            return;
        }
        
        UpdatePlayerInput();
    }

    public PlayerInput GetPlayerInput()
    {
        return _playerInput;
    }

    void UpdatePlayerInput()
    {
        _playerInput.strafe = Input.GetAxis("Horizontal");
        _playerInput.forward = Input.GetAxis("Vertical");
        _playerInput.sprint = Input.GetButton("Sprint");
        _playerInput.jump = Input.GetButton("Jump");
        
        _playerInput.fire = Input.GetButton("Fire");
        _playerInput.aim = Input.GetButton("Aim");
        
        _playerInput.enterFlight = Input.GetButtonDown("EnterExit Flight");
    }

    void ResetPlayerInput()
    {
        _playerInput = new PlayerInput();
    }
}