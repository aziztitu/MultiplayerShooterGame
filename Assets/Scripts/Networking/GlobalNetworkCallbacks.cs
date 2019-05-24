using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour]
public class GlobalNetworkCallbacks : Bolt.GlobalEventListener
{
    public override void BoltStartBegin()
    {
        base.BoltStartBegin();
        
        BoltNetwork.RegisterTokenClass<Account>();
        BoltNetwork.RegisterTokenClass<ArenaLobby.RoomInfo>();
        BoltNetwork.RegisterTokenClass<ArenaLobby.JoinInfo>();
        BoltNetwork.RegisterTokenClass<ArenaLobby.JoinResult>();
    }

    public override void OnEvent(UpdateEntityHealthEvent evnt)
    {
        base.OnEvent(evnt);
        if (evnt.Target != null)
        {
            var health = evnt.Target.GetComponent<Health>();
            if (health != null)
            {
                health.OnEvent(evnt);
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (BoltNetwork.IsRunning)
        {
            BoltNetwork.ShutdownImmediate();
        }
    }
}
