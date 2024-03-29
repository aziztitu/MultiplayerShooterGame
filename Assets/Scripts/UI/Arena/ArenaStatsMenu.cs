using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArenaStatsMenu : MonoBehaviour
{
    public Image titleBG;
    public TextMeshProUGUI titleText;
    public Image titleBorder;
    public GameObject teamStatsUIPrefab;
    public Transform teamListContainer;

    public bool stayOn => ArenaLevelManager.Instance.roundEnded;

    private List<TeamStatsUI> teamStatsUIList = new List<TeamStatsUI>();

    private void Awake()
    {
        ArenaDataManager.AddOnReadyListener(() =>
        {
            if (BoltNetwork.IsServer)
            {
                ArenaDataManager.Instance.OnTeamInfoChanged += OnTeamInfoChanged;
            }
            else
            {
                ArenaDataManager.Instance.OnAllTeamInfosRefreshed += RefreshAllTeams;
                ArenaDataManager.Instance.OnUnassignedPlayersRefreshed += RefreshUnassigned;
            }

            RefreshAllTeams();
        }, true);
    }

    private void OnDestroy()
    {
        ArenaDataManager.Instance.OnTeamInfoChanged -= OnTeamInfoChanged;
        if (!BoltNetwork.IsServer)
        {
            ArenaDataManager.Instance.OnAllTeamInfosRefreshed -= RefreshAllTeams;
            ArenaDataManager.Instance.OnUnassignedPlayersRefreshed -= RefreshUnassigned;
        }
    }

    private void OnEnable()
    {
//        HelperUtilities.UpdateCursorLock(false);
        if (LevelManager.Instance)
        {
//            LevelManager.Instance.interactingWithUI = true;
        }
    }

    private void OnDisable()
    {
//        HelperUtilities.UpdateCursorLock(true);
        if (LevelManager.Instance)
        {
//            LevelManager.Instance.interactingWithUI = false;
        }
    }

    void Reset()
    {
        foreach (var teamStatsUi in teamStatsUIList)
        {
            Destroy(teamStatsUi.gameObject);
        }

        foreach (var teamInfo in ArenaDataManager.Instance.arenaTeamInfos)
        {
            var teamStatsUI = Instantiate(teamStatsUIPrefab, teamListContainer.transform)
                .GetComponent<TeamStatsUI>();

            teamStatsUI.SetTeamInfo(teamInfo);
            teamStatsUIList.Add(teamStatsUI);
        }
    }

    private void RefreshAllTeams()
    {
        if (teamStatsUIList.Count != ArenaDataManager.Instance.arenaTeamInfos.Count)
        {
            Reset();
        }
        else
        {
            for (int i = 0; i < teamStatsUIList.Count; i++)
            {
                var teamStatsUi = teamStatsUIList[i];
                teamStatsUi.SetTeamInfo(ArenaDataManager.Instance.arenaTeamInfos[i]);
            }
        }
    }

    private void RefreshUnassigned()
    {
        // TODO
    }

    void OnTeamInfoChanged(int teamId)
    {
        if (teamId == ArenaDataManager.UnassignedTeamId)
        {
            RefreshUnassigned();
        }
    }
    
    public void EnableEndGameMode(int winnerTeamId)
    {
        var winnerTeamInfo = ArenaDataManager.Instance.GetArenaTeamInfo(winnerTeamId);
        titleText.text = $"{winnerTeamInfo.teamName} Wins";
        titleBG.color = winnerTeamInfo.teamColor;
        titleBorder.color = winnerTeamInfo.teamColor;
    }

    public void ToggleShowHide()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void ShowHide(bool show)
    {
        gameObject.SetActive(show);
    }
}