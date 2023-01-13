using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class EutrophicSandWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            DustType = 108;
            AddMapEntry(new Color(11, 56, 81));
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j].LiquidAmount <= 0)
            {
                Main.tile[i, j].LiquidAmount = 255;
                Main.tile[i, j].Get<LiquidData>().LiquidType = LiquidID.Water;
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
