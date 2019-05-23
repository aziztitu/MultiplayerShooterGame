using System.Collections;
using System.Collections.Generic;
using Bolt;
using JetBrains.Annotations;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, @"^Arena.*")]
public class ArenaServerCallbacks : Bolt.GlobalEventListener
{
    public override void SceneLoadLocalDone(string scene)
    {
        base.SceneLoadLocalDone(scene);
        OnSceneReady();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        base.SceneLoadRemoteDone(connection);

        OnSceneReady(connection);
    }

    void OnSceneReady(BoltConnection connection = null)
    {
        int teamId = -1;
        if (connection == null)
        {
            teamId = ArenaDataManager.Instance.GetLocalArenaPlayerInfo().teamId;
        }
        else
        {
            var playerId = ArenaDataManager.Instance.GetPlayerIdFromConnection(connection);
            teamId = ArenaDataManager.Instance.GetArenaPlayerInfo(playerId).teamId;
        }

        var teamConfig = ArenaLevelManager.Instance.teamConfigList[teamId];

        var spawnPoint = teamConfig.spawnPointsRandomizer.GetRandomItem();
        if (spawnPoint != null)
        {
            if (connection == null)
            {
                ArenaCallbacks.SpawnPlayer(spawnPoint.transform.position, spawnPoint.transform.rotation);
            }
            else
            {
                var spawnPlayerEvent = SpawnPlayerEvent.Create(connection, ReliabilityModes.ReliableOrdered);
                spawnPlayerEvent.Position = spawnPoint.position;
                spawnPlayerEvent.Rotation = spawnPoint.rotation;
                spawnPlayerEvent.Send();
            }
        }
    }
}