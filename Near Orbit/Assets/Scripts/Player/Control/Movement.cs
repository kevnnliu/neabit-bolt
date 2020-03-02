using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement {

    private Vector3 newPosition;
    private Quaternion newRotation;
    private ShipStats stats;
    private float speedFactor = 1f;

    public Movement(ShipStats shipStats, Transform shipT) {
        stats = shipStats;

        newPosition = shipT.position;
        newRotation = shipT.rotation;
    }

    /// <summary>
    /// Computes new position and rotation from an IMoveInput instance and the current Transform.
    /// </summary>
    public void ComputeNewTransform(Transform shipT, IMoveInput moveInput) {
        Vector3 eulerRotation = Vector3.Scale(moveInput.GetRotationInput(),
            new Vector3(stats.PitchRate, stats.YawRate, stats.RollRate));
        Quaternion rotation = Quaternion.Euler(eulerRotation);
        newRotation = shipT.rotation * Quaternion.Slerp(Quaternion.identity, rotation, Time.deltaTime);

        Vector3 diff = Vector3.zero;
        float thrust = moveInput.GetThrustInput() * stats.ThrustRate * Time.deltaTime;

        if (!Physics.Raycast(shipT.position, shipT.forward, thrust)) {
            diff = shipT.forward * thrust;
        }

        newPosition = shipT.position + (diff * speedFactor);

        speedFactor = 1f;
    }

    public Vector3 GetNewPosition() {
        return newPosition;
    }

    public Quaternion GetNewRotation() {
        return newRotation;
    }

    public void AmplifySpeed(float factor) {
        speedFactor *= factor;
    }

}
