namespace PhantomQoL.Patches;

public class CrackedDungeonBrickPatch {
    public static bool ChangeTileTypePrefix(ushort tileType) {
        if (!_config.NoCrackedDungeonBricks) return true;
        return tileType is < TileID.CrackedBlueDungeonBrick or > TileID.CrackedPinkDungeonBrick;
    }
}