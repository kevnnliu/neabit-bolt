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

    void Activate(Movement movement, IShip properties, BaseShip ship);
    
}
