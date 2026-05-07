namespace PhantomQoL.Patches;

public class AnglerQuestPatch {
    public static void Postfix() {
        if (!_config.InstantAnglerRefresh) return;
        if (!Main.anglerQuestFinished) return;

        if (Main.netMode == 0) Main.AnglerQuestSwap();
        else {
            Main.anglerQuestFinished = false;
            Main.anglerQuest         = Main.rand.Next(Main.anglerQuestItemNetIDs.Length);
        }
    }
}