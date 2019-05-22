using System;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchCreationMenu : Bolt.GlobalEventListener
{
    public string arenaLobbySceneName = "Arena Lobby";
    public string sceneToGoBack = "Multiplayer Menu";

    private ArenaSettingsAsset selectedArenaSettingsAsset = null;

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
            InitializeArenaLobby();
        }
    }

    void InitializeArenaLobby()
    {
        string matchName = Guid.NewGuid().ToString();

        BoltNetwork.SetServerInfo(matchName, null);

        if (ArenaDataManager.Instance != null)
        {
            DestroyImmediate(ArenaDataManager.Instance.gameObject);
        }

        var arenaDataManager = BoltNetwork.Instantiate(BoltPrefabs.Arena_Data_Manager).GetComponent<ArenaDataManager>();
        arenaDataManager.Initialize(selectedArenaSettingsAsset);

        BoltNetwork.LoadScene(arenaLobbySceneName);
    }

    void LoadArenaAsServer(ArenaSettingsAsset arenaSettingsAsset)
    {
        selectedArenaSettingsAsset = arenaSettingsAsset;
        BoltLauncher.StartServer();
    }

    public void GoBack()
    {
        SceneManager.LoadScene(sceneToGoBack);
    }
}