using System.Collections.Generic;
using UnityEngine;

public static class NetworkPlayerRegistry
{
    private static List<PlayerModel> _players = new List<PlayerModel>();

    public static void SpawnPlayer(Vector3 spawnPos, Quaternion spawnRot, BoltConnection connection = null)
    {
        BoltEntity playerEntity = BoltNetwork.Instantiate(BoltPrefabs.Player, spawnPos, spawnRot);
        PlayerModel playerModel = playerEntity.GetComponent<PlayerModel>();
        playerModel.connection = connection;

        if (playerModel.connection != null)
        {
            playerModel.connection.UserData = playerModel;
        }

        if (playerModel.IsServer)
        {
            playerEntity.TakeControl();
        }
        else
        {
            playerEntity.AssignControl(playerModel.connection);
        }
        
        _players.Add(playerModel);
    }
}