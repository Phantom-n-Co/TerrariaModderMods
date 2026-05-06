using TerrariaModder.Core.Input;

namespace PhantomQoL;

using static Mod;

public static class Keybinds {
    public static void Init(ModContext ctx) {
        ctx.RegisterKeybind("remove-cracked-bricks", "Remove Cracked Dungeon Bricks",
            "Scans the world and removes cracked dungeon bricks", new KeyCombo(KeyCode.None),
            RemoveExistingCrackedBricks);
    }

    private static void RemoveExistingCrackedBricks() {
        if (Main.netMode == 1) {
            Main.NewText("[PhantomQoL] Can only modify tiles in single player or on the server.", 255, 80, 80);
            return;
        }

        var removed = 0;
        for (var x = 0; x < Main.maxTilesX; x++) {
            for (var y = 0; y < Main.maxTilesY; y++) {
                var tile = Main.tile[x, y];
                if (!tile.active()) continue;
                var t = tile.type;
                if (t is < TileID.CrackedBlueDungeonBrick or > TileID.CrackedPinkDungeonBrick) continue;
                tile.active(false);
                WorldGen.SquareTileFrame(x, y);
                if (Main.netMode == 2) NetMessage.SendTileSquare(-1, x, y, 1);
                removed++;
            }
        }

        var msg = removed > 0
            ? $"Removed {removed} cracked dungeon bricks."
            : "No cracked dungeon bricks found.";
        Main.NewText($"[PhantomQoL] {msg}", 0, 200, 200);
        _log.Info(msg);
    }
}