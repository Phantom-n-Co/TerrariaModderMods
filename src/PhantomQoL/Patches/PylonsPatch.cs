using System.Reflection.Emit;
using HarmonyLib;
using Terraria.DataStructures;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(TeleportPylonsSystem))]
public static class PylonsPatch {
    [HarmonyPrefix]
    [HarmonyPatch(nameof(TeleportPylonsSystem.IsPlayerNearAPylon))]
    public static bool Prefix(Player player, ref bool __result) {
        if (!_config.PylonTweaks) return true;
        __result = true;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("HowManyNPCsDoesPylonNeed")]
    public static bool Prefix(TeleportPylonInfo info, Player player, ref int __result) {
        if (!_config.PylonTweaks) return true;
        __result = 0;
        return false;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(TeleportPylonsSystem.HandleTeleportRequest))]
    public static IEnumerable<CodeInstruction> Transpiler_HandleTeleportRequest(
        IEnumerable<CodeInstruction> instructions) {
        var configField  = AccessTools.Field(typeof(Mod), nameof(_config));
        var tweaksGetter = AccessTools.PropertyGetter(typeof(PhantomConfig), nameof(PhantomConfig.PylonTweaks));
        var rangeCheck = AccessTools.Method(typeof(Player), nameof(Player.InTileEntityInteractionRange),
            [typeof(int), typeof(int), typeof(int), typeof(int), typeof(TileReachCheckSettings)]);

        return new CodeMatcher(instructions)
            .MatchStartForward(new CodeMatch(i => i.Calls(rangeCheck)))
            .Advance(1)
            .Insert(
                new CodeInstruction(OpCodes.Ldsfld, configField),
                new CodeInstruction(OpCodes.Callvirt, tweaksGetter),
                new CodeInstruction(OpCodes.Or))
            .InstructionEnumeration();
    }
}