using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArenaSelectionUI : MonoBehaviour
{
    public Image coverImage;
    public TextMeshProUGUI arenaNameText;
    public TextMeshProUGUI arenaInfoText;

    private ArenaSettingsAsset arenaSettingsAsset;
    public event Action<ArenaSettingsAsset> OnArenaSelected;

    public void Init(ArenaSettingsAsset asset)
    {
        arenaSettingsAsset = asset;

        coverImage.sprite = arenaSettingsAsset.arenaCoverImage;
        arenaNameText.text = arenaSettingsAsset.arenaName;

        string controllerType = arenaSettingsAsset.levelPlayerType.ToString();
        int teamsCount = arenaSettingsAsset.teams.Length;

        string playersOnEachTeam = "";
        for (int i = 0; i < teamsCount; i++)
        {
            playersOnEachTeam += $"{arenaSettingsAsset.teams[0].maxCapacity}";
            if (i != teamsCount - 1)
            {
                playersOnEachTeam += ", ";
            }
        }

        playersOnEachTeam = $"[{playersOnEachTeam}]";

        arenaInfoText.text =
            $"Controller type: {controllerType}\nTeams: {teamsCount}\nPlayers on each team: {playersOnEachTeam}";

        GetComponent<Button>().onClick.AddListener(() => { OnArenaSelected?.Invoke(arenaSettingsAsset); });
    }
}