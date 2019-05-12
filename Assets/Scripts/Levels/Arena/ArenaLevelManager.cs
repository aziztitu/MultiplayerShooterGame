using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaLevelManager: LevelManager
{
    public GameObject CinemachineCameraRigPrefab;
    
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject FadeOut; 
    
    public string MainMenuSceneName = "MainMenu";
    public string EndGameCreditsSceneName = "EndCredits";
    
    public List<Transform> SpawnPoints;

    public new static ArenaLevelManager Instance => Get<ArenaLevelManager>();

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

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MainMenuSceneName);
    }
}