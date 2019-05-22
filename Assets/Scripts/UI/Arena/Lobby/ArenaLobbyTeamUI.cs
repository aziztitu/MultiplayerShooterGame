using TMPro;
using UnityEngine;

public class ArenaLobbyTeamUI : MonoBehaviour
{
    public TextMeshProUGUI teamNameText;
    public GameObject playerListContainer;
    public GameObject playerInfoPrefab;

    private ArenaDataManager.ArenaTeamInfo teamInfo;

    private void Awake()
    {
        ArenaDataManager.AddOnReadyListener(OnArenaDataManagerReady);
    }

    void OnArenaDataManagerReady()
    {
        ArenaDataManager.Instance.OnTeamInfoChanged += OnTeamInfoChanged;
    }

    public void SetTeamInfo(ArenaDataManager.ArenaTeamInfo newTeamInfo)
    {
        teamInfo = newTeamInfo;
        Refresh();
    }

    public void Refresh()
    {
        HelperUtilities.DestroyAllChildObjects(playerListContainer.transform);

        if (teamInfo != null)
        {
            Debug.Log("Refreshing Team: " + teamInfo.teamId);
            teamNameText.text = teamInfo.teamName;
            foreach (var playerInfo in teamInfo.arenaPlayerInfos)
            {
                var playerInfoItemUi = Instantiate(playerInfoPrefab, playerListContainer.transform)
                    .GetComponent<ArenaLobbyPlayerInfoItemUI>();

                playerInfoItemUi.SetPlayerInfo(playerInfo);
                Debug.Log("Refreshing player: " + playerInfo.playerName);
            }
        }
    }

    private void OnDestroy()
    {
        ArenaDataManager.RemoveOnReadyListener(OnArenaDataManagerReady);
        ArenaDataManager.Instance.OnTeamInfoChanged -= OnTeamInfoChanged; 
    }

    void OnTeamInfoChanged(int changedTeamId)
    {
        if (teamInfo != null && changedTeamId == teamInfo.teamId)
        {
            teamInfo = ArenaDataManager.Instance.arenaTeamInfos[changedTeamId];
            Refresh();
        }
    }
}