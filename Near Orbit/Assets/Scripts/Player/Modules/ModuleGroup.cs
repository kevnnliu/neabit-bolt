using System.Collections.Generic;

/// <summary>
/// OBSOLETE. Contains all of the modules installed in a ship.
/// Weapons can only be fired if they are selected, while
/// specials can be used directly from their slot.
/// </summary>
public class ModuleGroup {
    private const int MAX_WEAPONS = 3;
    private const int MAX_SPECIALS = 3;

    private List<IShipModule> weapons;
    private List<IShipModule> specials;

    private int equipped = 0;

    public ModuleGroup() {
        weapons = new List<IShipModule>();
        specials = new List<IShipModule>();
    }

    public void Update(IMoveInput input) {
        // Process weapon inputs
        //if (input.WeaponNextPressed()) {
        //    deactivateWeapons();
        //    equipped = (equipped + 1) % weapons.Count;
        //} else if (input.WeaponPrevPressed()) {
        //    deactivateWeapons();
        //    equipped = (equipped - 1) % weapons.Count;
        //} else if (input.WeaponActivated() > 0) {
        //    weapons[equipped].Activate();
        //} else if (input.WeaponActivated() < 0) {
        //    deactivateWeapons();
        //}
        // Process special inputs
        for (int i = 0; i < specials.Count; i++) {
            if (input.SpecialActivated(i) > 0) {
                specials[i].Activate();
            } else if (input.SpecialActivated(i) < 0) {
                specials[i].Deactivate();
            }
        }
        // Process weapon logic
        foreach (IShipModule weapon in weapons) {
            if (weapon.IsActive) {
                weapon.Update();
            }
        }
        // Process special logic
        foreach (IShipModule special in specials) {
            if (special.IsActive) {
                special.Update();
            }
        }
    }

    public IShipModule AddModule(IShipModule module) {
        if (module == null) {
            return null;
        }
        if (module.Type == ModuleType.Weapon && weapons.Count < MAX_WEAPONS) {
            weapons.Add(module);
            return module;
        } else if (module.Type == ModuleType.Special && specials.Count < MAX_SPECIALS) {
            specials.Add(module);
            return module;
        }
        return null;
    }

    private void deactivateWeapons() {
        foreach (IShipModule weapon in weapons) {
            weapon.Deactivate();
        }
    }
}