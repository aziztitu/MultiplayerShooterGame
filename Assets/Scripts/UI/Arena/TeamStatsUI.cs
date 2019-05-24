using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamStatsUI : MonoBehaviour
{
    public TextMeshProUGUI teamScoreText;
    public TextMeshProUGUI teamNameText;
    public Image teamScoreBG;
    public Image teamHeaderBorder;

    public GameObject playerStatsUIPrefab;
    public Transform playerListContainer;
    
    [SerializeField]
    private ArenaDataManager.ArenaTeamInfo teamInfo;

    private void Awake()
    {
        ArenaDataManager.AddOnReadyListener(() =>
        {
            ArenaDataManager.Instance.OnTeamInfoChanged += OnTeamInfoChanged;
        }, true);
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
            Debug.Log("Team Color: " + teamInfo.teamColor);
            
            teamNameText.text = teamInfo.teamName;
            teamScoreText.text = teamInfo.kills.ToString();
            teamScoreBG.color = teamInfo.teamColor;
            teamHeaderBorder.color = teamInfo.teamColor;
            
            foreach (var playerInfo in teamInfo.arenaPlayerInfos)
            {
                var playerStatsUi = Instantiate(playerStatsUIPrefab, playerListContainer.transform)
                    .GetComponent<PlayerStatsUI>();

                playerStatsUi.SetPlayerInfo(playerInfo);
                Debug.Log("Refreshing player: " + playerInfo.playerName);
            }
        }
    }

    private void OnDestroy()
    {
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