using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class EutrophicSandWallSafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = 108;
            ItemDrop = ModContent.ItemType<Items.Placeables.Walls.EutrophicSandWallSafe>();
            AddMapEntry(new Color(11, 56, 81));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
