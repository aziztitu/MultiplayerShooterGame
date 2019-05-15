using System.Collections;
using System.Collections.Generic;
using Bolt;
using JetBrains.Annotations;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, @"Arena.*")]
public class ArenaServerCallbacks : Bolt.GlobalEventListener
{
    private Randomizer<Transform> _spawnPointsRandomizer;

    public override void SceneLoadLocalDone(string scene)
    {
        base.SceneLoadLocalDone(scene);

        _spawnPointsRandomizer = new Randomizer<Transform>(ArenaLevelManager.Instance.SpawnPoints);
        OnSceneReady();
    }

    public override void SceneLoadRemoteDone(BoltConnection connection)
    {
        base.SceneLoadRemoteDone(connection);

        OnSceneReady(connection);
    }

    void OnSceneReady(BoltConnection connection = null)
    {
        Transform spawnPoint = _spawnPointsRandomizer.GetRandomItem();
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