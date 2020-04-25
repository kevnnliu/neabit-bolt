[BoltGlobalBehaviour(BoltNetworkModes.Server, "NetworkTest")]
public class ServerCallbacks : Bolt.GlobalEventListener {

    public override void Connected(BoltConnection connection) {
        PlayerObjectRegistry.CreateClientPlayer(connection);
    }
    
    public override void SceneLoadRemoteDone(BoltConnection connection) {
        BoltLog.Warn("Spawning player");
        PlayerObjectRegistry.GetPlayer(connection).Spawn();
    }

    public override void OnEvent(FireProjectile evnt) {
        // TODO: Ammo check
        const int MAXIMUM_LAG = 30;
        if (evnt.Frame + MAXIMUM_LAG < BoltNetwork.ServerFrame) {
            return;
        }

        var token = new ProjectileToken {
            Owner = evnt.Owner,
            SpawnFrame = evnt.Frame
        };
        BoltNetwork.Instantiate(evnt.ProjectileType, token, evnt.Origin, evnt.Rotation);
    }
}
