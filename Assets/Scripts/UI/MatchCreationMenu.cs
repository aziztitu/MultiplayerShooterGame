using System;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchCreationMenu : Bolt.GlobalEventListener
{
    public string sceneToGoBack = "Multiplayer Menu";

    private string selectedScene = "";
    
    private void Awake()
    {
        var arenaSelectionUis = GetComponentsInChildren<ArenaSelectionUI>();
        foreach (ArenaSelectionUI arenaSelectionUi in arenaSelectionUis)
        {
            arenaSelectionUi.OnArenaSelected += LoadArenaAsServer;
        }
        
        HelperUtilities.UpdateCursorLock(false);
        if (BoltNetwork.IsRunning)
        {
            BoltLauncher.Shutdown();
        }
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            string matchName = Guid.NewGuid().ToString();

            BoltNetwork.SetServerInfo(matchName, null);
            BoltNetwork.LoadScene(selectedScene);
        }
    }

    void LoadArenaAsServer(string arenaSceneName)
    {
        selectedScene = arenaSceneName;
        BoltLauncher.StartServer();
    }

    public void GoBack()
    {
        SceneManager.LoadScene(sceneToGoBack);
    }
}