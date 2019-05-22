using System;
using Bolt;
using UdpKit;

[Serializable]
public class Account : IProtocolToken
{
    public string name;

    public void Read(UdpPacket packet)
    {
        name = packet.ReadString();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(name);
    }
}