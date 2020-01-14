using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShipMod {

    float EnergyCost {
        get;
    }

    /// <summary>
    /// Determines whether this mod can be activated without equipping it.
    /// </summary>
    bool IsPassive {
        get;
    }

    /// <summary>
    /// Determines whether this mod's energy drain is multiplied by Time.deltaTime.
    /// </summary>
    bool IsContinuous {
        get;
    }

    void Activate(BaseShip properties);
    
}
