using TerrariaModder.Core.Config;

namespace PhantomQoL;

public class PhantomConfig : ModConfig {
    public override int Version => 1;

    [Client, Label("Open All Crates Enabled"),
     Description("Open entire stack of crates/bags when opened in the inventory.")]
    public bool OpenAllCratesEnabled { get; set; } = true;

    [Client, Label("Banner Buff Enabled"),
     Description("Claimable Banners in the Banner UI apply their buff at all times.")]
    public bool BannerBuffEnabled { get; set; } = true;

    [Client, Label("Quick Stack Banners Enabled"),
     Description(
         "Quick Stack sends banner items from your inventory into the claimable banner pool.")]
    public bool QuickStackBannersEnabled { get; set; } = true;

    [Client, Label("No Cracked Dungeon Bricks"),
     Description("Prevents cracked dungeon bricks from generating during world creation.")]
    public bool NoCrackedDungeonBricksEnabled { get; set; } = true;

    [Client, Server, Label("Spawn Toggle Enabled"),
     Description(
         "Right-click NPC entries in the bestiary to toggle their spawning (requires one banner's worth of kills).")]
    public bool SpawnToggleEnabled { get; set; } = true;

    [Client, Label("Pylon Tweaks"),
     Description("Pylons won't need to be near to spawn a player to teleport to them." +
                 "\nChanges pylons to not need Town NPCs near them.")]
    public bool PylonTweaks { get; set; } = true;
}