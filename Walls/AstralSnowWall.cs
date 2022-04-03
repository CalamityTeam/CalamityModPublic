
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class AstralSnowWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            dustType = ModContent.DustType<Dusts.AstralBasic>();

            AddMapEntry(new Color(135, 145, 149));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
