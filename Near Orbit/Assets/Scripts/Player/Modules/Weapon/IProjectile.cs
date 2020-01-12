using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile {

    /// <summary>
    /// Create impact object prefab (sparks, explosion, etc.) when Hit() is called.
    /// </summary>
    GameObject ImpactPrefab {
        get;
    }

    BaseShip Owner {
        get;
    }

    float MuzzleVelocity {
        get;
    }

    float Range {
        get;
    }

    /// <summary>
    /// ID of client who owns this projectile in the network. If this projectile hits a target
    /// on owner client, then the hit counts (shooter bias).
    /// </summary>
    int NetworkViewID {
        get;
    }

    void Parametrize(float dmg, BaseShip owner, int netID);

    /// <summary>
    /// Used to call BaseShip.HitMarker() to show hitmarker.
    /// </summary>
    void Hit(IShip target);

}
