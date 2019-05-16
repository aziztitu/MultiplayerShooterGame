using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaLevelManager : LevelManager
{
    public GameObject CinemachineCameraRigPrefab;

    public ArenaMenu arenaMenu;
    
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject FadeOut;

    public string MultiplayerSceneName = "Multiplayer Menu";
    public string EndGameCreditsSceneName = "EndCredits";

    public List<Transform> SpawnPoints;
    
    private Randomizer<Transform> _spawnPointsRandomizer;

    public new static ArenaLevelManager Instance => Get<ArenaLevelManager>();

    private new void Awake()
    {
        base.Awake();
        _spawnPointsRandomizer = new Randomizer<Transform>(ArenaLevelManager.Instance.SpawnPoints);
        arenaMenu.ShowHide(false);

        SetupPlayerHooks();
        OnLocalPlayerModelChanged += SetupPlayerHooks;
    }

    private void SetupPlayerHooks()
    {
        if (LocalPlayerModel)
        {
            LocalPlayerModel.health.OnDeath.AddListener(() => { arenaMenu.ShowHide(true); });
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            arenaMenu.ToggleShowHide();
        }
    }

    IEnumerator GameLost()
    {
        GameOver = true;

        Debug.Log("You Lost");
        InvokeOnGameOver(false);

        Time.timeScale = 0;
        FadeOut.SetActive(true);

        yield return new WaitForSecondsRealtime(1);

        HelperUtilities.UpdateCursorLock(false);
        LoseScreen.SetActive(true);
        FadeOut.SetActive(false);
    }

    IEnumerator GameWon()
    {
        GameOver = true;

        Debug.Log("You Won");
        InvokeOnGameOver(true);

        Time.timeScale = 0;
        FadeOut.SetActive(true);

        yield return new WaitForSecondsRealtime(1);

        SceneManager.LoadScene(EndGameCreditsSceneName);
    }

    public void GoToLobby(float delay)
    {
        StartCoroutine(GoToLobbyAfterSecs(delay));
    }

    public void RespawnLocalPlayer()
    {
        if (LocalPlayerModel != null)
        {
            BoltNetwork.Destroy(LocalPlayerModel.gameObject);
        }
        
        Transform spawnPoint = _spawnPointsRandomizer.GetRandomItem();
        if (spawnPoint != null)
        {
            ArenaCallbacks.SpawnPlayer(spawnPoint.transform.position, spawnPoint.transform.rotation);
            CinemachineCameraManager.Instance.SwitchCameraState(CinemachineCameraManager.CinemachineCameraState.FirstPerson);
        }
    }

    IEnumerator GoToLobbyAfterSecs(float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        SceneManager.LoadScene(MultiplayerSceneName);
    }
}