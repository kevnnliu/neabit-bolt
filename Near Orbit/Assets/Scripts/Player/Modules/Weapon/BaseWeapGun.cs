using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapGun : MonoBehaviour, IWeapGun {

    #region Serialized Fields

    [SerializeField]
    private GameObject _projPrefab;
    [SerializeField]
    private float _baseDamage;
    [SerializeField]
    private float _baseDelayBetweenShots;
    [SerializeField]
    private float _energyCost;
    [SerializeField]
    private bool _isPassive;

    #endregion

    private float _damage;
    private float _delayBetweenShots;

    #region Properties

    public GameObject ProjPrefab {
        get {
            return _projPrefab;
        }
    }

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

    #endregion

    private float delay = 0f;
    private BaseShip owner;

    protected void SetOwner(BaseShip ship) {
        owner = ship;
    }

    protected void CheckDelay() {
        if (delay > 0f) {
            delay -= Time.deltaTime;
        }
    }

    public void Activate(Movement movement, IShip properties) {
        Fire(owner.AimTarget());
    }

    public void Fire(Vector3 target) {
        delay = DelayBetweenShots;
        GameObject proj = Instantiate(ProjPrefab, transform.position, transform.rotation);
        proj.transform.LookAt(target, transform.up);
        proj.GetComponent<BaseProj>().Parametrize(Damage, owner, 9999); // TODO: Change 9999 to PhotonView.ViewID
    }

}
