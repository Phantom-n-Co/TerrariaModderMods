using TerrariaModder.Core.UI;

namespace PhantomQoL.UI;

internal static class BannerWindow {
    private const int ItemH        = 18;
    private const int ColW         = 165;
    private const int MaxRows      = 25;
    private const int Padding      = 8;
    private const int HeaderHeight = 22;

    private static readonly List<string> _entries = [];

    public static void OnDraw() {
        if (!_config.SortedBannerWindow) return;
        if (Main.gameMenu || !Main.bannerMouseOver) return;

        RefreshEntries();
        if (_entries.Count == 0) return;

        var cols   = Math.Max(1, (_entries.Count + MaxRows - 1) / MaxRows);
        var rows   = Math.Min(_entries.Count, MaxRows);
        var panelW = cols * ColW  + Padding * 2;
        var panelH = rows * ItemH + Padding * 2 + HeaderHeight;

        var mx     = UIRenderer.MouseX;
        var my     = UIRenderer.MouseY;
        var panelX = mx + 20;
        var panelY = my + 30;

        if (panelX + panelW > Main.screenWidth) panelX  = mx - panelW - 5;
        if (panelY + panelH > Main.screenHeight) panelY = my - panelH - 5;

        UIRenderer.DrawPanel(panelX, panelY, panelW, panelH);
        UIRenderer.DrawTextShadow($"Banners: {_entries.Count}",
            panelX + Padding, panelY + Padding, 255, 220, 120);

        var y0 = panelY + Padding + HeaderHeight;
        for (var i = 0; i < _entries.Count; i++) {
            var col = i / MaxRows;
            var row = i % MaxRows;
            UIRenderer.DrawText(_entries[i],
                panelX + Padding + col * ColW, y0 + row * ItemH,
                220, 220, 220);
        }
    }

    private static void RefreshEntries() {
        _entries.Clear();
        var player = Main.player[Main.myPlayer];
        if (player == null || player.dead) return;
        var bannerBuffs = Main.SceneMetrics.NPCBannerBuff;

        for (var npcType = 1; npcType < NPCID.Count; npcType++) {
            var bannerType = BannerSystem.NPCtoBanner(npcType);
            if (bannerType <= 0 || bannerType >= bannerBuffs.Length) continue;
            if (!bannerBuffs[bannerType]) continue;
            var name = Lang.GetNPCNameValue(npcType);
            if (!string.IsNullOrWhiteSpace(name) && !_entries.Contains(name))
                _entries.Add(name);
        }

        _entries.Sort(StringComparer.CurrentCultureIgnoreCase);
    }
}