using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class HardenedSulphurousSandstoneWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = 32;
            ItemDrop = ModContent.ItemType<Items.Placeables.Walls.HardenedSulphurousSandstoneWall>();
            AddMapEntry(new Color(62, 50, 39));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
