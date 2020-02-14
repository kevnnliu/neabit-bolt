using UnityEngine;

public class LaserBlasterWeapon : BaseWeapon {
    public override void Init(BaseShip owner) {
        Init(owner, 0.4f);
    }

    protected override void Fire() {
        Quaternion rotation = Quaternion.LookRotation(owner.AimTarget() - gameObject.transform.position);
        ProjectileManager.SpawnProjectile<ProjLaser>(owner, gameObject.transform.position, rotation);
    }
}
