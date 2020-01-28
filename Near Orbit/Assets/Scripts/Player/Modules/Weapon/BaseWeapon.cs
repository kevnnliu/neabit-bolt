using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : MonoBehaviour, IShipMod {

    #region Serialized Fields

    [SerializeField]
    private GameObject projPrefab;
    [SerializeField]
    private float baseDamage;
    [SerializeField]
    private float baseDelay;
    [SerializeField]
    private ModType typeOfMod;
    [SerializeField]
    private float energyCost;
    [SerializeField]
    private bool isPassive;
    [SerializeField]
    private bool isContinuous;

    #endregion

    #region Properties

    public ModType TypeOfMod {
        get {
            return typeOfMod;
        }
    }

    public float EnergyCost {
        get {
            return energyCost;
        }
    }

    public bool IsPassive {
        get {
            return isPassive;
        }
    }

    public bool IsContinuous {
        get {
            return isContinuous;
        }
    }

    #endregion

    private const float firingArc = 18f;

    private float damageFactor = 1f;
    private float delayFactor = 1f;
    private float delay = 0f;
    private BaseShip owner;

    void Update() {
        if (delay > 0f) {
            delay -= Time.deltaTime;
        }
    }

    public virtual bool ReadyToFire() {
        return delay <= 0f;
    }

    public void Activate(BaseShip properties) {
        owner = properties;
        if (ReadyToFire()) {
            Fire(owner.AimTarget());
        }
    }

    public void AmplifyDelay(float factor) {
        delayFactor = factor;
    }

    public void AmplifyDamage(float factor) {
        damageFactor = factor;
    }

    protected virtual void Fire(Vector3 target) {
        delay = baseDelay * delayFactor;
        float damage = baseDamage * damageFactor;

        GameObject proj = Instantiate(projPrefab, transform.position, transform.rotation);
        proj.GetComponent<BaseProj>().Parametrize(damage, owner, 9999); // TODO: Change 9999 to PhotonView.ViewID
        proj.transform.LookAt(target, transform.up);

        Vector3 aimVector = target - transform.position;
        Vector3 normal = Vector3.Cross(transform.forward, aimVector);
        float angle = Vector3.SignedAngle(transform.forward, aimVector, normal);
        if (Mathf.Abs(angle) > firingArc) {
            proj.transform.rotation = Quaternion.AngleAxis(firingArc * Mathf.Sign(angle), normal);
        }

        damageFactor = 1f;
        delayFactor = 1f;
    }

}
