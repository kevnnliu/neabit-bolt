using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using UdpKit;

public class ClientToken : IProtocolToken
{
    public string UserId;
    public string Username;
    public string PlayerSessionId;

    public void Read(UdpPacket packet)
    {
        UserId = packet.ReadString();
        Username = packet.ReadString();
        PlayerSessionId = packet.ReadString();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteString(UserId);
        packet.WriteString(Username);
        packet.WriteString(PlayerSessionId);
    }
}
