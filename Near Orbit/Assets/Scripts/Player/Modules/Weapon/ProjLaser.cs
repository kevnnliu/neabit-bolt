using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjLaser : BaseProj {

    void Update() {
        BurnRange();
        if (!DidHit()) {
            transform.position += transform.forward * projVelocity * Time.deltaTime;
        }
    }

}
