// using TerrariaModder.Core.Events;
using System;
using PhantomQoL.Patches;
using Terraria;
using TerrariaModder.Core;
using TerrariaModder.Core.Assets;
using static PhantomQoL.Mod;

namespace PhantomQoL.Items;

public static class ItemMagnet {
    private const string TEXTURE          = "assets/items/ItemMagnet.png";
    private const string ITEM_ID          = "phantom-qol:magnet";
    private const string DISABLED_ITEM_ID = "phantom-qol:magnet-disabled";

    public static void Init(ModContext ctx) {
        ctx.RegisterItem("magnet", new ItemDefinition {
            DisplayName = "Item Magnet",
            MaxStack    = 1,
            Texture     = TEXTURE,
            ModifyTooltips = lines => {
                if (_config.ItemMagnet) {
                    lines.Add("Pulls in nearby items.");
                    lines.Add("Works in your inventory.");
                    lines.Add("Right-click to toggle.");
                }
                else {
                    lines.Add("Item Magnet is disabled in config.");
                    lines.Add("This will not do anything until enabled in config.");
                }
            }
        });
        ctx.RegisterItem("magnet-disabled", new ItemDefinition {
            DisplayName = "Item Magnet (Disabled)",
            MaxStack    = 1,
            Texture     = TEXTURE,
            ModifyTooltips = lines => {
                if (_config.ItemMagnet) {
                    lines.Add("Pulls in nearby items.");
                    lines.Add("Works in your inventory.");
                    lines.Add("Right-click to toggle.");
                }
                else {
                    lines.Add("Item Magnet is disabled in config.");
                    lines.Add("This will not do anything until enabled in config.");
                }
            }
        });

        if (_config.ItemMagnet)
            ctx.AddShopItem(new ShopDefinition {
                NpcType = 1,
                ItemId  = ITEM_ID,
                Price   = 50000
            });
    }

    private static int ActiveId   { get; set; } = -1;
    private static int InactiveId { get; set; } = -1;

    public static void RegisterSwap() {
        ActiveId   = ItemRegistry.GetRuntimeType(ITEM_ID);
        InactiveId = ItemRegistry.GetRuntimeType(DISABLED_ITEM_ID);
        if (ActiveId < 0 || InactiveId < 0) return;
        ItemSwapPatch.Register(ActiveId, InactiveId);
    }

    private const float Range    = 800f;
    private const float Speed    = 1.2f;
    private const float MaxSpeed = 10f;

    // PlayerEvents.OnPlayerUpdate is declared in the framework but FirePlayerUpdate is never called.
    // Kept here for when it gets implemented.
    // public static void OnPlayerUpdate(PlayerEventArgs args) {
    //     if (!_config.ItemMagnet || ActiveId < 0) return;
    //     var player = Main.player[Main.myPlayer];
    //     if (player == null || player.dead) return;
    //     var hasMagnet = false;
    //     for (var i = 0; i < 58; i++) {
    //         if (player.inventory[i]?.type != ActiveId) continue;
    //         hasMagnet = true;
    //         break;
    //     }
    //
    //     if (!hasMagnet) return;
    //     var cx = player.position.X + player.width  * 0.5f;
    //     var cy = player.position.Y + player.height * 0.5f;
    //     for (var j = 0; j < Main.maxItems; j++) {
    //         var item = Main.item[j];
    //         if (!item.active || item.noGrabDelay != 0 || item.shimmerTime != 0f) continue;
    //         if (item.playerIndexTheItemIsReservedFor != Main.myPlayer) continue;
    //         var dx   = cx - (item.position.X + item.width  * 0.5f);
    //         var dy   = cy - (item.position.Y + item.height * 0.5f);
    //         var dist = (float)Math.Sqrt(dx * dx + dy * dy);
    //         if (dist > Range || dist < 1f) continue;
    //         var nx = dx / dist;
    //         var ny = dy / dist;
    //         var vx = item.velocity.X + nx * Speed;
    //         var vy = item.velocity.Y + ny * Speed;
    //         item.velocity.X = vx < -MaxSpeed ? -MaxSpeed : vx > MaxSpeed ? MaxSpeed : vx;
    //         item.velocity.Y = vy < -MaxSpeed ? -MaxSpeed : vy > MaxSpeed ? MaxSpeed : vy;
    //     }
    // }

    public static void MagnetPull() {
        if (!_config.ItemMagnet || ActiveId < 0) return;

        var player = Main.player[Main.myPlayer];
        if (player == null || player.dead) return;

        var hasMagnet = false;
        for (var i = 0; i < 58; i++) {
            if (player.inventory[i]?.type != ActiveId) continue;
            hasMagnet = true;
            break;
        }

        if (!hasMagnet) return;

        var cx = player.position.X + player.width  * 0.5f;
        var cy = player.position.Y + player.height * 0.5f;

        for (var j = 0; j < Main.maxItems; j++) {
            var item = Main.item[j];
            if (!item.active || item.noGrabDelay != 0 || item.shimmerTime != 0f) continue;
            if (item.playerIndexTheItemIsReservedFor != Main.myPlayer) continue;

            var dx   = cx - (item.position.X + item.width  * 0.5f);
            var dy   = cy - (item.position.Y + item.height * 0.5f);
            var dist = (float)Math.Sqrt(dx * dx + dy * dy);

            if (dist is > Range or < 1f) continue;

            var nx = dx / dist;
            var ny = dy / dist;

            var vx = item.velocity.X + nx * Speed;
            var vy = item.velocity.Y + ny * Speed;

            item.velocity.X = vx < -MaxSpeed ? -MaxSpeed : vx > MaxSpeed ? MaxSpeed : vx;
            item.velocity.Y = vy < -MaxSpeed ? -MaxSpeed : vy > MaxSpeed ? MaxSpeed : vy;
        }
    }
}