using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthMenu : MonoBehaviour
{
    public static string authMenuSceneName = "Auth Menu";
    
    public string multiplayerMenuSceneName = "Multiplayer Menu";
    public TMP_InputField usernameInput;
//    public TMP_InputField passwordInput;

    private void Awake()
    {
        HelperUtilities.UpdateCursorLock(false);
        if (GameManager.Instance.isLoggedIn)
        {
            GoToMultiplayerMenu();
            return;
        }
        
        if (BoltNetwork.IsRunning)
        {
            BoltLauncher.Shutdown();
        }
    }

    public void Login()
    {
        string username = usernameInput.text.Trim();

        if (username.Length > 0)
        {
            GameManager.Instance.Login(username);
            GoToMultiplayerMenu();
        }
    }

    void GoToMultiplayerMenu()
    {
        SceneManager.LoadScene(multiplayerMenuSceneName);
    }
}