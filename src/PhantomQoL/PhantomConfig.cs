using TerrariaModder.Core.Config;

namespace PhantomQoL;

public class PhantomConfig : ModConfig {
    public override int Version => 1;

    [Client, Label("Open All Crates Enabled"),
     Description("When enabled, open entire stack of crates/bags when opened in the inventory.")]
    public bool OpenAllCratesEnabled { get; set; } = true;

    [Client, Label("Banner Buff Enabled"),
     Description("When enabled, banners claimable in the Banner UI apply their buff at all times.")]
    public bool BannerBuffEnabled { get; set; } = true;

    [Client, Label("Quick Stack Banners Enabled"),
     Description(
         "When enabled, pressing Quick Stack sends banner items from your inventory into the claimable banner pool.")]
    public bool QuickStackBannersEnabled { get; set; } = true;
}