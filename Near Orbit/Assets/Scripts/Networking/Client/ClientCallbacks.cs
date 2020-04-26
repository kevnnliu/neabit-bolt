[BoltGlobalBehaviour]
public class ClientCallbacks : Bolt.GlobalEventListener
{

    [System.Obsolete]
    public override void SceneLoadLocalDone(string scene)
    {
        PlayerCamera.Instantiate();
    }

    public override void ControlOfEntityGained(BoltEntity entity)
    {
        PlayerCamera.instance.SetTarget(entity);
    }

}
