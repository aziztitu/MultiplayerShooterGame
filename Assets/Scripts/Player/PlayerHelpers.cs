using System.Collections.Generic;
using Bolt;
using UnityEngine;

public static class PlayerTypeMapping
{
    public static Dictionary<PlayerModel.PlayerType, PrefabId> playerPrefabs =
        new Dictionary<PlayerModel.PlayerType, PrefabId>()
        {
            {PlayerModel.PlayerType.Blue, BoltPrefabs.Player},
            {PlayerModel.PlayerType.Red, BoltPrefabs.Player_Red},
        };

    public static Dictionary<PlayerModel.PlayerType, PrefabId> flightPrefabs =
        new Dictionary<PlayerModel.PlayerType, PrefabId>()
        {
            {PlayerModel.PlayerType.Blue, BoltPrefabs.Flight},
            {PlayerModel.PlayerType.Red, BoltPrefabs.Flight_Red},
        };
}