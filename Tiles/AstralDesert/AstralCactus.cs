using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using ReLogic.Content;

namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralCactus : ModCactus
    {
        public override void SetStaticDefaults()
        {
            // Grows on astral sand
            GrowsOnTileId = new int[1] { ModContent.TileType<AstralSand>() };
        }

        //Idk what to make with the glowmask
        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("CalamityMod/Tiles/AstralDesert/AstralCactus");

        //What is a FruitTexture
        public override Asset<Texture2D> GetFruitTexture() => null;

    }
}
