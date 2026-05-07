namespace PhantomQoL.Patches;

public class PylonsPatch {
    [ThreadStatic] private static bool _runningTeleportRequest;

    public static bool IsPlayerNearAPylonPrefix(ref bool __result) {
        if (!_config.PylonTweaks) return true;
        __result = true;
        return false;
    }

    public static bool NpcCheckPrefix(ref bool __result) {
        if (!_config.PylonTweaks) return true;
        __result = true;
        return false;
    }

    public static void HandleTeleportRequestPrefix() {
        _runningTeleportRequest = true;
    }

    public static void HandleTeleportRequestPostfix() {
        _runningTeleportRequest = false;
    }

    public static bool HasPylonOfTypePrefix(ref bool __result) {
        // require skeletron to be beaten for unlimited pylon placement
        if (!NPC.downedBoss3 || !_config.PylonTweaks) return true;
        __result = false;
        return false;
    }

    public static bool InTileEntityInteractionRangePrefix(ref bool __result) {
        if (!_runningTeleportRequest || !_config.PylonTweaks) return true;
        __result = true;
        return false;
    }

    public static bool PlayerInteractionRangePrefix(ref bool __result) {
        if (!_runningTeleportRequest || !_config.PylonTweaks) return true;
        __result = true;
        return false;
    }
}