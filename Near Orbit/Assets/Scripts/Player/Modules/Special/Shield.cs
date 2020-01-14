using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : BaseSpecial {

    public override void Activate(BaseShip properties) {
        properties.SetInvincibility(true);
    }

}
