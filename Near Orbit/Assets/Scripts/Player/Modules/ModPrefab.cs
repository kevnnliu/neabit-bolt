using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// OBSOLETE.
/// </summary>
public class ModPrefab {

    public static Dictionary<ModuleName, string> Path = new Dictionary<ModuleName, string> {
        {ModuleName.LaserGun, "Prefabs/Weapons/LaserGun"},
        {ModuleName.Boost, "Prefabs/Specials/Boost"},
        {ModuleName.Shield, "Prefabs/Specials/Shield"}
    };

}

public enum ModuleName {
    LaserGun,
    Boost,
    Shield
}
