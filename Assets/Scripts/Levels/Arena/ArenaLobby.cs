using UnityEngine;

public class ArenaLobby : Bolt.GlobalEventListener
{
    private void Start()
    {
        if (BoltNetwork.IsServer)
        {
            ArenaDataManager.Instance.OnBoltPlayerConnected(null);
        }
    }

    public override void Connected(BoltConnection connection)
    {
        base.Connected(connection);

        if (BoltNetwork.IsServer)
        {
            ArenaDataManager.Instance.OnBoltPlayerConnected(connection);
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
}