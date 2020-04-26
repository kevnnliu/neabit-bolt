[BoltGlobalBehaviour]
public class GlobalCallbacks : Bolt.GlobalEventListener
{

    public override void BoltStartBegin()
    {
        BoltNetwork.RegisterTokenClass<ProjectileToken>();
    }

    public override void BoltStartDone()
    {
        BoltLog.Warn("Registered ProjectileToken class");
    }

}
