using System.Linq;
using System.Reflection;
using HarmonyLib;
using PhantomQoL.Patches;
using TerrariaModder.Core;

namespace PhantomQoL;

internal static class Patcher {
    // Patches every loaded assembly that contains the type, so both Terraria.exe
    // (client) and TerrariaServer.exe (server, loaded into a different context) get patched.
    private static void PatchAll(Harmony       h,                 string        typeName, string methodName,
                                 HarmonyMethod prefix     = null, HarmonyMethod postfix = null,
                                 HarmonyMethod transpiler = null) {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            Type t;
            try {
                t = asm.GetType(typeName);
            }
            catch (Exception e) {
                _log.Error($"Failed to get type {typeName} from assembly {asm.FullName}: {e.Message}");
                continue;
            }

            if (t == null) continue;
            MethodBase m;
            try {
                m = AccessTools.Method(t, methodName);
            }
            catch (Exception e) {
                _log.Error(
                    $"Failed to get method {methodName} from type {typeName} in assembly {asm.FullName}: {e.Message}");
                continue;
            }

            if (m != null) h.Patch(m, prefix, postfix, transpiler);
        }
    }

    private static void PatchCtor(Harmony       h,             string        typeName, int paramCount,
                                  HarmonyMethod prefix = null, HarmonyMethod postfix = null) {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            Type t;
            try {
                t = asm.GetType(typeName);
            }
            catch (Exception e) {
                _log.Error($"Failed to get type {typeName} from assembly {asm.FullName}: {e.Message}");
                continue;
            }

            if (t == null) continue;
            var c = t.GetConstructors()
                .FirstOrDefault(ctor => ctor.GetParameters().Length == paramCount);
            if (c != null) h.Patch(c, prefix, postfix);
        }
    }

    private static HarmonyMethod M<T>(string method) =>
        new HarmonyMethod(typeof(T), method);

    internal static void Apply(Harmony harmony) {
        const string sceneScan = "Terraria.SceneMetrics";
        const string player    = "Terraria.Player";
        const string main      = "Terraria.Main";

        var isServer = PluginLoader.IsDedicatedServer;

        if (!isServer) {
            PatchAll(harmony, main, "NPCChatText_DoAnglerQuest",
                postfix: M<AnglerQuestPatch>(nameof(AnglerQuestPatch.Postfix)));

            // SceneMetrics.Scan postfixes access Terraria.exe Main directly; never run on server.
            PatchAll(harmony, sceneScan, "Scan",
                postfix: M<BannerBuffsPatch>(nameof(BannerBuffsPatch.PostFix)));
            PatchAll(harmony, sceneScan, "Scan",
                postfix: M<GardenGnomePatch>(nameof(GardenGnomePatch.Postfix)));

            PatchAll(harmony, player, "ExtractinatorUse",
                prefix: M<ExtractinatorUsePatch>(nameof(ExtractinatorUsePatch.Prefix)));

            PatchAll(harmony, "Terraria.UI.ItemSlot", "TryItemSwap",
                prefix: M<ItemSwapPatch>(nameof(ItemSwapPatch.Prefix)));

            PatchAll(harmony, "Terraria.UI.ItemSlot", "TryOpenContainer",
                postfix: M<OpenCratesPatch>(nameof(OpenCratesPatch.PostFix)));

            PatchAll(harmony, "Terraria.QuickStacking", "QuickStackToNearbyInventories",
                prefix: M<QuickStackPatch>(nameof(QuickStackPatch.Prefix)));

            PatchAll(harmony, main, "MouseText_DrawBuffTooltip",
                prefix: M<SortedBannerBuffsPatch>(nameof(SortedBannerBuffsPatch.Prefix)));

            PatchCtor(harmony, "Terraria.GameContent.UI.Elements.UIBestiaryEntryButton", 2,
                postfix: M<SpawnTogglePatch>(nameof(SpawnTogglePatch.BestiaryButtonCtorPostfix)));
            PatchAll(harmony, "Terraria.GameContent.UI.Elements.UIBestiaryEntryButton", "DrawSelf",
                postfix: M<SpawnTogglePatch>(nameof(SpawnTogglePatch.BestiaryButtonDrawPostfix)));

            PatchAll(harmony, main, "MouseText_DrawItemTooltip_GetLinesInfo",
                postfix: M<VanillaItemTooltipPatch>(nameof(VanillaItemTooltipPatch.Postfix)));
        }

        PatchAll(harmony, "Terraria.GameContent.Generation.Dungeon.DungeonUtils", "ChangeTileType",
            prefix: M<CrackedDungeonBrickPatch>(nameof(CrackedDungeonBrickPatch.Prefix)));

        PatchAll(harmony, "Terraria.NPC", "NewNPC",
            prefix: M<SpawnTogglePatch>(nameof(SpawnTogglePatch.NewNpcPrefix)));
        PatchAll(harmony, "Terraria.NPC+Spawner", "SpawnAnNPC",
            prefix: M<SpawnTogglePatch>(nameof(SpawnTogglePatch.SpawnAnNpcPrefix)),
            postfix: M<SpawnTogglePatch>(nameof(SpawnTogglePatch.SpawnAnNpcPostfix)));

        const string pylonSystem = "Terraria.GameContent.TeleportPylonsSystem";
        PatchAll(harmony, pylonSystem, "IsPlayerNearAPylon",
            prefix: M<PylonsPatch>(nameof(PylonsPatch.IsPlayerNearAPylonPrefix)));
        PatchAll(harmony, pylonSystem, "DoesPylonHaveEnoughNPCsAroundIt",
            prefix: M<PylonsPatch>(nameof(PylonsPatch.NpcCheckPrefix)));
        PatchAll(harmony, pylonSystem, "HandleTeleportRequest",
            prefix: M<PylonsPatch>(nameof(PylonsPatch.HandleTeleportRequestPrefix)));
        PatchAll(harmony, pylonSystem, "HandleTeleportRequest",
            postfix: M<PylonsPatch>(nameof(PylonsPatch.HandleTeleportRequestPostfix)));
        PatchAll(harmony, pylonSystem, "HasPylonOfType",
            prefix: M<PylonsPatch>(nameof(PylonsPatch.HasPylonOfTypePrefix)));
        PatchAll(harmony, player, "InTileEntityInteractionRange",
            prefix: M<PylonsPatch>(nameof(PylonsPatch.InTileEntityInteractionRangePrefix)));
        PatchAll(harmony, player, "IsInTileInteractionRange",
            prefix: M<PylonsPatch>(nameof(PylonsPatch.PlayerInteractionRangePrefix)));
    }
}