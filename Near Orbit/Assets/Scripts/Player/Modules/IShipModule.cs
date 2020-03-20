using UnityEngine;

/// <summary>
/// OBSOLETE.
/// </summary>
public interface IShipModule {
    /// <summary>
    /// Type of module, either Weapon or Special.
    /// </summary>
    ModuleType Type { get; }

    /// <summary>
    /// Returns priority of module to determine which module will activate
    /// when multiple inputs are pressed.
    /// </summary>
    // int Priority { get; }

    /// <summary>
    /// Returns if the module's Update function should be called.
    /// </summary>
    bool IsActive { get; }

    void Init(BaseShip owner);

    void Activate();
    void Update();
    void Deactivate();
}

public enum ModuleType {
    Weapon,
    Special
}
