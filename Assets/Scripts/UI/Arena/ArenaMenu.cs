using UnityEngine;
using UnityEngine.UI;

public class ArenaMenu : MonoBehaviour
{
    public Button respawnBtn;
    
    public void ToggleShowHide()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    
    public void ShowHide(bool show)
    {
        gameObject.SetActive(show);
    }
    
    public void GoToMultiplayerMenu()
    {
        ArenaLevelManager.Instance.GoToMultiplayerMenu(0);
    }
    
    public void Respawn()
    {
        ArenaLevelManager.Instance.RespawnLocalPlayer();
        ShowHide(false);
    }

    private void OnEnable()
    {
        HelperUtilities.UpdateCursorLock(false);
        if (LevelManager.Instance)
        {
            LevelManager.Instance.interactingWithUI = true;
        }
    }
    
    private void OnDisable()
    {
        HelperUtilities.UpdateCursorLock(true);
        if (LevelManager.Instance)
        {
            LevelManager.Instance.interactingWithUI = false;
        }
    }
    
    public void EnableEndGameMode(int winnerTeamId)
    {
        respawnBtn.interactable = false;
        respawnBtn.gameObject.SetActive(false);
    }
}