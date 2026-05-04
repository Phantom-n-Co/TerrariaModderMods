using System.Reflection;
using HarmonyLib;
using Terraria;
using static PhantomQoL.Mod;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(Terraria.UI.ItemSlot), "TryOpenContainer")]
public static class OpenCratesPatch {
    [HarmonyPostfix]
    public static void PostFix(Item[] inv, int context, int slot, Player player) {
        if (!_config.OpenAllCratesEnabled || inv[slot].stack <= 0) return;

        var method = typeof(Terraria.UI.ItemSlot).GetMethod("TryOpenContainer",
            BindingFlags.Static | BindingFlags.NonPublic);
        method?.Invoke(null, [inv, context, slot, player]);
    }
}