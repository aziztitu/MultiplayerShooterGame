using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamScoreHUD_UI : MonoBehaviour
{
    public Image teamScoreBG;
    public TextMeshProUGUI teamScoreText;
    
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
        if (teamInfo != null)
        {
            teamScoreText.text = teamInfo.kills.ToString();
            teamScoreBG.color = teamInfo.teamColor;
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