using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProj : MonoBehaviour {

    #region Serialized Fields

    [SerializeField]
    private GameObject impactPrefab;
    [SerializeField]
    private float projVelocity;
    [SerializeField]
    private float range;

    #endregion

    /// <summary>
    /// ID of client who owns this projectile in the network. If this projectile hits a target
    /// on the owner client, then the hit counts (shooter bias).
    /// </summary>
    private int networkViewID;
    private BaseShip owner;
    private GameObject hitObject;
    private float damage;

    public void Parametrize(float dmg, BaseShip ship, int netID) {
        damage = dmg;
        owner = ship;
        networkViewID = netID;
    }

    protected float GetProjVelocity() {
        return projVelocity;
    }

    /// <summary>
    /// Did this projectile hit something other than its owner?
    /// </summmary>
    protected bool DidHit() {
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, transform.forward, out hit, range);
        if (hitSomething) {
            // TODO: Check PhotonView.ViewID
            hitObject = hit.transform.root.gameObject;
            Hit(hitObject.GetComponent<BaseShip>());
            return hitObject != owner.gameObject;
        }
        return false;
    }

    /// <summary>
    /// Calls BaseShip.HitMarker() to show hitmarker, instantiates impact prefab, destroys self.
    /// </summary>
    private void Hit(BaseShip target) {
        if (target == null) {
            return;
        }
        target.TakeDamage(damage);
        owner.ShowHitMarker();
        // Instantiate(impactPrefab, transform.position, transform.rotation);
        Destroy(this.gameObject); // TODO: Owner client bias, network destroy
    }

}
