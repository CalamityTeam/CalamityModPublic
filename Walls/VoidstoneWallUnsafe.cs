using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class VoidstoneWallUnsafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            DustType = 187;
            AddMapEntry(new Color(0, 0, 0));
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j].LiquidAmount <= 0 && j < Main.maxTilesY - 205)
            {
                Main.tile[i, j].LiquidAmount = 255;
                Main.tile[i, j].Get<LiquidData>().LiquidType = LiquidID.Water;
            }
        }

        public override void KillWall(int i, int j, ref bool fail) => fail = true;

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
