using Microsoft.Xna.Framework;
using Terraria.DataStructures;

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
}
