using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityMod.Walls.DraedonStructures
{
    public class HazardChevronWall : ModWall
    {

        public override void SetStaticDefaults()
        {
            DustType = 19;
            ItemDrop = ModContent.ItemType<Items.Placeables.Walls.DraedonStructures.HazardChevronWall>();
            Main.wallHouse[Type] = true;

            AddMapEntry(new Color(114, 105, 51));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
