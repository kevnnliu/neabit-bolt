using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInput : IMoveInput {

    private const float throttleMax = 1.2f;
    private const float pointerDistance = 5f;
    private const float pitchYawBorder = 15f;
    private const float rollBorder = 25f;

    private float rollCoeff;
    private float yawCoeff;
    private float pitchCoeff;
    private float thrustCoeff;
    private Transform shipTransform;
    private Vector2 pitchYaw;

    public GestureInput(float rCoeff, float yCoeff, float pCoeff, float thrust, Transform shipT) {
        rollCoeff = rCoeff;
        yawCoeff = yCoeff;
        pitchCoeff = pCoeff;
        thrustCoeff = thrust;
        ProcessRawInput(shipT);
    }

    public bool ReadInputs {
        get {
            return OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) > 0.5f;
        }
    }

    public void ProcessRawInput(Transform shipT) {
        shipTransform = shipT;
        pitchYaw = ConvertFromRaw();
    }

    public Quaternion GetRotationInput() {
        return Quaternion.Euler(GetPitchInput(), GetYawInput(), GetRollInput());
    }

    public float GetThrustInput() {
        float throttle = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y;
        return Mathf.Clamp(0.7f + throttle, 0f, throttleMax) * thrustCoeff * Time.deltaTime;
    }

    private float GetRollInput() {
        float inputReading = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles.z;
        if (inputReading > 180f) {
            inputReading -= 360f;
        }
        return Smooth(inputReading, rollCoeff, rollBorder);
    }

    private float GetYawInput() {
        return Smooth(pitchYaw.x, yawCoeff, pitchYawBorder);
    }

    private float GetPitchInput() {
        return Smooth(pitchYaw.y, pitchCoeff, pitchYawBorder);
    }

    /// <summary>
    /// Smooths input along a polynomial function, multiplied by Time.deltaTime, retains sign.
    /// </summary>
    private float Smooth(float amount, float coeff, float border) {
        float proportion = Mathf.Clamp(amount, -border, border) / border;
        float smoothed = Mathf.Sign(amount) * Mathf.Pow(proportion, 2f);
        return smoothed * coeff * Time.deltaTime;
    }

    /// <summary>
    /// Takes the local Transform of RTouch and converts it to a pointer heading.
    /// </summary>
    private Vector2 ConvertFromRaw() {
        // Get local controller position/rotation
        Vector3 controllerPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion controllerRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // Convert to global
        controllerPos = shipTransform.TransformPoint(controllerPos);
        controllerRot = shipTransform.rotation * controllerRot;

        // Compute pointer vector
        Vector3 pointerDirection = controllerRot * Vector3.forward;
        Vector3 pointerTarget = controllerPos + (pointerDirection * pointerDistance);
        Vector3 pointerVector = pointerTarget - shipTransform.position;

        // Compute projections and angles
        Vector3 pointerXProj = Vector3.ProjectOnPlane(pointerVector, shipTransform.up);
        Vector3 pointerYProj = Vector3.ProjectOnPlane(pointerVector, shipTransform.right);
        float x = Vector3.SignedAngle(shipTransform.forward, pointerXProj, shipTransform.up);
        float y = Vector3.SignedAngle(shipTransform.forward, pointerYProj, shipTransform.right);

        return new Vector2(x, y);
    }

}
