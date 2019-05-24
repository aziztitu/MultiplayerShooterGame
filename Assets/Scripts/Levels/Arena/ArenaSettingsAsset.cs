using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Arena/Settings", fileName = "Arena Settings")]
public class ArenaSettingsAsset: ScriptableObject
{
    [Serializable]
    public class TeamSettings
    {
        public string teamName;
        public Color teamColor;
        public int maxCapacity;
        public PlayerModel.PlayerType defaultPlayerType;
    }

    public string arenaName;
    public string arenaSceneName;
    public Sprite arenaCoverImage;

    public LevelManager.LevelPlayerType levelPlayerType = LevelManager.LevelPlayerType.PlayerAndFlight;

    public TeamSettings[] teams;

    public int arenaMaxCapacity => teams.Sum(team => team.maxCapacity);

    public int roundDuration = 300;
}