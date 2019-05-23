using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaLevelManager : LevelManager
{
    public const int maxTeamsAllowed = 2;
    
    public ArenaSettingsAsset arenaSettingsAsset;
    
    public GameObject CinemachineCameraRigPrefab;

    public ArenaMenu arenaMenu;
    
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject FadeOut;

    public string MultiplayerSceneName = "Multiplayer Menu";
    public string EndGameCreditsSceneName = "EndCredits";
    
    public List<ArenaTeamConfig> teamConfigList = new List<ArenaTeamConfig>();

    public new static ArenaLevelManager Instance => Get<ArenaLevelManager>();

    private new void Awake()
    {
        base.Awake();
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

    private void OnValidate()
    {
        if (arenaSettingsAsset)
        {
            if (arenaSettingsAsset.teams.Length > maxTeamsAllowed)
            {
                Debug.LogWarning($"Only a maximum of {maxTeamsAllowed} teams is supported now");
            }

            int numTeams = Math.Min(arenaSettingsAsset.teams.Length, maxTeamsAllowed);

            if (teamConfigList.Count < numTeams)
            {
                for (int i = 0; i < numTeams - teamConfigList.Count; i++)
                {
                    teamConfigList.Add(null);
                }
            }
            else if (teamConfigList.Count > numTeams)
            {
                teamConfigList.RemoveRange(numTeams, teamConfigList.Count - numTeams);
            }
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
            LocalPlayerModel.DestroyPlayer();
        }
        
        // TODO: Implement spawning in one of the team spawn points
        /*Transform spawnPoint = _spawnPointsRandomizer.GetRandomItem();
        if (spawnPoint != null)
        {
            ArenaCallbacks.SpawnPlayer(spawnPoint.transform.position, spawnPoint.transform.rotation);
            CinemachineCameraManager.Instance.SwitchCameraState(CinemachineCameraManager.CinemachineCameraState.FirstPerson);
        }*/
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