using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
namespace CalamityMod.Tiles
{
    public class AstralTree : ModTree
    {
        public override Texture2D GetTopTextures(int i, int j, ref int frame, ref int frameWidth, ref int frameHeight, ref int xOffsetLeft, ref int yOffset)
        {
            frame = (i + j * j) % 3;
            return ModContent.GetInstance<CalamityMod>().GetTexture("Tiles/Astral/AstralTree_Tops");
        }

        public override Texture2D GetBranchTextures(int i, int j, int trunkOffset, ref int frame)
        {
            return ModContent.GetInstance<CalamityMod>().GetTexture("Tiles/Astral/AstralTree_Branches");
        }

        public override Texture2D GetTexture()
        {
            return ModContent.GetInstance<CalamityMod>().GetTexture("Tiles/Astral/AstralTree");
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
