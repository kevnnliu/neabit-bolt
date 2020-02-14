using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : IShipModule {
    private readonly float firingDelay;
    private float delay = 0;
    private bool firing = false;

    protected BaseShip owner;

    protected BaseWeapon(BaseShip owner, float firingDelay) {
        this.owner = owner;
        this.firingDelay = firingDelay;
    }

    public ModuleType Type => ModuleType.Weapon;

    public bool IsActive => firing || delay > 0;

    public void Activate() {
        firing = true;
    }

    public void Deactivate() {
        firing = false;
    }

    public void Update() {
        delay = Mathf.Max(delay - Time.deltaTime, 0);

        // Fire after set delay only if button is held down
        if (delay == 0 && firing) {
            delay = firingDelay;
            Fire();
        }
    }

    protected abstract void Fire();
}
