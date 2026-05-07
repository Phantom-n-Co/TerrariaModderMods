namespace PhantomQoL.Patches;

public class SortedBannerBuffsPatch {
    public static bool Prefix() {
        return !Main.bannerMouseOver || !_config.SortedBannerWindow;
        // suppress vanilla tooltip — BannerWindow.OnDraw handles display
    }
}