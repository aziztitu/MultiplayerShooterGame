using System;
using Bolt;
using ExitGames.Client.Photon;
using UdpKit;
using UnityEngine;

public class ArenaLobby : Bolt.GlobalEventListener
{
    private int nextArenaPlayerId = 0;
    
    private void Start()
    {
        ArenaDataManager.AddOnReadyListener(OnArenaDataManagerReady);

        if (BoltNetwork.IsServer)
        {
            string matchName = Guid.NewGuid().ToString();
            BoltNetwork.SetServerInfo(matchName, new RoomInfo()
            {
                roomName = matchName,
                serverPlayerName = GameManager.Instance.curAccount.name
            });
        }
    }

    private void OnDestroy()
    {
        ArenaDataManager.RemoveOnReadyListener(OnArenaDataManagerReady);
    }

    public override void Connected(BoltConnection connection)
    {
        base.Connected(connection);
        
        Debug.Log("Connected");

        var joinResult = connection.AcceptToken as JoinResult;
        if (joinResult == null)
        {
            connection.Disconnect();
            return;
        }

        if (BoltNetwork.IsServer)
        {
            ArenaDataManager.Instance.OnBoltPlayerConnected(connection, joinResult);
        }
    }

    public override void Disconnected(BoltConnection connection)
    {
        base.Disconnected(connection);

        if (BoltNetwork.IsServer)
        {
            ArenaDataManager.Instance.OnBoltPlayerDisconnected(connection);
        }
    }

    public override void ConnectRequest(UdpEndPoint endpoint, IProtocolToken token)
    {
        base.ConnectRequest(endpoint, token);

        var joinInfo = token as JoinInfo;
        if (joinInfo != null)
        {
            var joinResult = new JoinResult()
            {
                account = joinInfo.account
            };

            if (ArenaDataManager.Instance.canAddPlayer)
            {
                joinResult.success = true;
                joinResult.arenaPlayerId = nextArenaPlayerId;
                nextArenaPlayerId++;
                
                BoltNetwork.Accept(endpoint, joinResult);
            }
            else
            {
                joinResult.success = false;
                BoltNetwork.Refuse(endpoint, joinResult);
            }
        }
    }

    void OnArenaDataManagerReady()
    {
        if (BoltNetwork.IsServer)
        {
            var dummyJoinResult = new JoinResult()
            {
                account = GameManager.Instance.curAccount,
                arenaPlayerId = nextArenaPlayerId,
                success = true
            };
            nextArenaPlayerId++;

            ArenaDataManager.Instance.SetLocalPlayerId(dummyJoinResult.arenaPlayerId);
            ArenaDataManager.Instance.OnBoltPlayerConnected(null, dummyJoinResult);
        }
        else
        {
            
        }
    }
    
    public class RoomInfo : IProtocolToken
    {
        public string roomName;
        public string serverPlayerName;

        public void Read(UdpPacket packet)
        {
            roomName = packet.ReadString();
            serverPlayerName = packet.ReadString();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteString(roomName);
            packet.WriteString(serverPlayerName);
        }
    }

    public class JoinInfo : IProtocolToken
    {
        public Account account;
        
        public void Read(UdpPacket packet)
        {
            account = packet.ReadToken() as Account;
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteToken(account);
        }
    }
    
    public class JoinResult : IProtocolToken
    {
        public Account account;
        public bool success = false;
        public int arenaPlayerId = -1;
        
        public void Read(UdpPacket packet)
        {
            account = packet.ReadToken() as Account;
            success = packet.ReadBool();
            arenaPlayerId = packet.ReadInt();
        }

        public void Write(UdpPacket packet)
        {
            packet.WriteToken(account);
            packet.WriteBool(success);
            packet.WriteInt(arenaPlayerId);
        }
    }
}