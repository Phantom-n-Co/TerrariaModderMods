using HarmonyLib;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(SceneMetrics), nameof(SceneMetrics.Scan))]
public static class BannerBuffsPatch {
    [HarmonyPostfix]
    public static void PostFix() {
        if (!_config.BannerBuff) return;

        var claimableBannerCounts = BannerSystem.GetClaimableBannerCounts();
        for (var banner = 1; banner < claimableBannerCounts.Length; ++banner) {
            var num = claimableBannerCounts[banner];
            if (num <= 0) continue;

            Main.SceneMetrics.NPCBannerBuff[banner] = true;
            Main.SceneMetrics.hasBanner             = true;
        }
    }
}