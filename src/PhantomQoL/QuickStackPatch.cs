using System;
using System.Collections.Generic;
using HarmonyLib;
using Terraria;
using Terraria.GameContent;
using static PhantomQoL.Mod;

namespace PhantomQoL;

[HarmonyPatch(typeof(QuickStacking), nameof(QuickStacking.QuickStackToNearbyInventories))]
public static class QuickStackPatch {
    private static readonly Dictionary<int, int> _itemToBanner = BuildLookup();

    private static Dictionary<int, int> BuildLookup() {
        var map = new Dictionary<int, int>();
        for (int i = 1; i < BannerSystem.MaxBannerTypes; i++) {
            int itemType = BannerSystem.BannerToItem(i);
            if (itemType > 0 && !map.ContainsKey(itemType))
                map[itemType] = i;
        }
        return map;
    }

    [HarmonyPrefix]
    public static void Prefix(Player player) {
        if (!_config.QuickStackBannersEnabled) return;

        var counts = BannerSystem.GetClaimableBannerCounts();
        bool anyAdded = false;

        // Slots 10-49 are main inventory (0-9 is hotbar) — matches vanilla QuickStack scope
        for (int i = 10; i < 50; i++) {
            Item item = player.inventory[i];
            if (item.IsAir || item.favorited) continue;
            if (!_itemToBanner.TryGetValue(item.type, out int bannerType)) continue;

            int room = ushort.MaxValue - counts[bannerType];
            int toAdd = Math.Min(item.stack, room);
            if (toAdd <= 0) continue;

            counts[bannerType] += (ushort)toAdd;
            item.stack -= toAdd;
            if (item.stack <= 0) item.TurnToAir();
            anyAdded = true;
        }

        if (anyAdded) BannerSystem.AnyNewClaimableBanners = true;
    }
}
