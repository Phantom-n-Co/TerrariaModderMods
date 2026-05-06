using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using static PhantomQoL.Mod;

namespace PhantomQoL.Patches;

public static class SpawnToggleData {
    public static readonly HashSet<int> Disabled = [];

    private static string _modFolder;
    private static string _worldName;

    public static void SetContext(string modFolder, string worldName) {
        _modFolder = modFolder;
        _worldName = worldName;
    }

    private static string GetWorldFilePath() {
        var folder = Path.Combine(_modFolder, "worlds", SanitizeName(_worldName));
        return Path.Combine(folder, "world.json");
    }

    private static string SanitizeName(string name) {
        if (string.IsNullOrEmpty(name)) return "unknown";
        var invalid = Path.GetInvalidFileNameChars();
        var sb      = new StringBuilder();
        foreach (var c in name)
            sb.Append(Array.IndexOf(invalid, c) < 0 ? c : '_');
        return sb.ToString();
    }

    public static void Load() {
        Disabled.Clear();
        if (string.IsNullOrEmpty(_modFolder) || string.IsNullOrEmpty(_worldName)) return;
        var    path    = GetWorldFilePath();
        string content = null;
        if (File.Exists(path))
            content = File.ReadAllText(path);
        else if (File.Exists(path + ".bak"))
            content = File.ReadAllText(path + ".bak");
        if (content != null) Deserialize(content);
    }

    public static void Save() {
        if (string.IsNullOrEmpty(_modFolder) || string.IsNullOrEmpty(_worldName)) return;
        var path = GetWorldFilePath();
        SafeWrite(path, Serialize());
    }

    private static string Serialize() {
        var ids = string.Join(", ", Disabled.OrderBy(x => x));
        return $"{{\n  \"disabledNpcTypes\": [{ids}]\n}}\n";
    }

    private static void Deserialize(string json) {
        var match = Regex.Match(json, @"""disabledNpcTypes""\s*:\s*\[([^\]]*)\]");
        if (!match.Success) return;
        foreach (var token in match.Groups[1].Value.Split(','))
            if (int.TryParse(token.Trim(), out var id))
                Disabled.Add(id);
    }

    private static void SafeWrite(string path, string content) {
        Directory.CreateDirectory(Path.GetDirectoryName(path) ?? path);
        var tmp = path + ".tmp";
        var bak = path + ".bak";
        File.WriteAllText(tmp, content);
        if (File.Exists(path)) {
            if (File.Exists(bak)) File.Delete(bak);
            File.Move(path, bak);
        }

        File.Move(tmp, path);
    }
}

[HarmonyPatch(typeof(UIBestiaryEntryButton), MethodType.Constructor, typeof(BestiaryEntry), typeof(bool))]
public static class BestiaryButtonCtorPatch {
    [HarmonyPostfix]
    public static void Postfix(UIBestiaryEntryButton __instance, BestiaryEntry entry) {
        var netIdElement = entry.Info.OfType<NPCNetIdBestiaryInfoElement>().FirstOrDefault();
        if (netIdElement == null) return;
        var npcNetId = netIdElement.NetId;
        __instance.OnRightClick += (_, _) => OnRightClick(npcNetId);
    }

    private static void OnRightClick(int npcNetId) {
        if (!_config.BestiaryToggle) return;

        var baseType   = NPCID.FromNetId(npcNetId);
        var bannerType = BannerSystem.NPCtoBanner(baseType);
        if (bannerType != 0) {
            var threshold = ItemID.Sets.KillsToBanner[BannerSystem.BannerToItem(bannerType)];
            var kills     = BannerSystem.GetKillCount(bannerType);
            if (kills < threshold) {
                var name = Lang.GetNPCName(baseType).Value;
                Main.NewText($"[PhantomQoL] Need {threshold - kills} more kills of {name} to toggle spawns.", 255, 200,
                    80);
                return;
            }
        }

        if (!SpawnToggleData.Disabled.Remove(npcNetId))
            SpawnToggleData.Disabled.Add(npcNetId);

        SpawnToggleData.Save();
    }
}

[HarmonyPatch(typeof(UIBestiaryEntryButton), "DrawSelf")]
public static class BestiaryButtonDrawPatch {
    private static readonly FieldInfo BordersField =
        typeof(UIBestiaryEntryButton).GetField("_borders", BindingFlags.NonPublic | BindingFlags.Instance);

    [HarmonyPostfix]
    public static void Postfix(UIBestiaryEntryButton __instance) {
        if (!_config.BestiaryToggle) return;
        var netIdElement = __instance.Entry.Info.OfType<NPCNetIdBestiaryInfoElement>().FirstOrDefault();
        if (netIdElement == null) return;

        var disabled = SpawnToggleData.Disabled.Contains(netIdElement.NetId);

        if (BordersField?.GetValue(__instance) is UIImage borders)
            borders.Color = disabled ? new Color(255, 80, 80) : Color.White;
    }
}

[HarmonyPatch(typeof(NPC), nameof(NPC.NewNPC))]
public static class NewNpcPatch {
    [HarmonyPrefix]
    public static bool Prefix(int Type, ref int __result) {
        if (!_config.BestiaryToggle) return true;
        if (!SpawnToggleData.Disabled.Contains(Type) &&
            !SpawnToggleData.Disabled.Contains(NPCID.FromNetId(Type))) return true;
        __result = Main.maxNPCs;
        if (SpawnAnNpcPatch.InSpawnerContext)
            SpawnAnNpcPatch.RerollNeeded = true;
        return false;
    }
}

/// <summary>
/// Due to how "awful" the spawner code is, we have to do this to lessen the chances of reducing spawn rates due to
/// blocking spawns. I'd like if there was a better way to do this, but this is the best I could come up with.
/// Or, I might just be stupid, idk, don't care. It works:tm:, which is good enough for now.
/// </summary>
[HarmonyPatch(typeof(NPC.Spawner), nameof(NPC.Spawner.SpawnAnNPC))]
public static class SpawnAnNpcPatch {
    private const int MaxRetries = 100;

    [ThreadStatic] public static  bool     InSpawnerContext;
    [ThreadStatic] public static  bool     RerollNeeded;
    [ThreadStatic] private static int      _retryCount;
    [ThreadStatic] private static object   _instance;
    [ThreadStatic] private static object[] _args;

    private static readonly MethodInfo Method =
        typeof(NPC.Spawner).GetMethod("SpawnAnNPC", BindingFlags.Public | BindingFlags.Instance);

    [HarmonyPrefix]
    public static void Prefix(object __instance,
                              int    spawnTileX, int spawnTileY, int spawnTileType, bool xRange, int target) {
        if (_retryCount == 0) {
            InSpawnerContext = true;
            _instance        = __instance;
            _args            = [spawnTileX, spawnTileY, spawnTileType, xRange, target];
        }

        RerollNeeded = false;
    }

    [HarmonyPostfix]
    public static void Postfix() {
        if (RerollNeeded && _retryCount < MaxRetries) {
            _retryCount++;
            RerollNeeded = false;
            Method.Invoke(_instance, _args);
            return;
        }

        _retryCount      = 0;
        InSpawnerContext = false;
        RerollNeeded     = false;
    }
}