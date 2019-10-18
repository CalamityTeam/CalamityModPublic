using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralPalmTree : ModPalmTree
    {
        public override Texture2D GetTexture()
        {
            return ModContent.GetInstance<CalamityMod>().GetTexture("Tiles/AstralDesert/AstralPalmTree");
        }

        public override Texture2D GetTopTextures()
        {
            return ModContent.GetInstance<CalamityMod>().GetTexture("Tiles/AstralDesert/AstralPalmTree_Tops");
        }

        public override int DropWood()
        {
            return ModContent.GetInstance<CalamityMod>().ItemType("AstralMonolith");
        }

        public override int CreateDust()
        {
            return ModContent.GetInstance<CalamityMod>().DustType("AstralBasic");
        }

        public override int GrowthFXGore()
        {
            return -1;
        }
    }
}
