using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArenaLobbyPlayerInfoItemUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI text;

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
        text.text = playerInfo.playerName;
    }
}