using System.Net.Mime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public Image localPlayerIndicator;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI assistsText;
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI pingText;
    
    private ArenaDataManager.ArenaPlayerInfo playerInfo;

    private void Awake()
    {
    }

    public void SetPlayerInfo(ArenaDataManager.ArenaPlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
        Refresh();
    }

    public void Refresh()
    {
        playerNameText.text = playerInfo.playerName;
        localPlayerIndicator.gameObject.SetActive(playerInfo.playerId == ArenaDataManager.Instance.localPlayerId);
        scoreText.text = playerInfo.score.ToString();
        killsText.text = playerInfo.kills.ToString();
        assistsText.text = playerInfo.assists.ToString();
        deathsText.text = playerInfo.deaths.ToString();
        pingText.text = ((int) playerInfo.ping).ToString();
    }
}