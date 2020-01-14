using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModPrefab {

    public static Dictionary<Module, string> Path = new Dictionary<Module, string> {
        {Module.LaserGun, "Prefabs/Weapons/LaserGun"},
        {Module.Boost, "Prefabs/Specials/Boost"},
        {Module.Shield, "Prefabs/Specials/Shield"}
    };

}

public enum Module {
    LaserGun,
    Boost,
    Shield
}
