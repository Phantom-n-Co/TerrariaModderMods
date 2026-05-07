namespace PhantomQoL.Patches;

public class BannerBuffsPatch {
    public static void PostFix() {
        if (Main.netMode == 2) return;
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