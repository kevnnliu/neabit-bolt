using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjBullet : IProjectile {

    /// <summary>
    /// Create impact object prefab (sparks, etc.) when Hit() is called.
    /// </summary>
    GameObject ImpactPrefab {
        get;
    }

}
