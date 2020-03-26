using UnityEngine;

[BoltGlobalBehaviour("NetworkTest")]
public class ClientCallbacks : Bolt.GlobalEventListener {

    public override void SceneLoadLocalDone(string scene) {
        PlayerCamera.Instantiate();
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        PlayerCamera.instance.SetTarget(entity);
    }

}
