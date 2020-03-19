using UnityEngine;
using Bolt;

public class Weapon : EntityBehaviour<IWeaponState> {
    [SerializeField]
    private ProjectileType projectileType;
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

    public void Update() {
        int lastFired = BoltNetwork.ServerFrame - state.LastFired;
        int lastReloaded = BoltNetwork.ServerFrame - state.LastReloaded;
        if (Firing && state.Clip > 0) {
            if (lastFired >= FireInterval) {
                state.LastFired = BoltNetwork.ServerFrame;
                state.Clip--;
                // Fire code
                Debug.Log("Firing: " + BoltNetwork.ServerTime);
            }
        } else if (lastFired >= cooldownDelay * BoltNetwork.FramesPerSecond &&
                lastReloaded >= ReloadInterval) {
            state.LastReloaded = BoltNetwork.ServerFrame;
            state.Clip++;
        }
    }
}
