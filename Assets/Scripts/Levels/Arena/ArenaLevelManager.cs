using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArenaLevelManager : LevelManager
{
    public const int maxTeamsAllowed = 2;

    public ArenaSettingsAsset arenaSettingsAsset;

    public GameObject CinemachineCameraRigPrefab;

    public ArenaMenu arenaMenu { get; private set; }
    public ArenaStatsMenu arenaStatsMenu { get; private set; }
    public bool roundEnded { get; private set; } = false;

    public string MultiplayerSceneName = "Multiplayer Menu";

    public List<ArenaTeamConfig> teamConfigList = new List<ArenaTeamConfig>();

    public override LevelManager.LevelPlayerType levelPlayerType => arenaSettingsAsset != null
        ? arenaSettingsAsset.levelPlayerType
        : LevelManager.LevelPlayerType.PlayerAndFlight;

    public new static ArenaLevelManager Instance => Get<ArenaLevelManager>();

    public event Action onRoundStarted;

    private new void Awake()
    {
        base.Awake();

        if (BoltNetwork.IsClient)
        {
            ArenaDataManager.Instance.arenaSettingsAsset = arenaSettingsAsset;
        }

        arenaMenu = GetComponentInChildren<ArenaMenu>(true);
        arenaStatsMenu = GetComponentInChildren<ArenaStatsMenu>(true);

        arenaMenu.ShowHide(false);
        arenaStatsMenu.ShowHide(false);

        SetupPlayerHooks();
        OnLocalPlayerModelChanged += SetupPlayerHooks;
    }

    private void Start()
    {
        OnRoundStarted();
    }

    private void SetupPlayerHooks()
    {
        if (LocalPlayerModel)
        {
            LocalPlayerModel.health.OnDeath.AddListener((killerPlayerId) => { arenaMenu.ShowHide(true); });
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            arenaMenu.ToggleShowHide();
        }

        arenaStatsMenu.ShowHide(arenaStatsMenu.stayOn || Input.GetButton("Stats"));
    }

    private void OnValidate()
    {
        if (arenaSettingsAsset)
        {
            if (arenaSettingsAsset.teams.Length > maxTeamsAllowed)
            {
                Debug.LogWarning($"Only a maximum of {maxTeamsAllowed} teams is supported now");
            }

            int numTeams = Math.Min(arenaSettingsAsset.teams.Length, maxTeamsAllowed);

            if (teamConfigList.Count < numTeams)
            {
                for (int i = 0; i < numTeams - teamConfigList.Count; i++)
                {
                    teamConfigList.Add(null);
                }
            }
            else if (teamConfigList.Count > numTeams)
            {
                teamConfigList.RemoveRange(numTeams, teamConfigList.Count - numTeams);
            }
        }
    }

    void OnRoundStarted()
    {
        roundEnded = false;
        
        if (BoltNetwork.IsServer)
        {
            ArenaDataManager.Instance.ResetTimer();
        }

        onRoundStarted?.Invoke();
    }

    public void OnRoundEnded(int winnerTeamId)
    {
        if (LocalPlayerModel != null)
        {
            LocalPlayerModel.DestroyPlayer();
        }

        arenaStatsMenu.EnableEndGameMode(winnerTeamId);
        
        CinemachineCameraManager.Instance.SwitchCameraState(CinemachineCameraManager.CinemachineCameraState.FreeFly);
        roundEnded = true;
    }

    public PlayerModel.PlayerType GetPlayerType(int playerId)
    {
        var arenaPlayerInfo = ArenaDataManager.Instance.GetArenaPlayerInfo(playerId);
        return arenaSettingsAsset.teams[arenaPlayerInfo.teamId].defaultPlayerType;
    }

    public PlayerModel.PlayerType GetLocalPlayerType()
    {
        return GetPlayerType(ArenaDataManager.Instance.localPlayerId);
    }

    public void GoToMultiplayerMenu(float delay)
    {
        StartCoroutine(GoToLobbyAfterSecs(delay));
    }

    public void RespawnLocalPlayer()
    {
        if (roundEnded)
        {
            return;
        }
        
        if (LocalPlayerModel != null)
        {
            if (LocalPlayerModel.health.isAlive && LocalPlayerModel.playerId.HasValue)
            {
                ArenaDataManager.Instance.PlayerDied(-1, LocalPlayerModel.playerId.Value);
            }

            LocalPlayerModel.DestroyPlayer();
        }

        var arenaTeamInfo =
            ArenaDataManager.Instance.GetArenaTeamInfoForPlayer(ArenaDataManager.Instance.localPlayerId);
        Transform spawnPoint = teamConfigList[arenaTeamInfo.teamId].spawnPointsRandomizer.GetRandomItem();
        if (spawnPoint != null)
        {
            ArenaCallbacks.SpawnPlayer(spawnPoint.transform.position, spawnPoint.transform.rotation);
        }
    }

    IEnumerator GoToLobbyAfterSecs(float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        SceneManager.LoadScene(MultiplayerSceneName);
    }
}