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
        ArenaDataManager.AddOnReadyListener(() =>
        {
            ArenaDataManager.Instance.OnTeamInfoChanged += (changedTeamId) =>
            {
                if (teamInfo != null && changedTeamId == teamInfo.teamId)
                {
                    Refresh();
                }
            };
        });
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
}