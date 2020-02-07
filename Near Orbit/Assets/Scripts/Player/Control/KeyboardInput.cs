using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : IMoveInput
{
    private float mouseMoveSpeed = 0.01f;
    private Vector3 pitchYaw;
    private Vector3 lastPosition;

    public KeyboardInput() {
        lastPosition = new Vector3(Screen.width/2, Screen.height/2);
    }

    public bool ReadInputs {
        get {
            return true;
        }
    }

    public Vector3 GetRotationInput() {
        //pitchYaw += mouseMoveSpeed * (Input.mousePosition - lastPosition);
        //lastPosition = Input.mousePosition;
        pitchYaw = mouseMoveSpeed * (Input.mousePosition - lastPosition);
        pitchYaw.x = Mathf.Clamp(pitchYaw.x, -1, 1);
        pitchYaw.y = Mathf.Clamp(pitchYaw.y, -1, 1);
        return new Vector3(GetPitchInput(), GetYawInput(), GetRollInput());
    }

    public float GetThrustInput() {
        return Input.GetButton("Thrust") ? 1 : 0;
    }

    public void ProcessRawInput(Transform shipT) { }

    private float GetRollInput() {
        return -Input.GetAxis("Horizontal");
    }

    private float GetYawInput() {
        return pitchYaw.x;
    }

    private float GetPitchInput() {
        return -pitchYaw.y;
    }

}
