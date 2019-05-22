using System;
using UnityEngine;
using UnityEngine.UI;

public class ArenaSelectionUI : MonoBehaviour
{
    public ArenaSettingsAsset arenaSettingsAsset;
    public event Action<ArenaSettingsAsset> OnArenaSelected;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { OnArenaSelected?.Invoke(arenaSettingsAsset); });
    }
}