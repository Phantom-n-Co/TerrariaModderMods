using System;
using System.Threading;
using HarmonyLib;
using Terraria;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace PhantomQoL;

public class Mod : IMod {
    public string Id      => "phantom-qol";
    public string Name    => "Phantom's QoL";
    public string Version => "1.0.0";

    private const string harmonyId = "phantom.qol";

    public static ILogger       _log;
    public static PhantomConfig _config;

    public void Initialize(ModContext context) {
        _log    = context.Logger;
        _config = context.GetConfig<PhantomConfig>();

        _log.Info("Phantom's QoL initialized.");

        context.RegisterKeybind("toggle-instant-crates", "Toggle Instant Crates",
            "Toggles the instant crate opening feature", "F9", OnTogglePressed);
    }

    public void OnConfigChanged() {
        _log.Info("Config changed - reloading settings");
    }

    private void OnTogglePressed() {
        _config.InstanCratesEnabled = !_config.InstanCratesEnabled;
        _config.Save();
        _log.Info($"Feature is now: {(_config.InstanCratesEnabled ? "ENABLED" : "DISABLED")}");
    }

    public void Unload() { }
}