using HarmonyLib;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace PhantomQoL;

public class Mod : IMod {
    public string Id      => "phantom-qol";
    public string Name    => "Phantom's QoL";
    public string Version => "1.2.0";

    private const string harmonyId = "dev.wp.qol";

    public static ILogger       _log;
    public static PhantomConfig _config;
    private       Harmony       _harmony;

    public void Initialize(ModContext context) {
        _log     = context.Logger;
        _config  = context.GetConfig<PhantomConfig>();
        _harmony = new Harmony(harmonyId);

        context.RegisterKeybind("remove-cracked-bricks", "Remove Cracked Dungeon Bricks",
            "Scans the world and removes cracked dungeon bricks", "None",
            CrackedDungeonBrickPatch.RemoveExistingCrackedBricks);

        _log.Info("Phantom's QoL initialized.");
    }

    public void OnConfigChanged() { }

    public void Unload() {
        _harmony.UnpatchAll(harmonyId);
    }
}