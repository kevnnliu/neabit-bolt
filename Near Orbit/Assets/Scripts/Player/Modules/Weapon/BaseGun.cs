using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : MonoBehaviour, IWeapon {

    #region Serialized Fields

    [SerializeField]
    private GameObject projPrefab;
    [SerializeField]
    private float _baseDamage;
    [SerializeField]
    private float _baseDelayBetweenShots;
    [SerializeField]
    private float _energyCost;
    [SerializeField]
    private bool _isPassive;
    [SerializeField]
    private bool _isContinuous;

    #endregion

    private float _damage;
    private float _delayBetweenShots;

    #region Properties

    public float BaseDamage {
        get {
            return _baseDamage;
        }
    }

    public float Damage { 
        get {
            return _damage;
        } 
        set {
            _damage = value;
        } 
    }

    public float BaseDelayBetweenShots {
        get {
            return _baseDelayBetweenShots;
        }
    }

    public float DelayBetweenShots { 
        get {
            return _delayBetweenShots;
        } 
        set {
            _delayBetweenShots = value;
        } 
    }

    public float EnergyCost {
        get {
            return _energyCost;
        }
    }

    public bool IsPassive {
        get {
            return _isPassive;
        }
    }

    public bool IsContinuous {
        get {
            return _isContinuous;
        }
    }

    #endregion

    private float delay = 0f;
    private BaseShip owner;

    public void SetOwner(BaseShip ship) {
        owner = ship;
    }

    public void Activate(BaseShip properties) {
        if (delay <= 0f) {
            Fire(owner.AimTarget());
        }
    }

    /// <summary>
    /// To be called every frame to update the firing delay.
    /// </summary>
    protected void CheckDelay() {
        if (delay > 0f) {
            delay -= Time.deltaTime;
        }
    }

    protected virtual void Fire(Vector3 target) {
        delay = DelayBetweenShots;
        GameObject proj = Instantiate(projPrefab, transform.position, transform.rotation);
        proj.transform.LookAt(target, transform.up);
        proj.GetComponent<BaseProj>().Parametrize(Damage, owner, 9999); // TODO: Change 9999 to PhotonView.ViewID
    }

}
