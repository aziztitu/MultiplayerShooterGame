using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class LevelManager: SingletonMonoBehaviour<LevelManager>
{
    public PlayerModel LocalPlayerModel;

    public event Action<bool> OnGameOver;
    public bool GameOver { get; protected set; }

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