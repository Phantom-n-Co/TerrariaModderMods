using PhantomQoL.Patches;
using Terraria;
using TerrariaModder.Core;
using TerrariaModder.Core.Input;
using TerrariaModder.Core.Logging;

namespace PhantomQoL;

public class Mod : IMod, IModLifecycle {
    public string Id      => "phantom-qol";
    public string Name    => "Phantom's QoL";
    public string Version => "1.3.0";

    public static ILogger       _log;
    public static PhantomConfig _config;
    private       string        _modFolder;
    private       ModContext    _ctx;

    public void Initialize(ModContext context) {
        _log       = context.Logger;
        _config    = context.GetConfig<PhantomConfig>();
        _modFolder = context.ModFolder;
        _ctx       = context;

        context.RegisterKeybind("remove-cracked-bricks", "Remove Cracked Dungeon Bricks",
            "Scans the world and removes cracked dungeon bricks", new KeyCombo(KeyCode.None),
            CrackedDungeonBrickPatch.RemoveExistingCrackedBricks);

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

    public void OnContentReady(ModContext context) { }

    public void OnWorldLoad() {
        SpawnToggleData.SetContext(_modFolder, Main.worldName);
        SpawnToggleData.Load();

        //NPCEvents.OnNPCSpawn += OnNPCSpawn;
    }

    public void OnWorldUnload() {
        SpawnToggleData.Save();
        SpawnToggleData.Disabled.Clear();

        //NPCEvents.OnNPCSpawn -= OnNPCSpawn;
    }

    public void OnConfigChanged() { }

    public void Unload() { }
}