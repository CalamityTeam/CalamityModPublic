using Microsoft.Xna.Framework;
using Terraria.DataStructures;

public interface IExtendedHat
{
    /// <summary>
    /// The texture of the extension
    /// </summary>
    string ExtensionTexture { get; }
    /// <summary>
    /// Unless you are using custom drawing, mount offsets are taken into account automatically.
    /// </summary>
    Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo);
    /// <summary>
    ///Return true to make the extension get drawn automatically from the texture and offsets provided. Return false if you want to draw it yourself
    /// </summary>
    bool PreDrawExtension(PlayerDrawSet drawInfo) => true;
}
