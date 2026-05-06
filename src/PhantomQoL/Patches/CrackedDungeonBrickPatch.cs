using HarmonyLib;
using Terraria.GameContent.Generation.Dungeon;
using Terraria.ID;
using static PhantomQoL.Mod;

namespace PhantomQoL.Patches;

[HarmonyPatch(typeof(DungeonUtils), "ChangeTileType")]
public static class CrackedDungeonBrickPatch {
    [HarmonyPrefix]
    public static bool Prefix(ushort tileType) {
        if (!_config.NoCrackedDungeonBricks) return true;
        return tileType is < TileID.CrackedBlueDungeonBrick or > TileID.CrackedPinkDungeonBrick;
    }
}