using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShip {

    float BaseHealth {
        get;
    }

    float Health {
        get;
        set;
    }

    float BaseEnergy {
        get;
    }

    float Energy {
        get;
        set;
    }

    float EnergyChargeRate {
        get;
    }

    /// <summary>
    /// The base speed of this ship. Non-throttle changes in speed should be handled by a Movement instance.
    /// </summary>
    float Speed {
        get;
    }

    /// <summary>
    /// The maximum total number of all mods on a ship.
    /// </summary>
    int TotalModCapacity {
        get;
    }

    /// <summary>
    /// Used for spawn invincibility, shielding, etc.
    /// </summary>
    bool Invincible {
        get;
        set;
    }

    /// <summary>
    /// Checks invincibility, subtracts from Health, and initiates respawn when necessary.
    /// </summary>
    void TakeDamage(float damage);

}
