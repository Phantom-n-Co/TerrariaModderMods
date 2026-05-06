namespace PhantomQoL.Features;

public static class TileBuffs {
    private static readonly Dictionary<int, int> _itemToBuffMap = new() {
        { ItemID.HeartLantern, BuffID.HeartLamp },
        { ItemID.PeaceCandle, BuffID.PeaceCandle },
        { ItemID.StarinaBottle, BuffID.StarInBottle },
        { ItemID.Sunflower, BuffID.Sunflower },
        { ItemID.CatBast, BuffID.CatBast },
        { ItemID.HoneyBucket, BuffID.Honey },
        { ItemID.BottomlessHoneyBucket, BuffID.Honey },
    };

    public static void ApplyBuffs() {
        if (!_config.TileBuffsFromInventory) return;
        var player = Main.player[Main.myPlayer];
        if (player == null || player.dead) return;

        var buffs = new HashSet<int>();
        CheckItems(player.inventory, buffs);
        CheckItems(player.bank.item, buffs);
        CheckItems(player.bank2.item, buffs);
        CheckItems(player.bank3.item, buffs);
        CheckItems(player.bank4.item, buffs);

        foreach (var buff in buffs)
            player.AddBuff(buff, 2);
    }

    private static void CheckItems(Item[] items, HashSet<int> buffs) {
        foreach (var item in items) {
            if (item is not { type: > 0 }) continue;
            if (item.type < ItemID.Sets.Campfires.Length && ItemID.Sets.Campfires[item.type])
                buffs.Add(BuffID.Campfire);
            if (_itemToBuffMap.TryGetValue(item.type, out var buffId))
                buffs.Add(buffId);
        }
    }
}