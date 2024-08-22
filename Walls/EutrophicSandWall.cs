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
            if (Main.tile[i, j].LiquidAmount == 0 && j < Main.maxTilesY - 205)
            {
                Main.tile[i, j].Get<LiquidData>().LiquidType = LiquidID.Water;
                Main.tile[i, j].LiquidAmount = byte.MaxValue;
                WorldGen.SquareTileFrame(i, j);
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.sendWater(i, j);
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
