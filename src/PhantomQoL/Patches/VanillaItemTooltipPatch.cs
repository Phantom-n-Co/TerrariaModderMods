#nullable enable
using HarmonyLib;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(Main), nameof(Main.MouseText_DrawItemTooltip_GetLinesInfo))]
public static class VanillaItemTooltipPatch {
    private record Entry(Func<int, bool> Matches, string Text, Color Color, Func<bool>? Condition) {
        public Func<int, bool> Matches   { get; } = Matches;
        public string          Text      { get; } = Text;
        public Color           Color     { get; } = Color;
        public Func<bool>?     Condition { get; } = Condition;
    }

    private static readonly List<Entry> _entries = [];

    public static void Register(int itemType, string text, Color color, Func<bool>? condition = null) =>
        Register(type => type == itemType, text, color, condition);

    public static void Register(Func<int, bool> predicate, string text, Color color, Func<bool>? condition = null) =>
        _entries.Add(new Entry(predicate, text, color, condition));

    [HarmonyPostfix]
    public static void Postfix(Item item, ref int numLines, string[] toolTipLine, Color[] lineColors) {
        foreach (var entry in _entries) {
            if (numLines >= toolTipLine.Length) break;
            if (entry.Condition != null && !entry.Condition()) continue;
            if (!entry.Matches(item.type)) continue;
            toolTipLine[numLines] = entry.Text;
            lineColors[numLines]  = entry.Color;
            numLines++;
        }
    }
}