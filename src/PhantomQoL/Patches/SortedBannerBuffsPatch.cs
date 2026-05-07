using HarmonyLib;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(Main))]
public static class SortedBannerBuffsPatch {
    [HarmonyPrefix]
    [HarmonyPatch("MouseText_DrawBuffTooltip")]
    public static bool Prefix() {
        return !Main.bannerMouseOver || !_config.SortedBannerWindow;
        // suppress vanilla tooltip — BannerWindow.OnDraw handles display
    }
}