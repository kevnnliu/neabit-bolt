﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour(BoltNetworkModes.Server, "Test")]
public class ServerCallbacks : Bolt.GlobalEventListener {

    public override void Connected(BoltConnection connection) {
        PlayerObjectRegistry.CreateClientPlayer(connection);
    }
    
    public override void SceneLoadRemoteDone(BoltConnection connection) {
        PlayerObjectRegistry.GetPlayer(connection).Spawn();
    }

}
