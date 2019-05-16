using System;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerMenu : Bolt.GlobalEventListener
{
    public string matchCreationScene = "Match Creation Menu";
    public string matchJoiningScene = "Match Joining Menu";
    
    private void Awake()
    {
        HelperUtilities.UpdateCursorLock(false);
        if (BoltNetwork.IsRunning)
        {
            BoltLauncher.Shutdown();
        }
    }
    
    public void CreateMatch()
    {
        SceneManager.LoadScene(matchCreationScene);
    }

    public void FindMatch()
    {
        SceneManager.LoadScene(matchJoiningScene);
    }
}