using HarmonyLib;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(Main), nameof(Main.NPCChatText_DoAnglerQuest))]
public static class AnglerQuestPatch {
    [HarmonyPostfix]
    public static void Postfix() {
        if (!_config.InstantAnglerRefresh) return;
        if (Main.anglerQuestFinished) Main.AnglerQuestSwap();
    }
}