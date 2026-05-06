using TerrariaModder.Core.Config;

namespace PhantomQoL;

public class PhantomConfig : ModConfig {
    public override int Version => 1;

    [Client, Label("Open All Crates"),
     FormerlySerializedAs("OpenAllCratesEnabled"),
     Description("Open entire stack of crates/bags when opened in the inventory.")]
    public bool OpenAllCrates { get; set; } = true;

    [Client, Label("Banner Buff"),
     FormerlySerializedAs("BannerBuffEnabled"),
     Description("Claimable Banners in the Banner UI apply their buff at all times.")]
    public bool BannerBuff { get; set; } = true;

    [Client, Label("Banner Quick Stack"),
     FormerlySerializedAs("QuickStackBannersEnabled"),
     Description(
         "Quick Stack sends banner items from your inventory into the claimable banner pool.")]
    public bool QuickStackBanners { get; set; } = true;

    [Client, Label("No Cracked Dungeon Bricks"),
     FormerlySerializedAs("NoCrackedDungeonBricksEnabled"),
     Description("Prevents cracked dungeon bricks from generating during world creation.")]
    public bool NoCrackedDungeonBricks { get; set; } = true;

    [Client, Label("Bestiary Spawn Toggle"),
     FormerlySerializedAs("SpawnToggleEnabled"),
     Description(
         "Right-click NPC entries in the bestiary to toggle their spawning (requires one banner's worth of kills).")]
    public bool BestiaryToggle { get; set; } = true;

    [Client, Label("Pylon Tweaks"),
     Description("Tweaks:"                      +
                 "\n - No distance requirement" +
                 "\n - No TownNPC requirement")]
    public bool PylonTweaks { get; set; } = true;

    [Server, RestartRequired, Label("Item Magnet"),
     Description("Disabling will not remove the item,"         +
                 "\nbut it will not be available for purchase" +
                 "\nand the item will not have any effect")]
    public bool ItemMagnet { get; set; } = true;

    [Client, Label("Instant Angler Refresh"),
     Description("Instantly refresh Angler Quest after completion")]
    public bool InstantAnglerRefresh { get; set; } = true;

    [Client, Label("Tile Buffs From Inventory"),
     Description("Campfire, Heart Lantern, Peace Candle, Star in a Bottle, Sunflower, and Cat Bast" +
                 "\napply their buff while in your inventory or any bank/vault.")]
    public bool TileBuffsFromInventory { get; set; } = true;
}