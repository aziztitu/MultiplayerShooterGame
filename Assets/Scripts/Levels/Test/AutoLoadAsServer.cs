using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoLoadAsServer : Bolt.GlobalEventListener
{
    void Awake()
    {
        BoltLauncher.StartServer(); 
    }

    public override void BoltStartDone()
    {
        base.BoltStartDone();
        if (BoltNetwork.IsServer)
        {
            string matchName = Guid.NewGuid().ToString();
            
            BoltNetwork.SetServerInfo(matchName, null);
            BoltNetwork.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}