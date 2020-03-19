using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour("Test")]
public class ClientCallbacks : Bolt.GlobalEventListener {

    public override void SceneLoadLocalDone(string scene) {
        PlayerCamera.Instantiate();
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        // Temporary fix
        if (entity.PrefabId == BoltPrefabs.ViperShip) {
            PlayerCamera.instance.SetTarget(entity);
        }
    }

}
