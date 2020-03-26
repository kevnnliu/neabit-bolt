using UdpKit;

public class ProjectileToken : Bolt.IProtocolToken {
    public int SpawnFrame;

    public void Read(UdpPacket packet) {
        SpawnFrame = packet.ReadInt();
    }

    public void Write(UdpPacket packet) {
        packet.WriteInt(SpawnFrame);
    }
}