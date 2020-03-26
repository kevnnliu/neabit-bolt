using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // Fire now
        var token = new ProjectileToken {
            SpawnFrame = evnt.Frame
        };
        BoltNetwork.Instantiate(evnt.ProjectileType, token, evnt.Origin, evnt.Rotation);
    }

}
