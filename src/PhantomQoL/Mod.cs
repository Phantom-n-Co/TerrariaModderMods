using PhantomQoL.Features;
using PhantomQoL.Items;
using PhantomQoL.Patches;
using TerrariaModder.Core.Events;
using TerrariaModder.Core.Logging;

namespace PhantomQoL;

public class Mod : IMod, IModLifecycle {
    public string Id      => "phantom-qol";
    public string Name    => "Phantom's QoL";
    public string Version => "1.6.0";

    public static ILogger       _log;
    public static PhantomConfig _config;
    private       string        _modFolder;

    public void Initialize(ModContext ctx) {
        _log       = ctx.Logger;
        _config    = ctx.GetConfig<PhantomConfig>();
        _modFolder = ctx.ModFolder;

        Keybinds.Init(ctx);
        ItemMagnet.Init(ctx);

        _log.Info("Phantom's QoL initialized.");
    }

    // private void OnNPCSpawn(NPCSpawnEventArgs args) {
    //     if (!_config.SpawnToggleEnabled) return;
    //     int baseType = NPCID.FromNetId(args.NPCType);
    //
    //     bool blocked = SpawnToggleData.Disabled.Contains(baseType);
    //
    //     if (!blocked && SpawnToggleData.BestiaryTypes.Count > 0 && !SpawnToggleData.BestiaryTypes.Contains(baseType)) {
    //         int banner = BannerSystem.NPCtoBanner(baseType);
    //         if (banner != 0)
    //             blocked = SpawnToggleData.Disabled.Any(d => BannerSystem.NPCtoBanner(d) == banner);
    //     }
    //
    //     if (!blocked) return;
    //     Main.npc[args.NPCIndex].active = false;
    // }

    public void OnContentReady(ModContext context) {
        ItemMagnet.RegisterSwap();
    }

    public void OnWorldLoad() {
        SpawnToggleData.SetContext(_modFolder, Main.worldName);
        SpawnToggleData.Load();

        // PlayerEvents.OnPlayerUpdate += ItemMagnet.MagnetPull.OnPlayerUpdate; // not yet implemented in framework
        FrameEvents.OnPostUpdate += ItemMagnet.MagnetPull;
        FrameEvents.OnPostUpdate += TileBuffs.ApplyBuffs;
        //NPCEvents.OnNPCSpawn += OnNPCSpawn;
    }

    public void OnWorldUnload() {
        SpawnToggleData.Save();
        SpawnToggleData.Disabled.Clear();

        // PlayerEvents.OnPlayerUpdate -= ItemMagnet.MagnetPull.OnPlayerUpdate;
        FrameEvents.OnPostUpdate -= ItemMagnet.MagnetPull;
        FrameEvents.OnPostUpdate -= TileBuffs.ApplyBuffs;
        //NPCEvents.OnNPCSpawn -= OnNPCSpawn;
    }

    public void OnConfigChanged() { }

    public void Unload() { }
}