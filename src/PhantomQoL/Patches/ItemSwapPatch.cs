using Terraria.Audio;
using TerrariaModder.Core.Assets;

namespace PhantomQoL.Patches;

/// <summary>
/// Generic two-way item swap patch mirroring vanilla TryItemSwap behavior.
/// Call Register() after runtime types are assigned (OnContentReady) to add a pair.
/// </summary>
public class ItemSwapPatch {
    private static readonly Dictionary<int, int> _pairs = new();

    /// <summary>Register a swap pair by full item IDs ("modid:name").</summary>
    public static void Register(string fullIdA, string fullIdB) {
        var typeA = ItemRegistry.GetRuntimeType(fullIdA);
        var typeB = ItemRegistry.GetRuntimeType(fullIdB);
        if (typeA < 0 || typeB < 0) return;
        Register(typeA, typeB);
    }

    /// <summary>Register a swap pair by runtime type IDs.</summary>
    public static void Register(int typeA, int typeB) {
        _pairs[typeA] = typeB;
        _pairs[typeB] = typeA;
        ExtendHasItemSwap(typeA);
        ExtendHasItemSwap(typeB);
    }

    private static void ExtendHasItemSwap(int type) {
        if (type >= ItemID.Sets.HasItemSwap.Length) {
            var arr = new bool[type + 1];
            Array.Copy(ItemID.Sets.HasItemSwap, arr, ItemID.Sets.HasItemSwap.Length);
            ItemID.Sets.HasItemSwap = arr;
        }

        ItemID.Sets.HasItemSwap[type] = true;
    }

    public static bool Prefix(Item item) {
        if (!_pairs.TryGetValue(item.type, out var newType)) return true;

        item.ChangeItemType(newType);
        SoundEngine.PlaySound(22);
        Main.stackSplit        = 30;
        Main.mouseRightRelease = false;

        return false;
    }
}