using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon: IShipMod {

    float BaseDamage {
        get;
    }

    float Damage {
        get;
        set;
    }

    float BaseDelayBetweenShots {
        get;
    }

    float DelayBetweenShots {
        get;
        set;
    }

}
