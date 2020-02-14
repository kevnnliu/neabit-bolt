using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureInput : IMoveInput {

    private const float throttleMax = 1.2f;
    private const float pointerDistance = 5f;
    private const float pitchYawBorder = 15f;
    private const float rollBorder = 25f;

    private PointAim pointAim;
    private Transform rightController;
    private Transform shipTransform;
    private Vector2 pitchYaw;

    private struct Input {
        public bool weaponActivation, weaponNext, weaponPrev;
        public bool special0, special1, special2;
    }
    private Input prevInput, curInput;

    public GestureInput(Transform shipT) {
        shipTransform = shipT;
        rightController = shipT.Find("OVRCameraRig").Find("TrackingSpace").Find("RightHandAnchor");
        pointAim = new PointAim(shipT);
        UpdateInput();
    }

    public bool ReadInputs {
        get {
            return OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger) > 0.5f;
        }
    }

    public void UpdateInput() {
        pitchYaw = ConvertFromRaw();
        pointAim.UpdateAim();

        prevInput = curInput;
        curInput.weaponActivation = OVRInput.Get(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.RTouch);
        curInput.weaponNext = OVRInput.Get(OVRInput.RawButton.A, OVRInput.Controller.RTouch);
        curInput.weaponPrev = OVRInput.Get(OVRInput.RawButton.B, OVRInput.Controller.RTouch);
        curInput.special0 = OVRInput.Get(OVRInput.RawButton.LIndexTrigger, OVRInput.Controller.LTouch);
        curInput.special1 = OVRInput.Get(OVRInput.RawButton.X, OVRInput.Controller.LTouch);
        curInput.special2 = OVRInput.Get(OVRInput.RawButton.Y, OVRInput.Controller.LTouch);
    }

    public Vector3 GetRotationInput() {
        return new Vector3(GetPitchInput(), GetYawInput(), GetRollInput());
    }

    public float GetThrustInput() {
        // TODO: Change how throttle input works
        float throttle = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick).y;
        return Mathf.Clamp(0.7f + throttle, 0f, throttleMax);
    }

    public int WeaponActivated() {
        return (curInput.weaponActivation ? 1 : 0) - (prevInput.weaponActivation ? 1 : 0);
    }
    
    public bool WeaponNextPressed() {
        return curInput.weaponNext && !prevInput.weaponNext;
    }

    public bool WeaponPrevPressed() {
        return curInput.weaponPrev && !prevInput.weaponPrev;
    }

    public int SpecialActivated(int index) {
        switch (index) {
            case 0:
                return (curInput.special0 ? 1 : 0) - (prevInput.special0 ? 1 : 0);
            case 1:
                return (curInput.special0 ? 1 : 0) - (prevInput.special0 ? 1 : 0);
            case 2:
                return (curInput.special0 ? 1 : 0) - (prevInput.special0 ? 1 : 0);
            default:
                return 0;
        }
    }

    public Vector3 GetReticlePoint() {
        return pointAim.GetReticlePoint();
    }

    public Vector3 GetAimPoint() {
        return pointAim.GetAimPoint();
    }

    private float GetRollInput() {
        float inputReading = rightController.localRotation.eulerAngles.z;
        if (inputReading > 180f) {
            inputReading -= 360f;
        }
        return Smooth(inputReading, rollBorder);
    }

    private float GetYawInput() {
        return Smooth(pitchYaw.x, pitchYawBorder);
    }

    private float GetPitchInput() {
        return Smooth(pitchYaw.y, pitchYawBorder);
    }

    /// <summary>
    /// Smooths input along a polynomial function, multiplied by Time.deltaTime, retains sign.
    /// </summary>
    private float Smooth(float amount, float border) {
        float proportion = Mathf.Clamp(amount, -border, border) / border;
        float smoothed = Mathf.Sign(amount) * Mathf.Pow(proportion, 2f);
        return smoothed;
    }

    /// <summary>
    /// Takes the local Transform of RTouch and converts it to a pointer heading.
    /// </summary>
    private Vector2 ConvertFromRaw() {
        // Get global controller position/rotation
        Vector3 controllerPos = rightController.position;
        Quaternion controllerRot = rightController.rotation;

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
