using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapGun : IWeapon {

    GameObject ProjPrefab {
        get;
    }

    void Fire(Vector3 target);

}
