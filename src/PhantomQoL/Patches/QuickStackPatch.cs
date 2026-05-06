using HarmonyLib;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(QuickStacking), nameof(QuickStacking.QuickStackToNearbyInventories))]
public static class QuickStackPatch {
    private static readonly Dictionary<int, int> _itemToBanner = BuildLookup();

    private static Dictionary<int, int> BuildLookup() {
        var map = new Dictionary<int, int>();
        for (var i = 1; i < BannerSystem.MaxBannerTypes; i++) {
            var itemType = BannerSystem.BannerToItem(i);
            if (itemType > 0 && !map.ContainsKey(itemType))
                map[itemType] = i;
        }

        return map;
    }

    [HarmonyPrefix]
    public static void Prefix(Player player) {
        if (!_config.QuickStackBanners) return;

        var counts   = BannerSystem.GetClaimableBannerCounts();
        var anyAdded = false;

        // Slots 10-49 are main inventory (0-9 is hotbar) — matches vanilla QuickStack scope
        for (var i = 10; i < 50; i++) {
            var item = player.inventory[i];
            if (item.IsAir || item.favorited) continue;
            if (!_itemToBanner.TryGetValue(item.type, out var bannerType)) continue;

            var room  = ushort.MaxValue - counts[bannerType];
            var toAdd = Math.Min(item.stack, room);
            if (toAdd <= 0) continue;

            counts[bannerType] += (ushort)toAdd;
            item.stack         -= toAdd;
            if (item.stack <= 0) item.TurnToAir();
            anyAdded = true;
        }

        if (anyAdded) BannerSystem.AnyNewClaimableBanners = true;
    }
}