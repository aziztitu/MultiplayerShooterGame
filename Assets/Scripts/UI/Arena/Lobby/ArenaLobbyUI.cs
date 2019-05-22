using System.Collections.Generic;
using System.Linq;
using BasicTools.ButtonInspector;
using UnityEngine;
using UnityEngine.UI;

public class ArenaLobbyUI : MonoBehaviour
{
    public GameObject teamListContainer;
    public GameObject serverTools;
    public GameObject arenaLobbyTeamUIPrefab;

    private List<ArenaLobbyTeamUI> teamUIList = new List<ArenaLobbyTeamUI>();
    private bool initializedListeners = false;

    [Button("Refresh All Teams", "RefreshAllTeams")]
    public bool refreshAllItems_Btn;

    private void Awake()
    {
        ArenaDataManager.AddOnReadyListener(() =>
        {
            ArenaDataManager.Instance.OnTeamInfoChanged += (changedTeamInfo) =>
            {
                if (changedTeamInfo == ArenaDataManager.UnassignedTeamId)
                {
                    RefreshUnassigned();
                }
            };
            
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
        });

        serverTools.SetActive(BoltNetwork.IsServer);
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

    public void StartGame()
    {
        if (BoltNetwork.IsServer)
        {
            BoltNetwork.LoadScene(ArenaDataManager.Instance.arenaSettingsAsset.arenaSceneName);
        }
    }
}