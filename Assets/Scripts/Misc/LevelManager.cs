using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LevelManager : SingletonMonoBehaviour<LevelManager>
{
    [ReadOnly] public bool interactingWithUI = false;

    public event Action OnLocalPlayerModelChanged;
    public event Action<bool> OnGameOver;
    public bool GameOver { get; protected set; }

    [SerializeField] private PlayerModel _localPlayerModel;

    public PlayerModel LocalPlayerModel
    {
        get { return _localPlayerModel; }
        set
        {
            _localPlayerModel = value;
            OnLocalPlayerModelChanged?.Invoke();
        }
    }

    public Transform thirdPersonCameraTarget
    {
        get
        {
            if (_localPlayerModel != null)
            {
                return _localPlayerModel.flightModelInControl != null
                    ? _localPlayerModel.flightModelInControl.thirdPersonCamTarget
                    : _localPlayerModel.thirdPersonCamTarget;
            }

            return null;
        }
    }
    
    public Transform thirdPersonCameraFollow
    {
        get
        {
            if (_localPlayerModel != null)
            {
                return _localPlayerModel.flightModelInControl != null
                    ? _localPlayerModel.flightModelInControl.thirdPersonCamFollow
                    : _localPlayerModel.firstPersonCamFollow;
            }

            return null;
        }
    }

    public Transform firstPersonCameraFollow =>
        _localPlayerModel != null ? _localPlayerModel.firstPersonCamFollow : null;

    new void Awake()
    {
        base.Awake();
        Time.timeScale = 1;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    protected void InvokeOnGameOver(bool data)
    {
        OnGameOver?.Invoke(data);
    }
}