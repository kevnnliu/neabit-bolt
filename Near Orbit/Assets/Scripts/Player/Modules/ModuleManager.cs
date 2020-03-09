using System.Collections.Generic;
using UnityEngine;

public class ModuleManager {
    private static readonly List<string> validModules = new List<string>() {
        "Weapons/LaserGun", "Specials/Boost"
    };

    public static T CreateModule<T>(string id, BaseShip ship, Transform mount) where T : MonoBehaviour, IShipModule {
        if (validModules.Contains(id)) {
            GameObject prefab = Resources.Load<GameObject>(id);
            GameObject gameObject = Object.Instantiate(prefab, mount);
            T shipModule = gameObject.GetComponent<T>();
            shipModule.Init(ship);
            return shipModule;
        }
        return null;
    }
}
