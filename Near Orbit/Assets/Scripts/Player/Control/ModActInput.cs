using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModActInput {

    public static int WeaponActivated() {
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.RTouch)) {
            return 0;
        } else if (OVRInput.Get(OVRInput.RawButton.A, OVRInput.Controller.RTouch)) {
            return 1;
        } else if (OVRInput.Get(OVRInput.RawButton.B, OVRInput.Controller.RTouch)) {
            return 2;
        }
        return int.MaxValue;
    }

    public static int SpecialActivated() {
        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger, OVRInput.Controller.LTouch)) {
            return 0;
        } else if (OVRInput.Get(OVRInput.RawButton.X, OVRInput.Controller.LTouch)) {
            return 1;
        } else if (OVRInput.Get(OVRInput.RawButton.Y, OVRInput.Controller.LTouch)) {
            return 2;
        }
        return int.MaxValue;
    }

}
