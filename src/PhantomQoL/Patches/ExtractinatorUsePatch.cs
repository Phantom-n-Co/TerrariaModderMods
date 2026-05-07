namespace PhantomQoL.Patches;

public class ExtractinatorUsePatch {
    public static void ExtractinatorUsePrefix(Player __instance) {
        if (!_config.FastExtractinator) return;
        __instance.itemAnimation = 1;
        __instance.itemTime      = 1;
        __instance.itemTimeMax   = 1;
    }
}