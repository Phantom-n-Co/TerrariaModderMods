using System.Linq;
using HarmonyLib;

namespace PhantomQoL.Patches;

public class GardenGnomePatch {
    private static readonly Action<SceneMetrics, bool> _setHasGardenGnome =
        AccessTools.MethodDelegate<Action<SceneMetrics, bool>>(
            AccessTools.PropertySetter(typeof(SceneMetrics), "HasGardenGnome"));

    public static void Postfix() {
        if (Main.netMode == 2) return;
        if (!_config.TileBuffsFromInventory) return;
        var player = Main.player[Main.myPlayer];
        if (player == null || player.dead) return;
        if (HasGnome(player))
            _setHasGardenGnome(Main.SceneMetrics, true);
    }

    private static bool HasGnome(Player player) =>
        ContainsGnome(player.inventory)  ||
        ContainsGnome(player.bank.item)  ||
        ContainsGnome(player.bank2.item) ||
        ContainsGnome(player.bank3.item) ||
        ContainsGnome(player.bank4.item);

    private static bool ContainsGnome(Item[] items) {
        return items.Any(item => item is { type: ItemID.GardenGnome });
    }
}