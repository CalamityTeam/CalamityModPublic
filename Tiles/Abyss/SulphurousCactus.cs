using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousCactus : ModCactus
    {
        public override Texture2D GetTexture() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/SulphurousCactus").Value;
    }
}
