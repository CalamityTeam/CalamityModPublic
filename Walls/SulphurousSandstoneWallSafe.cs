using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class SulphurousSandstoneWallSafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = 32;
            AddMapEntry(new Color(57, 45, 38));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
