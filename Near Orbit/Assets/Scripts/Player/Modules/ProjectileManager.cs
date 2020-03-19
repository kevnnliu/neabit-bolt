using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager {
    private static Dictionary<string, Type> projectileTypes = new Dictionary<string, Type>() {
        { "laser", typeof(ProjLaser) }
    };

    public static Dictionary<Type, string> prefabPaths = new Dictionary<Type, string>() {
        { typeof(ProjLaser), "Prefabs/Weapons/Projectiles/Laser" }
    };

    public static void SpawnProjectile<T>(BaseShip owner, Vector3 position, Quaternion rotation) {
        Type projectileType = typeof(T);
        if (projectileTypes.ContainsValue(projectileType)) {
            GameObject prefab = Resources.Load<GameObject>(prefabPaths[projectileType]);
            GameObject gameObject = UnityEngine.Object.Instantiate(prefab, position, rotation);

            gameObject.GetComponent<BaseProj>().SetOwner(owner);
        }
    }

    public static void SpawnProjectile(string id, BaseShip owner, Vector3 position, Quaternion rotation) {
        if (projectileTypes.ContainsKey(id)) {
            Type projectileType = projectileTypes[id];
            GameObject prefab = Resources.Load<GameObject>(prefabPaths[projectileType]);
            GameObject gameObject = UnityEngine.Object.Instantiate(prefab, position, rotation);

            gameObject.GetComponent<BaseProj>().SetOwner(owner);
        }
    }
}

public enum ProjectileType {
    MachineGunBullet,
    Laser
}