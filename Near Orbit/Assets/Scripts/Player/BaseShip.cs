using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShip : MonoBehaviour {

    #region Constants

    const float rollRate = 10f;
    const float yawRate = 10f;
    const float pitchRate = 10f;

    #endregion

    IMoveInput moveInput;
    Movement movement;
    ModBox weapons;
    ModBox specials;

    protected virtual void LoadBaseShip() {
        moveInput = new GestureInput(rollRate, yawRate, pitchRate, transform);
        movement = new Movement();
        // TODO: Load ModBox instances
    }

    protected virtual void CalculateMovement() {
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
