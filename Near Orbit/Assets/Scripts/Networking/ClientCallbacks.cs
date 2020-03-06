using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[BoltGlobalBehaviour("Test")]
public class TutorialPlayerCallbacks : Bolt.GlobalEventListener {

    public override void SceneLoadLocalDone(string scene) {
        PlayerCamera.Instantiate();
    }

    public override void ControlOfEntityGained(BoltEntity entity) {
        PlayerCamera.instance.SetTarget(entity);
    }

}
