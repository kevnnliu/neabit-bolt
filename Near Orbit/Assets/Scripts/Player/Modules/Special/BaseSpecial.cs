using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpecial : IShipModule {
    private float cooldown;

    public ModuleType Type => ModuleType.Special;

    public bool IsActive => throw new System.NotImplementedException();

    public void Activate() {
        throw new System.NotImplementedException();
    }

    public void Deactivate() {
        throw new System.NotImplementedException();
    }

    public void Update() {
        throw new System.NotImplementedException();
    }
}
