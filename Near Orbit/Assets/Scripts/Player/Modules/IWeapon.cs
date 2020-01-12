using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon: IShipMod {

    float Damage {
        get;
        set;
    }

    float DelayBetweenShots {
        get;
        set;
    }

}
