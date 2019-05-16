using System;
using UnityEngine;
using UnityEngine.UI;

public class ArenaSelectionUI : MonoBehaviour
{
    public string arenaLevelName = "Arena";
    public event Action<string> OnArenaSelected;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => { OnArenaSelected?.Invoke(arenaLevelName); });
    }
}