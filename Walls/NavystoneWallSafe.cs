using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class NavystoneWallSafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = 96;
            AddMapEntry(new Color(0, 50, 50));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
