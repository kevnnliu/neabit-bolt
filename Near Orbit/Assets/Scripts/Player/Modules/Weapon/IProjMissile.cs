using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjMissile : IProjectile {

    BaseShip Target {
        get;
        set;
    }

    void FollowTarget();

}
