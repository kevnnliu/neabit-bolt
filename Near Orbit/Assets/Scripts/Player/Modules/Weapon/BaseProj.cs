using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProj : MonoBehaviour, IProjectile {

    #region Property Fields

    [SerializeField]
    private float _muzzleVelocity;
    [SerializeField]
    private float _range;

    private BaseShip _owner;
    private int _networkViewID;

    #endregion

    #region Properties

    public float MuzzleVelocity {
        get {
            return _muzzleVelocity;
        }
    }

    public float Range {
        get {
            return _range;
        }
    }

    public BaseShip Owner {
        get {
            return _owner;
        }
    }

    public int NetworkViewID {
        get {
            return _networkViewID;
        }
    }

    #endregion

    protected GameObject hitObject;

    private float damage;

    public void Parametrize(float dmg, BaseShip owner, int netID) {
        damage = dmg;
        _owner = owner;
        _networkViewID = netID;
    }

    public void Hit(IShip target) {
        target.TakeDamage(damage);
        _owner.HitMarker();
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Did this projectile hit something other than its owner?
    /// </summmary>
    protected bool DidHit() {
        RaycastHit hit;
        bool hitSomething = Physics.Raycast(transform.position, transform.forward, out hit, Range);
        if (hitSomething) {
            // TODO: Check PhotonView.ViewID
            hitObject = hit.transform.root.gameObject;
            return hitObject != Owner.gameObject;
        }
        return false;
    }

}
