using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapMissile : IWeapon {

    float LockOnTime {
        get;
    }

    void LockOn(BaseShip target);

    void Fire(BaseShip target);

}
