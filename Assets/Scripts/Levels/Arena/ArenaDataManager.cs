using System;
using System.Collections.Generic;
using System.Linq;
using BasicTools.ButtonInspector;
using UnityEngine;

public class ArenaDataManager : Bolt.EntityBehaviour<IArenaState>
{
    public const int UnassignedTeamId = -1;
    public const int InvalidArrayItemId = -2;

    public ArenaSettingsAsset arenaSettingsAsset;

    [Serializable]
    public class ArenaTeamInfo
    {
        public int teamId = -1;
        public string teamName = "";
        public List<ArenaPlayerInfo> arenaPlayerInfos = new List<ArenaPlayerInfo>();
    }

    [Serializable]
    public class ArenaPlayerInfo
    {
        public int teamId = -1;
        public int playerId = -1;
        public string playerName = "";
        public bool isServer = false;
        public int kills = 0;
    }

    public List<ArenaTeamInfo> arenaTeamInfos = new List<ArenaTeamInfo>();
    public List<ArenaPlayerInfo> unassignedPlayers = new List<ArenaPlayerInfo>();

    private int localPlayerId = -1;

    public int connectedPlayersCount => BoltNetwork.Connections.Count() + 1;
    public bool canAddPlayer => connectedPlayersCount < arenaSettingsAsset.arenaMaxCapacity;

    [Button("Refresh Team Infos", "RefreshTeamInfosFromState")]
    public bool refreshTeamInfos_Btn;

    private readonly Dictionary<int, ArenaPlayerInfo> arenaPlayerInfoDict = new Dictionary<int, ArenaPlayerInfo>();
    private Randomizer<ArenaTeamInfo> teamInfoRandomizer;

    public event Action<int> OnTeamInfoChanged;
    public event Action OnTeamInfosRefreshed;
    public event Action OnUnassignedPlayersRefreshed;

    public static ArenaDataManager Instance { get; private set; }

    private static readonly List<Action> OnReadyListeners = new List<Action>();

    private static void CallOnReadyListeners()
    {
        foreach (var onReadyListener in OnReadyListeners)
        {
            onReadyListener();
        }
    }

    public static void AddOnReadyListener(Action action)
    {
        if (Instance != null)
        {
            action();
        }

        OnReadyListeners.Add(action);
    }
    
