using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viper : BaseShip {

    void Start() {
        LoadBaseShip();
    }

    void Update() {
        // TODO: Process mod activation before calculating movement.
        CalculateMovement();
    }

}
