using UnityEngine;

[BoltGlobalBehaviour("Test")]
public class ClientCallbacks : Bolt.GlobalEventListener {

    public override void SceneLoadLocalDone(string scene) {
        PlayerCamera.Instantiate();
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        PlayerCamera.instance.SetTarget(entity);
    }

    public override void OnEvent(FireProjectile evnt) {
        BoltLog.Warn("Spawned projectile at " + BoltNetwork.ServerTime);
    }
}
