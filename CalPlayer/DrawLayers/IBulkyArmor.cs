using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;

/// <summary>
/// Interface that can be used by chestplates to appear extra bulky, by drawing partly over the players helmet with an extra layer
/// </summary>
public interface IBulkyArmor
{
    /// <summary>
    /// The texture of the part that goes above the helmet
    /// This should be the same size as a regular equipped sprite (20 x 560 in 1x1)
    /// </summary>
    string BulkTexture { get; }

    /// <summary>
    /// The name of the equip slot for the item. If left empty, the equip slot that is looked for will use the name of the item.
    /// Useful if you have multiple body textures you need to add bulk to.
    /// </summary>
    string EquipSlotName(Player drawPlayer) => "";
}
