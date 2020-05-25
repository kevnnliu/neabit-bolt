using Bolt;
using UdpKit;

public class ProjectileToken : Bolt.IProtocolToken
{
    public NetworkId Owner;
    public int SpawnFrame;

    public void Read(UdpPacket packet)
    {
        Owner = new NetworkId(packet.ReadULong());
        SpawnFrame = packet.ReadInt();
    }

    public void Write(UdpPacket packet)
    {
        packet.WriteULong(Owner.PackedValue);
        packet.WriteInt(SpawnFrame);
    }
}