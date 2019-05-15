using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour]
public class GlobalNetworkCallbacks : Bolt.GlobalEventListener
{
    public override void OnEvent(UpdatePlayerHealthEvent evnt)
    {
        base.OnEvent(evnt);
        evnt.TargetPlayerEntity.GetComponent<Health>().OnEvent(evnt);
    }
}
