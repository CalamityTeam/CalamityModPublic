using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class EutrophicSandWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            dustType = 108;
            AddMapEntry(new Color(80, 80, 120));
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j].LiquidAmount <= 0)
            {
                Main.tile[i, j].LiquidAmount = 255;
                Main.tile[i, j].LiquidType = LiquidID.Water;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
