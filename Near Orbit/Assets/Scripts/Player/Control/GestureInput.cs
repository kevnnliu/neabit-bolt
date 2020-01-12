using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInput : IMoveInput {

    private const float throttleMax = 1.25f;
    private const float pointerDistance = 5f;

    private float rollRate;
    private float yawRate;
    private float pitchRate;
    private Transform shipTransform;
    private Vector2 pitchYaw;

    public GestureInput(float rRate, float yRate, float pRate, Transform shipT) {
        rollRate = rRate;
        yawRate = yRate;
        pitchRate = pRate;
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
        return Mathf.Clamp(1 + throttle, 0f, throttleMax) * Time.deltaTime;
    }

    private float GetRollInput() {
        float inputReading = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles.z;
        if (inputReading > 180f) {
            inputReading -= 360f;
        }
        return Mathf.Clamp(inputReading, -rollRate, rollRate) * Time.deltaTime;
    }

    private float GetYawInput() {
        return Mathf.Clamp(pitchYaw.x, -yawRate, yawRate) * Time.deltaTime;
    }

    private float GetPitchInput() {
        return Mathf.Clamp(pitchYaw.y, -pitchRate, pitchRate) * Time.deltaTime;
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
