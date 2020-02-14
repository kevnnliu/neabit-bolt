using System;
using System.Collections.Generic;
using System.Reflection;

public class ModuleCollection {
    private static Dictionary<string, Type> moduleTypes;

    public static Dictionary<Type, string> modulePath;

    public static IShipModule InstallModule(BaseShip owner, string id) {
        switch (id) {
            case "laser":
                return new LaserBlasterWeapon(owner);
            // TODO: Add more cases
            default:
                return null;
        }
    }

    public static IShipModule InstallModuleDynamic(BaseShip owner, string id) {
        if (moduleTypes.ContainsKey(id)) {
            Type moduleType = moduleTypes[id];
            ConstructorInfo constructor = moduleType.GetConstructor(new Type[] { owner.GetType() });
            // TODO: Dynamic module constructor loading
        }
        return null;
    }
}
