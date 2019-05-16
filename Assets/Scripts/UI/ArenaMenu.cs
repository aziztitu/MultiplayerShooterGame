using UnityEngine;

public class ArenaMenu : MonoBehaviour
{
    public void ToggleShowHide()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    
    public void ShowHide(bool show)
    {
        gameObject.SetActive(show);
    }
    
    public void GoToLobby()
    {
        ArenaLevelManager.Instance.GoToLobby(0);
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
}