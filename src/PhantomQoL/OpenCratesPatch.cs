using System.Reflection;
using HarmonyLib;
using Terraria;
using static PhantomQoL.Mod;

namespace PhantomQoL;

[HarmonyPatch(typeof(Terraria.UI.ItemSlot), "TryOpenContainer")]
public static class OpenCratesPatch {
    [HarmonyPostfix]
    public static void PostFix(Item[] inv, int context, int slot, Player player) {
        if (_config.InstanCratesEnabled && inv[slot].stack > 0) {
            var method = typeof(Terraria.UI.ItemSlot).GetMethod("TryOpenContainer",
                BindingFlags.Static | BindingFlags.NonPublic);
            method?.Invoke(null, [inv, context, slot, player]);
        }
    }
}