using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using ReLogic.Content;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousCactus : ModCactus
    {
        public override void SetStaticDefaults()
        {
            // Grows on sulphurous sand
            GrowsOnTileId = new int[] { ModContent.TileType<SulphurousSand>() };
        }

        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("CalamityMod/Tiles/Abyss/SulphurousCactus");

        //What is a FruitTexture
        public override Asset<Texture2D> GetFruitTexture() => null;
    }
}
