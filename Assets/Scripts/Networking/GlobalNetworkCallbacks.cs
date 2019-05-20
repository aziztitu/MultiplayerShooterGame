using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour]
public class GlobalNetworkCallbacks : Bolt.GlobalEventListener
{
    public override void OnEvent(UpdateEntityHealthEvent evnt)
    {
        base.OnEvent(evnt);
        evnt.Target.GetComponent<Health>().OnEvent(evnt);
    }

    private void OnApplicationQuit()
    {
        if (BoltNetwork.IsRunning)
        {
            BoltNetwork.ShutdownImmediate();
        }
    }
}
