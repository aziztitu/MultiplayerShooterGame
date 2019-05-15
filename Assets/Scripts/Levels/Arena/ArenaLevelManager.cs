using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaLevelManager : LevelManager
{
    public GameObject CinemachineCameraRigPrefab;

    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject FadeOut;

    public string LobbySceneName = "LobbyMenu";
    public string EndGameCreditsSceneName = "EndCredits";

    public List<Transform> SpawnPoints;

    public new static ArenaLevelManager Instance => Get<ArenaLevelManager>();

    private new void Awake()
    {
        base.Awake();
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

    IEnumerator GoToLobbyAfterSecs(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(LobbySceneName);
    }
}