    public static void RemoveOnReadyListener(Action action)
    {
        OnReadyListeners.Remove(action);
    }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
        CallOnReadyListeners();
    }

    public override void Attached()
    {
        base.Attached();
        SetupState();
    }

    public void Initialize(ArenaSettingsAsset arenaSettings)
    {
        arenaSettingsAsset = arenaSettings;

        for (int i = 0; i < arenaSettingsAsset.teams.Length; i++)
        {
            arenaTeamInfos.Add(new ArenaTeamInfo
            {
                teamId = i,
                teamName = $"Team {'A' + i}"
            });
        }
        
        teamInfoRandomizer = new Randomizer<ArenaTeamInfo>(arenaTeamInfos);
    }

    void SetupState()
    {
        if (entity.IsOwner)
        {
            ApplyTeamInfosToState();
            ApplyUnassignedPlayersToState();
        }
        else
        {
            state.AddCallback("ArenaTeamInfo[]", ((newState, path, indices) => { RefreshTeamInfosFromState(); }));
            state.AddCallback("UnassignedPlayers[]",
                ((newState, path, indices) => { RefreshUnassignedPlayersFromState(); }));
        }
    }

    public void SetLocalPlayerId(int playerId)
    {
        localPlayerId = playerId;
    }

    public void PlacePlayerInTeam(ArenaPlayerInfo arenaPlayerInfo, ArenaTeamInfo teamInfo)
    {
        if (arenaPlayerInfo.teamId >= 0)
        {
            arenaTeamInfos[arenaPlayerInfo.teamId].arenaPlayerInfos.Remove(arenaPlayerInfo);
            OnTeamInfoChanged?.Invoke(arenaPlayerInfo.teamId);
        }
        else
        {
            unassignedPlayers.Remove(arenaPlayerInfo);
            OnTeamInfoChanged?.Invoke(UnassignedTeamId);
        }

        teamInfo.arenaPlayerInfos.Add(arenaPlayerInfo);
        arenaPlayerInfo.teamId = teamInfo.teamId;

        OnTeamInfoChanged?.Invoke(teamInfo.teamId);
    }

    public void OnBoltPlayerConnected(BoltConnection boltConnection, ArenaLobby.JoinResult joinResult)
    {
        int connectionId = GetSafeConnectionId(boltConnection);

        if (arenaPlayerInfoDict.ContainsKey(connectionId))
        {
            return;
        }

        ArenaPlayerInfo arenaPlayerInfo = new ArenaPlayerInfo()
        {
            teamId = UnassignedTeamId,
            playerId = joinResult.arenaPlayerId,
            playerName = joinResult.account.name,
            isServer = boltConnection == null
        };
        unassignedPlayers.Add(arenaPlayerInfo);
        OnTeamInfoChanged?.Invoke(UnassignedTeamId);

        arenaPlayerInfoDict[connectionId] = arenaPlayerInfo;

        PlacePlayerInRandomTeam(arenaPlayerInfo);

        ApplyTeamInfosToState();
        ApplyUnassignedPlayersToState();
    }

    public void OnBoltPlayerDisconnected(BoltConnection boltConnection)
    {
        int connectionId = GetSafeConnectionId(boltConnection);

        if (!arenaPlayerInfoDict.ContainsKey(connectionId))
        {
            return;
        }

        var arenaPlayerInfo = arenaPlayerInfoDict[connectionId];
        if (arenaPlayerInfo.teamId >= 0)
        {
            arenaTeamInfos[arenaPlayerInfo.teamId].arenaPlayerInfos.Remove(arenaPlayerInfo);
            OnTeamInfoChanged?.Invoke(arenaPlayerInfo.teamId);

            ApplyTeamInfosToState();
        }
        else
        {
            unassignedPlayers.Remove(arenaPlayerInfo);
            OnTeamInfoChanged?.Invoke(UnassignedTeamId);

            ApplyUnassignedPlayersToState();
        }

        arenaPlayerInfoDict.Remove(connectionId);
    }

    public void PlacePlayerInRandomTeam(ArenaPlayerInfo arenaPlayerInfo)
    {
        ArenaTeamInfo randomTeam = null;

        for (int i = 0; i < teamInfoRandomizer.items.Count * 2; i++)
        {
            var tmpTeamInfo = teamInfoRandomizer.GetRandomItem();
            
            var teamSettings = arenaSettingsAsset.teams[tmpTeamInfo.teamId];
            if (teamSettings.maxCapacity < 0 ||
                tmpTeamInfo.arenaPlayerInfos.Count < teamSettings.maxCapacity)
            {
                randomTeam = tmpTeamInfo;
                break;
            }
        }

        if (randomTeam != null)
        {
            PlacePlayerInTeam(arenaPlayerInfo, randomTeam);
        }
    }

    int GetSafeConnectionId(BoltConnection boltConnection)
    {
        return boltConnection != null ? (int) boltConnection.ConnectionId : -1;
    }

    void ApplyTeamInfosToState()
    {
        for (int i = 0; i < state.ArenaTeamInfo.Length; i++)
        {
            var stateTeamInfo = state.ArenaTeamInfo[i];

            if (i < arenaTeamInfos.Count)
            {
                var teamInfo = arenaTeamInfos[i];
                CopyToState(teamInfo, stateTeamInfo);
            }
            else
            {
                stateTeamInfo.TeamId = InvalidArrayItemId;
            }
        }
    }

    void ApplyUnassignedPlayersToState()
    {
        for (int i = 0;
            i < state.UnassignedPlayers.Length;
            i++)
        {
            var statePlayerInfo = state.UnassignedPlayers[i];

            if (i < unassignedPlayers.Count)
            {
                var playerInfo = unassignedPlayers[i];
                CopyToState(playerInfo, statePlayerInfo);
            }
            else
            {
                statePlayerInfo.TeamId = InvalidArrayItemId;
            }
        }
    }

    void RefreshTeamInfosFromState()
    {
        Debug.Log("Refreshing team infos");
        arenaTeamInfos.Clear();

        foreach (var stateTeamInfo in state.ArenaTeamInfo)
        {
            if (stateTeamInfo.TeamId == InvalidArrayItemId)
            {
                break;
            }

            arenaTeamInfos.Add(CreateFromState(stateTeamInfo));
        }

        OnTeamInfosRefreshed?.Invoke();
    }

    void RefreshUnassignedPlayersFromState()
    {
        Debug.Log("Refreshing unassigned players");
        unassignedPlayers.Clear();

        foreach (var statePlayerInfo in state.UnassignedPlayers)
        {
            if (statePlayerInfo.TeamId == InvalidArrayItemId)
            {
                break;
            }

            unassignedPlayers.Add(CreateFromState(statePlayerInfo));
        }

        OnUnassignedPlayersRefreshed?.Invoke();
    }

    void CopyToState(ArenaPlayerInfo playerInfo, ArenaPlayerInfoStateObj statePlayerInfo)
    {
        statePlayerInfo.TeamId = playerInfo.teamId;
        statePlayerInfo.PlayerId = playerInfo.playerId;
        statePlayerInfo.PlayerName = playerInfo.playerName;
        statePlayerInfo.IsServer = playerInfo.isServer;
        statePlayerInfo.Kills = playerInfo.kills;
    }

    ArenaPlayerInfo CreateFromState(ArenaPlayerInfoStateObj statePlayerInfo)
    {
        ArenaPlayerInfo playerInfo = new ArenaPlayerInfo()
        {
            teamId = statePlayerInfo.TeamId,
            playerId = statePlayerInfo.PlayerId,
            playerName = statePlayerInfo.PlayerName,
            isServer = statePlayerInfo.IsServer,
            kills = statePlayerInfo.Kills,
        };
        return playerInfo;
    }

    void CopyToState(ArenaTeamInfo teamInfo, ArenaTeamInfoStateObj stateTeamInfo)
    {
        stateTeamInfo.TeamId = teamInfo.teamId;
        stateTeamInfo.TeamName = teamInfo.teamName;

        for (int j = 0;
            j < stateTeamInfo.ArenaPlayerInfos.Length;
            j++)
        {
            var statePlayerInfo = stateTeamInfo.ArenaPlayerInfos[j];

            if (j < teamInfo.arenaPlayerInfos.Count)
            {
                var playerInfo = teamInfo.arenaPlayerInfos[j];
                CopyToState(playerInfo, statePlayerInfo);
            }
            else
            {
                statePlayerInfo.TeamId = InvalidArrayItemId;
            }
        }
    }

    ArenaTeamInfo CreateFromState(ArenaTeamInfoStateObj stateTeamInfo)
    {
        ArenaTeamInfo teamInfo = new ArenaTeamInfo()
        {
            teamId = stateTeamInfo.TeamId,
            teamName = stateTeamInfo.TeamName,
        };

        foreach (var statePlayerInfo in stateTeamInfo.ArenaPlayerInfos)
        {
            if (statePlayerInfo.TeamId == InvalidArrayItemId)
            {
                break;
            }

            teamInfo.arenaPlayerInfos.Add(CreateFromState(statePlayerInfo));
        }

        return teamInfo;
    }
}