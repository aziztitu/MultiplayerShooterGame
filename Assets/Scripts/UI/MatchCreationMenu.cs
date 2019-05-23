using System;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchCreationMenu : Bolt.GlobalEventListener
{
    public GameObject arenaListContainer;
    public GameObject arenaListItemPrefab;
    
    public string arenaLobbySceneName = "Arena Lobby";
    public string sceneToGoBack = "Multiplayer Menu";

    public ArenaSettingsAsset[] arenaSettingsAssets;

    private ArenaSettingsAsset selectedArenaSettingsAsset = null;
    
    private void Awake()
    {
        if (!GameManager.Instance.isLoggedIn)
        {
            SceneManager.LoadScene(AuthMenu.authMenuSceneName);
            return;
        }

        foreach (var arenaSettingsAsset in arenaSettingsAssets)
        {
            var arenaSelectionUi = Instantiate(arenaListItemPrefab, arenaListContainer.transform).GetComponent<ArenaSelectionUI>();
            arenaSelectionUi.Init(arenaSettingsAsset);
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