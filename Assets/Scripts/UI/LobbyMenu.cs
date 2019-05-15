using System;
using System.Collections.Generic;
using UdpKit;
using UnityEngine;

public class LobbyMenu: Bolt.GlobalEventListener
{
    public string arenaSceneName = "ArenaTestScene";
    
    public void CreateGame()
    {
        BoltLauncher.StartServer();
    }
    
    public void JoinGame()
    {
        BoltLauncher.StartClient();
    }

    public override void BoltStartDone()
    {
        if (BoltNetwork.IsServer)
        {
            string matchName = Guid.NewGuid().ToString();
            
            BoltNetwork.SetServerInfo(matchName, null);
            BoltNetwork.LoadScene(arenaSceneName);
        }
        else
        {
            Map<Guid, UdpSession> sessionList = new Map<Guid, UdpSession>();
            BoltNetwork.UpdateSessionList(sessionList);
            Debug.LogFormat("Forced: Session list updated: {0} total sessions", sessionList.Count);
        }
    }

    public override void SessionListUpdated(Map<Guid, UdpSession> sessionList)
    {
        Debug.LogFormat("Session list updated: {0} total sessions", sessionList.Count);

        foreach (KeyValuePair<Guid,UdpSession> pair in sessionList)
        {
            UdpSession session = pair.Value;

            if (session.Source == UdpSessionSource.Photon)
            {
                BoltNetwork.Connect(session);
            }
        }
    }       
}