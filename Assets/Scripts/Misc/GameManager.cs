using System;

[Serializable]
public class GameManager
{
    public Account curAccount { get; private set; }

    public bool isLoggedIn => curAccount != null;

    public static GameManager Instance => _instance ?? (_instance = new GameManager());
    private static GameManager _instance;

    private GameManager()
    {
        curAccount = null;
    }

    public void Login(string username)
    {
        curAccount = new Account()
        {
            name = username
        };
    }
    
    public void Logout()
    {
        curAccount = null;
    }
}