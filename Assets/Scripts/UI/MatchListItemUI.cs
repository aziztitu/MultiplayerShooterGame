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
        
        matchNameText.text = $"{udpEndPoint.Address}:{udpEndPoint.Port} [{session.Id}]";
    }

    public void JoinMatch()
    {
        BoltNetwork.Connect(session);
    }
}