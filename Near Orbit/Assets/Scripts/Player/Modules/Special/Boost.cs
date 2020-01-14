using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boost : BaseSpecial {

    [SerializeField]
    private float speedMultiplier;

    public override void Activate(BaseShip properties) {
        properties.GetMovement().AmplifySpeed(speedMultiplier);
    }

}
