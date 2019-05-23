using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Arena/Settings", fileName = "Arena Settings")]
public class ArenaSettingsAsset: ScriptableObject
{
    public enum LevelPlayerType
    {
        PlayerAndFlight,
        PlayerOnly,
        FlightOnly,
    }

    [Serializable]
    public class TeamSettings
    {
        public int maxCapacity;
        public PlayerModel.PlayerType defaultPlayerType;
    }

    public string arenaName;
    public string arenaSceneName;

    public LevelPlayerType levelPlayerType = LevelPlayerType.PlayerAndFlight;

    public TeamSettings[] teams;

    public int arenaMaxCapacity => teams.Sum(team => team.maxCapacity);
}