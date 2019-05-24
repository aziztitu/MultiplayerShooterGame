using UnityEngine;

public class ArenaStatsMenu : MonoBehaviour
{
    public GameObject teamStatsUIPrefab;
    public Transform teamListContainer;

    public void ToggleShowHide()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    
    public void ShowHide(bool show)
    {
        gameObject.SetActive(show);
    }

    private void OnEnable()
    {
//        HelperUtilities.UpdateCursorLock(false);
        if (LevelManager.Instance)
        {
//            LevelManager.Instance.interactingWithUI = true;
        }
    }
    
    private void OnDisable()
    {
//        HelperUtilities.UpdateCursorLock(true);
        if (LevelManager.Instance)
        {
//            LevelManager.Instance.interactingWithUI = false;
        }
    }
}