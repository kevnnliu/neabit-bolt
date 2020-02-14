using UnityEngine;
using System.Collections;

public class LaserBlasterWeapon : BaseWeapon {
    public LaserBlasterWeapon(BaseShip owner) : base(owner, 0.4f) {
        
    }

    protected override void Fire() {
        Debug.Log("I'm firin' ma lazer!");
    }
}
