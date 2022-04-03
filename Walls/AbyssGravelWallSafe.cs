using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Walls
{
    public class AbyssGravelWallSafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = ModContent.ItemType<Items.Placeables.Walls.AbyssGravelWallItem>();
            AddMapEntry(new Color(6, 10, 54));
            dustType = 33;
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
