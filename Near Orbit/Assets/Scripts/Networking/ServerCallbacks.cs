using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Game")]
public class ServerCallbacks : Bolt.GlobalEventListener {
    
    public override void SceneLoadLocalDone(string scene) {
        BoltNetwork.Instantiate(BoltPrefabs.TestShip);
    }

    public override void SceneLoadRemoteDone(BoltConnection connection) {
        BoltNetwork.Instantiate(BoltPrefabs.TestShip);
    }

}
