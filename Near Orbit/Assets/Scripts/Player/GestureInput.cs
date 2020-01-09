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
    private Quaternion pointerRotation;
    private bool _readInputs = false;

    public GestureInput(float rRate, float yRate, float pRate, Transform shipT) {
        rollRate = rRate;
        yawRate = yRate;
        pitchRate = pRate;
        ProcessRawInput(shipT);
    }

    public bool ReadInputs {
        get {
            return OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) > 0.5f;
        }
    }

    public void ProcessRawInput(Transform shipT) {
        shipTransform = shipT;
        pointerRotation = ConvertFromRaw();
    }

    public Quaternion GetRotationInput() {
        return Quaternion.Euler(GetPitchInput(), GetYawInput(), GetRollInput());
    }

    public float GetThrustInput() {
        float throttle = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
        return Mathf.Clamp(1 + throttle, 0f, throttleMax) * Time.deltaTime;
    }

    private float GetRollInput() {
        return Mathf.Clamp(pointerRotation.eulerAngles.z, -rollRate, rollRate) * Time.deltaTime;
    }

    private float GetYawInput() {
        return Mathf.Clamp(pointerRotation.eulerAngles.y, -yawRate, yawRate) * Time.deltaTime;
    }

    private float GetPitchInput() {
        return Mathf.Clamp(pointerRotation.eulerAngles.x, -pitchRate, pitchRate) * Time.deltaTime;
    }

    /// <summary>
    /// Takes the local Transform of RTouch and converts it to a pointer heading.
    /// </summary>
    private Quaternion ConvertFromRaw() {
        // Get local Transform and compute pointer direction in local space
        Vector3 localControllerPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion localControllerRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        Vector3 pointerDirection = localControllerRot * Vector3.forward;

        // Convert pointer target point from local to world space
        Vector3 localPointerTarget = localControllerPos + (pointerDirection * pointerDistance);
        Vector3 worldPointerTarget = shipTransform.TransformPoint(localPointerTarget);
        
        // Compute forward vector and pointer target point vector relative to ship position
        Vector3 relativeVector = worldPointerTarget - shipTransform.position;
        Vector3 forwardVector = shipTransform.position + (shipTransform.forward * relativeVector.magnitude);

        // Convert world rotation to local rotation
        Quaternion worldRelativeRotation = Quaternion.FromToRotation(forwardVector, relativeVector);
        Quaternion localRelativeRotation = Quaternion.Inverse(shipTransform.rotation) * worldRelativeRotation;

        return localRelativeRotation;
    }

}
