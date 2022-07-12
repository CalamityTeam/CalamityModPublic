using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;

/// <summary>
/// Interface that can be used by chestplates that need their front arm (when walking) to go over the shoulderpads
/// </summary>
public interface IDrawArmOverShoulderpad
{
    /// <summary>
    /// The texture of the part that goes above the shoulderpads
    /// This should be the same size as a regular equipped sprite (20 x 560 in 1x1)
    /// </summary>
    string FrontArmTexture { get; }

    /// <summary>
    /// The name of the equip slot for the item. If left empty, the equip slot that is looked for will use the name of the item.
    /// Useful if you have multiple head textures you need to extend.
    /// </summary>
    string EquipSlotName(Player drawPlayer) => "";
}
