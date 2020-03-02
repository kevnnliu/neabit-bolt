//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ModSelect {

//    private List<IShipModule> slots;

//    public void EquipMod(IShipModule mod, int index) {
//        slots[index] = mod;
//        SortSlots();
//    }

//    public void RemoveMod(int index) {
//        slots.RemoveAt(index);
//        SortSlots();
//    }

//    /// <summary>
//    /// Ensures active mods are at the front and preserves order.
//    /// </summary>
//    private void SortSlots() {
//        List<IShipModule> sorted = new List<IShipModule>(slots.Capacity);
//        for (int i = 0; i < slots.Count; i++) {
//            if (!slots[i].IsPassive) {
//                sorted.Add(slots[i]);
//            }
//        }
//        for (int j = 0; j < slots.Count; j++) {
//            if (slots[j].IsPassive) {
//                sorted.Add(slots[j]);
//            }
//        }
//        slots = sorted;
//    }

//}
