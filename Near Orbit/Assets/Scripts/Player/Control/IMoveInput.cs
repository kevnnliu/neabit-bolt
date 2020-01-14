using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveInput {

    /// <summary>
    /// Usually always true except for no-idle inputs like GestureInput.
    /// </summary>
    bool ReadInputs {
        get;
    }

    Quaternion GetRotationInput();

    float GetThrustInput();

    /// <summary>
    /// Convert raw input to scaled values, relative to the ship transform if necessary.
    /// </summary>
    void ProcessRawInput(Transform shipT);

}
