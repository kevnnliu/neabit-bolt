using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShip : MonoBehaviour {

    #region Constants

    const float rollRate = 10f;
    const float yawRate = 10f;
    const float pitchRate = 10f;

    #endregion

    protected IMoveInput moveInput;
    protected Movement movement;
    protected ModBox weapons;
    protected ModBox specials;

    /// <summary>
    /// Called by projectile or beam to show hitmarker on hit.
    /// </summary>
    public void HitMarker() {
        // TODO: Implement showing hitmarker
    }

    /// <summary>
    /// Returns the Vector3 that is being aimed at.
    /// </summary>
    public Vector3 AimTarget() {
        // TODO: Return the transform to shoot at
        throw new System.Exception("Not implemented!");
    }

    protected void LoadBaseShip() {
        moveInput = new GestureInput(rollRate, yawRate, pitchRate, transform);
        movement = new Movement(transform);
        // TODO: Load ModBox instances
    }

    protected void CalculateMovement() {
        ConvertInputs(moveInput);
        ApplyMovement(movement);
    }

    /// <summary>
    /// Processes an IMoveInput instance and updates the ship's Movement instance.
    /// </summary>
    void ConvertInputs(IMoveInput moveInput) {
        if (moveInput.ReadInputs) {
            moveInput.ProcessRawInput(transform);
            movement.ComputeNewTransform(transform, moveInput);
        }
    }

    /// <summary>
    /// Applies the Movement instance by updating the ship's Transform component.
    /// </summary>
    void ApplyMovement(Movement movement) {
        if (movement != null) {
            transform.position = movement.GetNewPosition();
            transform.rotation = movement.GetNewRotation();
        }
    }

}
