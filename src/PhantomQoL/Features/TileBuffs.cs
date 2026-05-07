using Microsoft.Xna.Framework;
using PhantomQoL.Patches;

namespace PhantomQoL.Features;

public static class TileBuffs {
    private static readonly Color TooltipColor = new(150, 255, 150);
    private const string BuffText = "Applies buff while in inventory.";
    private const string LuckText = "Applies luck bonus while in inventory.";

    public static void RegisterTooltips() {
        Func<bool> enabled = () => _config.TileBuffsFromInventory;

        VanillaItemTooltipPatch.Register(
            type => type < ItemID.Sets.Campfires.Length && ItemID.Sets.Campfires[type],
            BuffText, TooltipColor, enabled);
        VanillaItemTooltipPatch.Register(ItemID.HeartLantern,          BuffText, TooltipColor, enabled);
        VanillaItemTooltipPatch.Register(ItemID.PeaceCandle,           BuffText, TooltipColor, enabled);
        VanillaItemTooltipPatch.Register(ItemID.StarinaBottle,         BuffText, TooltipColor, enabled);
        VanillaItemTooltipPatch.Register(ItemID.Sunflower,             BuffText, TooltipColor, enabled);
        VanillaItemTooltipPatch.Register(ItemID.CatBast,               BuffText, TooltipColor, enabled);
        VanillaItemTooltipPatch.Register(ItemID.HoneyBucket,           BuffText, TooltipColor, enabled);
        VanillaItemTooltipPatch.Register(ItemID.BottomlessHoneyBucket, BuffText, TooltipColor, enabled);
        VanillaItemTooltipPatch.Register(ItemID.GardenGnome,           LuckText, TooltipColor, enabled);
    }

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