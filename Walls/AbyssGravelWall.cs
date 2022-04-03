using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class AbyssGravelWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            dustType = 33;
            AddMapEntry(new Color(6, 10, 54));
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j].liquid <= 0 && j < Main.maxTilesY - 205)
            {
                Main.tile[i, j].liquid = 255;
                Main.tile[i, j].lava(false);
            }
        }

        public override void KillWall(int i, int j, ref bool fail) => fail = true;

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
