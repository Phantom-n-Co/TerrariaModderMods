using HarmonyLib;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(Player), "ExtractinatorUse")]
public class ExtractinatorUsePatch {
    [HarmonyPrefix]
    public static void Prefix(Player __instance) {
        if (!_config.FastExtractinator) return;
        __instance.itemAnimation = 1;
        __instance.itemTime      = 1;
        __instance.itemTimeMax   = 1;
    }
}