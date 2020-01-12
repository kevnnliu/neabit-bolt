using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjMissile : IProjectile {

    GameObject ExplosionPrefab {
        get;
    }

    BaseShip Target {
        get;
        set;
    }

    void FollowTarget();

    void Explode();

}
