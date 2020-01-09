using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement {

    private Vector3 newPosition;
    private Quaternion newRotation;
    private float speedFactor = 1f;

    /// <summary>
    /// Computes a new Transform from an IMoveInput instance and the current Transform.
    /// </summary>
    public void ComputeNewTransform(Transform shipT, IMoveInput moveInput) {
        newRotation = shipT.rotation * moveInput.GetRotationInput();;
        newPosition = shipT.position + (shipT.forward * moveInput.GetThrustInput());

        Vector3 diff = newPosition - shipT.position;
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
