using HarmonyLib;
using Terraria.GameContent.Generation.Dungeon;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(DungeonUtils), "ChangeTileType")]
public static class CrackedDungeonBrickPatch {
    [HarmonyPrefix]
    public static bool Prefix(ushort tileType) {
        if (!_config.NoCrackedDungeonBricks) return true;
        return tileType is < TileID.CrackedBlueDungeonBrick or > TileID.CrackedPinkDungeonBrick;
    }
}