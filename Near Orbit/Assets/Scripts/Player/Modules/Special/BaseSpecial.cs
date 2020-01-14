using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpecial: MonoBehaviour, IShipMod {

    #region Serialized Fields

    [SerializeField]
    private ModType _typeOfMod;
    [SerializeField]
    private float _energyCost;
    [SerializeField]
    private bool _isPassive;
    [SerializeField]
    private bool _isContinuous;

    #endregion

    #region Properties

    public ModType TypeOfMod {
        get {
            return _typeOfMod;
        }
    }

    public float EnergyCost {
        get {
            return _energyCost;
        }
    }

    public bool IsPassive {
        get {
            return _isContinuous;
        }
    }

    public bool IsContinuous {
        get {
            return _isPassive;
        }
    }

    #endregion

    public virtual void Activate(BaseShip properties) {
        throw new System.NotImplementedException();
    }

}
