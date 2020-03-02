using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour, IShipModule {
    private float firingDelay;
    private float delay = 0;
    private bool firing = false;

    protected BaseShip owner;

    public ModuleType Type => ModuleType.Weapon;

    public bool IsActive => firing || delay > 0;

    public abstract void Init(BaseShip owner);

    protected void Init(BaseShip owner, float firingDelay) {
        this.owner = owner;
        this.firingDelay = firingDelay;
    }

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
