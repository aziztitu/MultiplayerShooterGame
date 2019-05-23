using System;
using System.Collections.Generic;
using System.Linq;
using BasicTools.ButtonInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ArenaLobbyUI : MonoBehaviour
{
    public GameObject teamListContainer;
    public GameObject serverTools;
    public GameObject arenaLobbyTeamUIPrefab;
    public string sceneToGoBack_Server = "";
    public string sceneToGoBack_Client = "";

    private List<ArenaLobbyTeamUI> teamUIList = new List<ArenaLobbyTeamUI>();
    private bool initializedListeners = false;

    [Button("Refresh All Teams", "RefreshAllTeams")]
    public bool refreshAllItems_Btn;

    private void Awake()
    {
        ArenaDataManager.AddOnReadyListener(OnArenaDataManagerReady);

        serverTools.SetActive(BoltNetwork.IsServer);
    }

    private void OnDestroy()
    {
        ArenaDataManager.RemoveOnReadyListener(OnArenaDataManagerReady);
        ArenaDataManager.Instance.OnTeamInfoChanged -= OnTeamInfoChanged;
        if (!BoltNetwork.IsServer)
        {
            ArenaDataManager.Instance.OnTeamInfosRefreshed -= RefreshAllTeams;
            ArenaDataManager.Instance.OnUnassignedPlayersRefreshed -= RefreshUnassigned;
        }
    }

    void Reset()
    {
        foreach (var teamUi in teamUIList)
        {
            Destroy(teamUi.gameObject);
        }

        foreach (var teamInfo in ArenaDataManager.Instance.arenaTeamInfos)
        {
            var teamUI = Instantiate(arenaLobbyTeamUIPrefab, teamListContainer.transform)
                .GetComponent<ArenaLobbyTeamUI>();

            teamUI.SetTeamInfo(teamInfo);
            teamUIList.Add(teamUI);
        }
    }

    private void RefreshAllTeams()
    {
        if (teamUIList.Count != ArenaDataManager.Instance.arenaTeamInfos.Count)
        {
            Reset();
        }
        else
        {
            for (int i = 0; i < teamUIList.Count; i++)
            {
                var arenaLobbyTeamUi = teamUIList[i];
                arenaLobbyTeamUi.SetTeamInfo(ArenaDataManager.Instance.arenaTeamInfos[i]);
            }
        }
    }

    private void RefreshUnassigned()
    {
        // TODO
    }

    public void GoBack()
    {
        SceneManager.LoadScene(BoltNetwork.IsServer ? sceneToGoBack_Server : sceneToGoBack_Client);
    }

    public void StartGame()
    {
        if (BoltNetwork.IsServer)
        {
            string matchName = Guid.NewGuid().ToString();
            BoltNetwork.SetServerInfo(matchName, new ArenaLobby.RoomInfo()
            {
                isAccepting = false
            });

            ArenaDataManager.Instance.DisconnectUnassignedPlayers();
            BoltNetwork.LoadScene(ArenaDataManager.Instance.arenaSettingsAsset.arenaSceneName);
        }
    }

    void OnTeamInfoChanged(int changedTeamInfo)
    {
        if (changedTeamInfo == ArenaDataManager.UnassignedTeamId)
        {
            RefreshUnassigned();
        }
    }

    void OnArenaDataManagerReady()
    {
        ArenaDataManager.Instance.OnTeamInfoChanged += OnTeamInfoChanged;

        if (BoltNetwork.IsServer)
        {
            Reset();
        }
        else
        {
            ArenaDataManager.Instance.OnTeamInfosRefreshed += RefreshAllTeams;
            ArenaDataManager.Instance.OnUnassignedPlayersRefreshed += RefreshUnassigned;

            RefreshAllTeams();
        }
    }
}