using HarmonyLib;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Tile_Entities;

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
    [HarmonyPatch("DoesPylonHaveEnoughNPCsAroundIt")]
    public static bool Prefix(TeleportPylonInfo info, int necessaryNPCCount, ref bool __result) {
        if (!_config.PylonTweaks) return true;
        __result = true;
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

    [HarmonyPrefix]
    [HarmonyPatch(nameof(TeleportPylonsSystem.HasPylonOfType))]
    public static bool Prefix(TeleportPylonType pylonType, ref bool __result) {
        if (!NPC.downedBoss3 || !_config.PylonTweaks) return true;
        __result = false;
        return false;
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