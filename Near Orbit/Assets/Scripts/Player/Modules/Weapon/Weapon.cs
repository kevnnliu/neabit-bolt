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
    private float cooldownDelay;
    [SerializeField]
    private Transform origin;

    public int FireInterval => BoltNetwork.FramesPerSecond * 60 / rpm;
    public int ReloadInterval => BoltNetwork.FramesPerSecond * 60 / cooldownRate;

    [System.NonSerialized]
    public BaseShip Owner;
    [System.NonSerialized]
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
                // Old code: Server instantiates projectile
                //if (BoltNetwork.IsServer) {
                //    BoltNetwork.Instantiate(projectileType, transform.position, transform.rotation)
                //        .GetComponent<Projectile>()
                //        .Init(BoltNetwork.ServerFrame, transform.position, Owner.AimTarget());
                //}
                // New code: Client sends information, server instantiates projectile
                if (BoltNetwork.IsClient) {
                    var evnt = FireProjectile.Create();
                    evnt.Owner = Owner.entity.NetworkId;
                    evnt.Frame = BoltNetwork.ServerFrame;
                    evnt.ProjectileType = projectileType;
                    evnt.Origin = origin.position;
                    evnt.Rotation = Quaternion.LookRotation(Owner.AimTarget() - origin.position);
                    evnt.Send();
                    Debug.Log("Ammo: " + clip + "/" + clipSize);
                }
                if (BoltNetwork.IsServer) {

                }
                BoltLog.Warn("Firing at time: " + BoltNetwork.ServerTime);
            }
        } else if (fireDelay >= cooldownDelay * BoltNetwork.FramesPerSecond &&
                reloadDelay >= ReloadInterval && clip < clipSize) {
            lastReloaded = BoltNetwork.ServerFrame;
            clip++;
        }
    }
}
