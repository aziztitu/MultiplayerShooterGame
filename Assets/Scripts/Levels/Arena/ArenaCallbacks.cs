using System.Collections.Generic;
using Bolt;
using JetBrains.Annotations;
using UnityEngine;

[BoltGlobalBehaviour("ArenaTestScene")]
public class ArenaCallbacks : Bolt.GlobalEventListener
{
    private void Start()
    {
    }
    
    public override void ControlOfEntityGained(BoltEntity entity)
    {
        base.ControlOfEntityGained(entity);
        
        ArenaLevelManager.Instance.LocalPlayerModel = entity.GetComponent<PlayerModel>();
//        ArenaLevelManager.Instance.LocalPlayerModel.SetupState();

        Instantiate(ArenaLevelManager.Instance.CinemachineCameraRigPrefab);
    }
}