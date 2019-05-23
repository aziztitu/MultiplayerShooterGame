using Bolt.Utils;
using TMPro;
using UdpKit;
using UnityEngine;
using UnityEngine.UI;

public class MatchListItemUI : MonoBehaviour
{
    public TextMeshProUGUI matchNameText;

    private UdpSession session;

    private void Awake()
    {
    }

    public void SetSessionData(UdpSession session)
    {
        this.session = session;

        UdpEndPoint udpEndPoint = session.HasLan ? session.LanEndPoint : session.WanEndPoint;
        var roomInfo = session.GetProtocolToken() as ArenaLobby.RoomInfo;

        string ipAndPort = $"{udpEndPoint.Address}:{udpEndPoint.Port}";
        if (roomInfo != null)
        {
            matchNameText.text = $"{roomInfo.roomName} [Host: {roomInfo.serverPlayerName}]";
        }
        else
        {
            matchNameText.text = $"{ipAndPort}";
        }
    }

    public void JoinMatch()
    {
        BoltNetwork.Connect(session, new ArenaLobby.JoinInfo()
        {
            account = GameManager.Instance.curAccount
        });
    }
}