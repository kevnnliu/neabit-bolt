using UnityEngine;
using Bolt;

public class Weapon : MonoBehaviour {
    private PrefabId projectileType;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private int rpm;
    [SerializeField]
    private int clipSize;
    [SerializeField]
    private int cooldownRate;
    [SerializeField]
    private int cooldownDelay;

    public int FireInterval => BoltNetwork.FramesPerSecond * 60 / rpm;
    public int ReloadInterval => BoltNetwork.FramesPerSecond * 60 / cooldownRate;

    public BaseShip Owner;
    public bool Firing;

    private int lastFired;
    private int lastReloaded;
    private int clip;

    public void Start() {
        projectileType = projectile.GetComponent<BoltEntity>().PrefabId;
    }

    public void Update() {
        int fireDelay = BoltNetwork.ServerFrame - lastFired;
        int reloadDelay = BoltNetwork.ServerFrame - lastReloaded;
        if (Firing && clip > 0) {
            if (fireDelay >= FireInterval) {
                lastFired = BoltNetwork.ServerFrame;
                clip--;
                // Fire code
                if (BoltNetwork.IsServer) {
                    FireProjectile evt = FireProjectile.Create();
                    evt.ProjectileType = projectileType;
                    evt.Origin = transform.position;
                    evt.Rotation = transform.rotation;
                    evt.Frame = BoltNetwork.ServerFrame;
                    evt.Send();
                    BoltLog.Warn("Sent fire event");
                    // Other option?
                    BoltNetwork.Instantiate(projectileType, transform.position, transform.rotation)
                        .GetComponent<Projectile>()
                        .Init(BoltNetwork.ServerFrame, transform.position, Owner.AimTarget());
                }
                BoltLog.Warn("Firing: " + BoltNetwork.ServerTime);
            }
        } else if (fireDelay >= cooldownDelay * BoltNetwork.FramesPerSecond &&
                reloadDelay >= ReloadInterval) {
            lastReloaded = BoltNetwork.ServerFrame;
            clip++;
        }
    }
}
