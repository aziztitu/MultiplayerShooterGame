using TMPro;
using UnityEngine;

public class AutoRespawnerUI : MonoBehaviour
{
    public TextMeshProUGUI msgText;
    public int respawnDelay = 5;

    private float enabledTime = float.MinValue;

    private void OnEnable()
    {
        enabledTime = Time.time;
    }

    private void Update()
    {
        int elapsedTime = (int) (Time.time - enabledTime);
        int secsLeft = Mathf.Clamp(respawnDelay - elapsedTime, 0, respawnDelay);

        msgText.text = $"Respawning in {secsLeft}...";

        if (secsLeft <= 0)
        {
            ArenaLevelManager.Instance.RespawnLocalPlayer();
            ShowHide(false);
        }
    }
    
    public void ShowHide(bool show)
    {
        gameObject.SetActive(show);
    }
}