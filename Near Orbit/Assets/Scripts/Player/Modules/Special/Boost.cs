using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : MonoBehaviour, ISpecial {

    #region Serialized Fields

    [SerializeField]
    private float _coolDown;
    [SerializeField]
    private float _energyCost;
    [SerializeField]
    private bool _isPassive;
    [SerializeField]
    private bool _isContinuous;
    [SerializeField]
    private float speedMultiplier;

    #endregion

    #region Properties

    public float Cooldown {
        get {
            return _coolDown;
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

    public void Activate(BaseShip properties) {
        properties.GetMovement().AmplifySpeed(speedMultiplier);
    }

}
