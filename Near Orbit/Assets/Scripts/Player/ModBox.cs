using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[System.Serializable]
public class ModBox {

    private List<IShipMod> slots;
    private int equippedIndex;
    private int boxCap;
    private string shipName;

    public ModBox(int cap, string shipN) {
        slots = new List<IShipMod>(cap + 1); // Extra space, shouldn't need to resize
        equippedIndex = 0;
        boxCap = cap;
        shipName = shipN;
    }

    public int GetBoxCap() {
        return boxCap;
    }

    /// <summary>
    /// Activates a mod or cycles through mods depending on the input index.
    /// </summary>
    public void ActivateMod(Movement movement, BaseShip ship, IShip properties, int inputIndex) {
        if (slots[inputIndex].IsPassive) {
            slots[inputIndex].Activate(movement, properties, ship);
        }
        else {
            switch (inputIndex) {
                case 0:
                    slots[equippedIndex].Activate(movement, properties, ship);
                    break;

                case 1:
                    equippedIndex += 1;
                    if (equippedIndex >= slots.Capacity) {
                        equippedIndex = 0;
                    }
                    break;

                default:
                    break;
            }
        }
    }

    public void EquipMod(IShipMod mod, int index) {
        slots[index] = mod;
        SortSlots();
    }

    public void RemoveMod(int index) {
        slots.RemoveAt(index);
        SortSlots();
    }

    /// <summary>
    /// Ensures active mods are at the front and preserves order.
    /// </summary>
    private void SortSlots() {
        List<IShipMod> sorted = new List<IShipMod>(slots.Capacity);
        for (int i = 0; i < slots.Capacity; i++) {
            if (!slots[i].IsPassive) {
                sorted.Add(slots[i]);
            }
        }
        for (int j = 0; j < slots.Capacity; j++) {
            if (slots[j].IsPassive) {
                sorted.Add(slots[j]);
            }
        }
        slots = sorted;
    }

}
