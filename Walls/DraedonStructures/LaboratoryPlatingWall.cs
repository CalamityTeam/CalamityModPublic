using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityMod.Walls.DraedonStructures
{
	public class LaboratoryPlatingWall : ModWall
    {

        public override void SetDefaults()
        {
            dustType = 30;
            drop = ModContent.ItemType<Items.Placeables.Walls.DraedonStructures.LaboratoryPlatingWall>();
            Main.wallHouse[Type] = true;

            AddMapEntry(new Color(105, 102, 98));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
