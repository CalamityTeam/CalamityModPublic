using CalamityMod.Dusts;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralPalmTree : ModPalmTree
    {
        public override Texture2D GetTexture()
        {
            return ModContent.Request<Texture2D>("CalamityMod/Tiles/AstralDesert/AstralPalmTree");
        }

        public override Texture2D GetTopTextures()
        {
            return ModContent.Request<Texture2D>("CalamityMod/Tiles/AstralDesert/AstralPalmTree_Tops");
        }

        public override int DropWood()
        {
            return ModContent.ItemType<Items.Placeables.AstralMonolith>();
        }

        public override int CreateDust()
        {
            return ModContent.DustType<AstralBasic>();
        }

        public override int GrowthFXGore()
        {
            return -1;
        }
    }
}
