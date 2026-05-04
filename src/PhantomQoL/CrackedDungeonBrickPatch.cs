using HarmonyLib;
using Terraria;
using Terraria.GameContent.Generation.Dungeon;
using Terraria.ID;
using static PhantomQoL.Mod;

namespace PhantomQoL;

[HarmonyPatch(typeof(DungeonUtils), "ChangeTileType")]
public static class CrackedDungeonBrickPatch {
    [HarmonyPrefix]
    public static bool Prefix(ushort tileType) {
        if (!_config.NoCrackedDungeonBricksEnabled) return true;
        return tileType < TileID.CrackedBlueDungeonBrick || tileType > TileID.CrackedPinkDungeonBrick;
    }

    public static void RemoveExistingCrackedBricks() {
        if (Main.netMode == 1) {
            Main.NewText("[PhantomQoL] Can only modify tiles in single player or on the server.", 255, 80, 80);
            return;
        }

        int removed = 0;
        for (int x = 0; x < Main.maxTilesX; x++) {
            for (int y = 0; y < Main.maxTilesY; y++) {
                Tile tile = Main.tile[x, y];
                if (!tile.active()) continue;
                ushort t = tile.type;
                if (t < TileID.CrackedBlueDungeonBrick || t > TileID.CrackedPinkDungeonBrick) continue;
                tile.active(false);
                WorldGen.SquareTileFrame(x, y);
                if (Main.netMode == 2) NetMessage.SendTileSquare(-1, x, y, 1);
                removed++;
            }
        }

        string msg = removed > 0
            ? $"Removed {removed} cracked dungeon bricks."
            : "No cracked dungeon bricks found.";
        Main.NewText($"[PhantomQoL] {msg}", 0, 200, 200);
        _log.Info(msg);
    }
}