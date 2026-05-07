using System.Reflection;

namespace PhantomQoL.Patches;

public class OpenCratesPatch {
    public static void PostFix(Item[] inv, int context, int slot, Player player) {
        if (!_config.OpenAllCrates || inv[slot].stack <= 0) return;

        var method = typeof(ItemSlot).GetMethod("TryOpenContainer",
            BindingFlags.Static | BindingFlags.NonPublic);
        method?.Invoke(null, [inv, context, slot, player]);
    }
}