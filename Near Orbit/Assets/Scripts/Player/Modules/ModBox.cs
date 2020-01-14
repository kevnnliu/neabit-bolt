using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Each ship should have two ModBox instances associated with it, one for weapon mods and
/// one for special mods. These instances should be saved during ship customization and loaded at spawn.
/// </summary>
[System.Serializable]
public class ModBox : MonoBehaviour {

    private IShipMod[] slots;
    private int equippedIndex;

    public ModBox(Transform[] mounts, Module[] mods) {
        equippedIndex = 0;
        slots = new IShipMod[mods.Length];

        for (int i = 0; i < mods.Length; i++) {
            GameObject prefab = Resources.Load<GameObject>(ModPrefab.Path[mods[i]]);
            slots[i] = Instantiate(prefab, mounts[i].position, mounts[i].rotation).GetComponent<IShipMod>();
        }
    }

    /// <summary>
    /// Activates a mod or cycles through mods based on inputIndex, returns true if a mod was activated.
    /// </summary>
    public bool ActivateMod(BaseShip properties, int inputIndex) {
        if (inputIndex >= slots.Length) {
            return false;
        }
        if (slots[inputIndex].IsPassive && properties.SpendEnergy(slots[inputIndex])) {
            slots[inputIndex].Activate(properties);
            return true;
        }
        else {
            switch (inputIndex) {
                case 0:
                    if (properties.SpendEnergy(slots[equippedIndex])) {
                        slots[equippedIndex].Activate(properties);
                        return true;
                    }
                    break;

                case 1:
                    equippedIndex += 1;
                    if (equippedIndex >= slots.Length) {
                        equippedIndex = 0;
                    }
                    break;

                case 2:
                    equippedIndex -= 1;
                    if (equippedIndex < 0) {
                        equippedIndex = slots.Length - 1;
                    }
                    break;

                default:
                    break;
            }
        }
        return false;
    }

}
