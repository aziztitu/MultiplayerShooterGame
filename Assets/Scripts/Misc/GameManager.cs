using System;

[Serializable]
public class GameManager
{
    public Account curAccount { get; private set; }

    public static GameManager Instance => _instance ?? (_instance = new GameManager());
    private static GameManager _instance;

    private GameManager()
    {
        curAccount = null;
    }

    public void Login(string name)
    {
        curAccount = new Account()
        {
            name = name
        };
    }
}