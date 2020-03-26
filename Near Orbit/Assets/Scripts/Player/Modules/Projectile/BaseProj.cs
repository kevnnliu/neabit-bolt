using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OBSOLETE. Use for reference code.
/// </summary>
//public class BaseProj : MonoBehaviour {

//    #region Serialized Fields

//    [SerializeField]
//    protected float projVelocity;

//    [SerializeField]
//    private GameObject impactPrefab;
//    [SerializeField]
//    private float range;

//    #endregion

//    /// <summary>
//    /// ID of client who owns this projectile in the network. If this projectile hits a target
//    /// on the owner client, then the hit counts (shooter bias).
//    /// </summary>
//    private int networkViewID;
//    private BaseShip owner;
//    private GameObject hitObject;
//    private float damage;

//    public void Parametrize(float dmg, BaseShip ship, int netID) {
//        damage = dmg;
//        owner = ship;
//        networkViewID = netID;
//    }

//    public void SetOwner(BaseShip owner) {
//        this.owner = owner;
//    }

//    /// <summary>
//    /// To be called by derived classes at the beginning of every frame.
//    /// </summary>
//    protected void BurnRange() {
//        range -= Time.deltaTime;
//        if (range <= 0f) {
//            Destroy(this.gameObject);
//        }
//    }

//    /// <summary>
//    /// Did this projectile hit something other than its owner?
//    /// </summmary>
//    protected bool DidHit() {
//        RaycastHit hit;
//        bool hitSomething = Physics.Raycast(transform.position, transform.forward, out hit, projVelocity * Time.deltaTime);
//        if (hitSomething) {
//            // TODO: Check PhotonView.ViewID
//            hitObject = hit.transform.root.gameObject;
//            if (hitObject != owner.gameObject) {
//                Hit(hitObject.GetComponent<BaseShip>());
//            }
//        }
//        return hitSomething;
//    }

//    /// <summary>
//    /// Calls BaseShip.HitMarker() to show hitmarker, instantiates impact prefab, destroys self.
//    /// </summary>
//    private void Hit(BaseShip target) {
//        if (target != null) {
//            target.TakeDamage(damage);
//            owner.ShowHitMarker();
//        }
//        Instantiate(impactPrefab, transform.position, transform.rotation);
//        Destroy(this.gameObject); // TODO: Owner client bias, network destroy
//    }

//}
