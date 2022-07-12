using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria;

/// <summary>
/// Interface that can be used by items that need to hide the players front arm when held.
/// Useful for prosthesis type items.
/// </summary>
public interface IHideFrontArm
{
    /// <summary>
    /// When should the arm be hidden. Defauls to always
    /// </summary>
    bool ShouldHideArm(Player player) => true;
}
