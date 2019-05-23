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

    public int localPlayerId { get; private set; } = -1;

    public int connectedPlayersCount => BoltNetwork.Connections.Count() + 1;
    public bool canAddPlayer => connectedPlayersCount < arenaSettingsAsset.arenaMaxCapacity;

    [Button("Refresh Team Infos", "RefreshTeamInfosFromState")]
    public bool refreshTeamInfos_Btn;

    private readonly Dictionary<int, ArenaPlayerInfo> arenaPlayerInfoDict = new Dictionary<int, ArenaPlayerInfo>();
    
    // Valid in server Only
    private readonly Dictionary<int, BoltConnection> arenaPlayerConnectionDict = new Dictionary<int, BoltConnection>();
    
    private Randomizer<ArenaTeamInfo> teamInfoRandomizer;

    public event Action<int> OnTeamInfoChanged;
    public event Action OnTeamInfosRefreshed;
    public event Action OnUnassignedPlayersRefreshed;

    public static ArenaDataManager Instance { get; private set; }

    private static readonly List<Action> OnReadyListeners = new List<Action>();
    private static readonly List<Action> OnReadyOneShotListeners = new List<Action>();

    private static void CallOnReadyListeners()
    {
        foreach (var onReadyListener in OnReadyListeners)
        {
            onReadyListener();
        }

        foreach (var onReadyOneShotListener in OnReadyOneShotListeners)
        {
            onReadyOneShotListener();
        }
        OnReadyOneShotListeners.Clear();
    }

    public static void AddOnReadyListener(Action action, bool oneShot = false)
    {
        if (Instance != null)
        {
            action();
            if (oneShot)
            {
                return;
            }
        }

        if (oneShot)
        {
            OnReadyOneShotListeners.Add(action);
        }
        else
        {
            OnReadyListeners.Add(action);
        }
    }

    public static void RemoveOnReadyListener(Action action, bool oneShot = false)
    {
        if (oneShot)
        {
            OnReadyOneShotListeners.Remove(action);
        }
        else
        {
            OnReadyListeners.Remove(action);
        }
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
        Debug.Log("Setting Local Player ID: " + playerId);
        localPlayerId = playerId;
    }
    
    public ArenaPlayerInfo GetLocalArenaPlayerInfo()
    {
        return GetArenaPlayerInfo(localPlayerId);
    }

    public ArenaPlayerInfo GetArenaPlayerInfo(int playerId)
    {
        Debug.Log(playerId);
        Debug.Log(arenaPlayerInfoDict[playerId]);
        return arenaPlayerInfoDict.ContainsKey(playerId) ? arenaPlayerInfoDict[playerId] : null;
    }

    public ArenaTeamInfo GetArenaTeamInfo(int playerId)
    {
        var playerInfo = GetArenaPlayerInfo(playerId);
        if (playerInfo != null && playerInfo.playerId >= 0)
        {
            return arenaTeamInfos[playerInfo.playerId];
        }

        return null;
    }
    
    #region Server Only

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

    public void OnBoltPlayerConnected(BoltConnection boltConnection, ArenaLobby.JoinResult joinResult)
    {
        int playerId = GetPlayerIdFromConnection(boltConnection);

        if (arenaPlayerInfoDict.ContainsKey(playerId))
        {
            return;
        }
        
        arenaPlayerConnectionDict[playerId] = boltConnection;

        ArenaPlayerInfo arenaPlayerInfo = new ArenaPlayerInfo()
        {
            teamId = UnassignedTeamId,
            playerId = joinResult.arenaPlayerId,
            playerName = joinResult.account.name,
            isServer = boltConnection == null
        };
        unassignedPlayers.Add(arenaPlayerInfo);
        OnTeamInfoChanged?.Invoke(UnassignedTeamId);

        arenaPlayerInfoDict[playerId] = arenaPlayerInfo;

        PlacePlayerInRandomTeam(arenaPlayerInfo);

        ApplyTeamInfosToState();
        ApplyUnassignedPlayersToState();
    }

    public void OnBoltPlayerDisconnected(BoltConnection boltConnection)
    {
        int playerId = GetPlayerIdFromConnection(boltConnection);

        if (!arenaPlayerInfoDict.ContainsKey(playerId))
        {
            return;
        }
        
        arenaPlayerConnectionDict.Remove(playerId);

        var arenaPlayerInfo = arenaPlayerInfoDict[playerId];
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

        arenaPlayerInfoDict.Remove(playerId);
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

    public int GetPlayerIdFromConnection(BoltConnection boltConnection)
    {
        if (boltConnection != null)
        {
            var joinResult = boltConnection.AcceptToken as ArenaLobby.JoinResult;
            if (joinResult != null)
            {
                return joinResult.arenaPlayerId;
            }
        }
        else
        {
            return localPlayerId;
        }

        return -1;
    }

    public void DisconnectUnassignedPlayers()
    {
        foreach (var unassignedPlayer in unassignedPlayers)
        {
            arenaPlayerConnectionDict[unassignedPlayer.playerId].Disconnect();
        }
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

    void CopyToState(ArenaPlayerInfo playerInfo, ArenaPlayerInfoStateObj statePlayerInfo)
    {
        statePlayerInfo.TeamId = playerInfo.teamId;
        statePlayerInfo.PlayerId = playerInfo.playerId;
        statePlayerInfo.PlayerName = playerInfo.playerName;
        statePlayerInfo.IsServer = playerInfo.isServer;
        statePlayerInfo.Kills = playerInfo.kills;
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

    #endregion

    #region Client Only

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

        arenaPlayerInfoDict[playerInfo.playerId] = playerInfo;

        return playerInfo;
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

    #endregion
}