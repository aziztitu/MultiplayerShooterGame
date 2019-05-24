using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArenaHUD : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public GameObject teamScoresContainer;
    public GameObject teamScorePrefab;

    private List<TeamScoreHUD_UI> teamScoreHudUiList = new List<TeamScoreHUD_UI>();

    private void Awake()
    {
        ArenaDataManager.AddOnReadyListener(() =>
        {
            if (BoltNetwork.IsClient)
            {
                ArenaDataManager.Instance.OnAllTeamInfosRefreshed += RefreshAllTeams;
            }

            RefreshAllTeams();
            RefreshTimer();

            ArenaDataManager.Instance.state.AddCallback("Timer", RefreshTimer);
        }, true);
    }

    private void OnDestroy()
    {
        if (BoltNetwork.IsClient)
        {
            ArenaDataManager.Instance.OnAllTeamInfosRefreshed -= RefreshAllTeams;
        }
    }

    void Reset()
    {
        foreach (var teamScoreHudUi in teamScoreHudUiList)
        {
            Destroy(teamScoreHudUi.gameObject);
        }

        foreach (var teamInfo in ArenaDataManager.Instance.arenaTeamInfos)
        {
            var teamScoreHudUi = Instantiate(teamScorePrefab, teamScoresContainer.transform)
                .GetComponent<TeamScoreHUD_UI>();

            teamScoreHudUi.SetTeamInfo(teamInfo);
            teamScoreHudUiList.Add(teamScoreHudUi);
        }
    }

    private void RefreshAllTeams()
    {
        if (teamScoreHudUiList.Count != ArenaDataManager.Instance.arenaTeamInfos.Count)
        {
            Reset();
        }
        else
        {
            for (int i = 0; i < teamScoreHudUiList.Count; i++)
            {
                var teamStatsUi = teamScoreHudUiList[i];
                teamStatsUi.SetTeamInfo(ArenaDataManager.Instance.arenaTeamInfos[i]);
            }
        }
    }

    void RefreshTimer()
    {
        int secsLeft = ArenaDataManager.Instance.state.Timer;
        secsLeft = Math.Max(0, secsLeft);

        int mins = secsLeft / 60;
        int secs = secsLeft % 60;

        timerText.text = $"{mins:00}:{secs:00}";
    }
}