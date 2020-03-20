using UnityEngine;
using Bolt;

public class Weapon : MonoBehaviour {
    [SerializeField]
    private PrefabId projectileType;
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

    public bool Firing;

    private int lastFired;
    private int lastReloaded;
    private int clip;

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
                }
                Debug.Log("Firing: " + BoltNetwork.ServerTime);
            }
        } else if (fireDelay >= cooldownDelay * BoltNetwork.FramesPerSecond &&
                reloadDelay >= ReloadInterval) {
            lastReloaded = BoltNetwork.ServerFrame;
            clip++;
        }
    }
}
