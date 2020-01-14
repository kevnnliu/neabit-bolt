using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjLaser : BaseProj {

    void Update() {
        if (!DidHit()) {
            transform.position += transform.forward * GetProjVelocity() * Time.deltaTime;
        }
    }

}
