using HarmonyLib;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(TeleportPylonsSystem))]
public static class PylonsPatch {
    public static bool _runningTeleportRequest;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(TeleportPylonsSystem.IsPlayerNearAPylon))]
    public static bool Prefix(Player player, ref bool __result) {
        if (!_config.PylonTweaks) return true;
        __result = true;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("HowManyNPCsDoesPylonNeed")]
    public static bool Prefix(TeleportPylonInfo info, Player player, ref int __result) {
        if (!_config.PylonTweaks) return true;
        __result = 0;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(TeleportPylonsSystem.HandleTeleportRequest))]
    public static void HandleTeleportRequestPrefix() {
        _runningTeleportRequest = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(TeleportPylonsSystem.HandleTeleportRequest))]
    public static void HandleTeleportRequestPostfix() {
        _runningTeleportRequest = false;
    }
}

[HarmonyPatch(typeof(Player), nameof(Player.IsInTileInteractionRange))]
public static class PylonsPlayerPatch {
    [HarmonyPrefix]
    public static bool Prefix(ref bool __result) {
        if (!PylonsPatch._runningTeleportRequest && _config.PylonTweaks) return true;
        __result = true;
        return false;
    }
}