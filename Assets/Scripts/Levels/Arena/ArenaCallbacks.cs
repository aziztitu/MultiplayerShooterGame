using System.Collections.Generic;
using Bolt;
using JetBrains.Annotations;
using UnityEngine;

[BoltGlobalBehaviour(@"Arena.*")]
public class ArenaCallbacks : Bolt.GlobalEventListener
{
    public static void SpawnPlayer(Vector3 spawnPos, Quaternion spawnRot)
    {
        var playerType = ArenaLevelManager.Instance.GetLocalPlayerType();
        
        BoltEntity playerEntity = BoltNetwork.Instantiate(PlayerTypeMapping.playerPrefabs[playerType], spawnPos, spawnRot);
        PlayerModel playerModel = playerEntity.GetComponent<PlayerModel>();
        ArenaLevelManager.Instance.LocalPlayerModel = playerModel;

        if (!CinemachineCameraManager.Instance)
        {
            Instantiate(ArenaLevelManager.Instance.CinemachineCameraRigPrefab);
        }
    }

    private void Start()
    {
    }

    public override void OnEvent(SpawnPlayerEvent e)
    {
        base.OnEvent(e);
        SpawnPlayer(e.Position, e.Rotation);
    }
}