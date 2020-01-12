﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjLaser : BaseProj, IProjBullet {

    [SerializeField]
    private GameObject _impactPrefab;

    public GameObject ImpactPrefab {
        get {
            return _impactPrefab;
        }
    }

    void Update() {
        if (!DidHit()) {
            transform.position += transform.forward * MuzzleVelocity * Time.deltaTime;
        }
        else if (hitObject.GetComponent<IShip>() != null) {
            // TODO: Create impact object
            Hit(hitObject.GetComponent<IShip>());
        }
    }

}
