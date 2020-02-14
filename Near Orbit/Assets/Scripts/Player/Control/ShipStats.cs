using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShipStats {
    public float RollRate;
    public float YawRate;
    public float PitchRate;
    public float ThrustRate;

    public ShipStats() { }

    public ShipStats(float roll, float yaw, float pitch, float thrust) {
        RollRate = roll;
        YawRate = yaw;
        PitchRate = pitch;
        ThrustRate = thrust;
    }
}