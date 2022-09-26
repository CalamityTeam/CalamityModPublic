using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityMod.Walls.DraedonStructures
{
    public class LaboratoryPlateBeam : ModWall
    {

        public override void SetStaticDefaults()
        {
            DustType = 109;
            ItemDrop = ModContent.ItemType<Items.Placeables.Walls.DraedonStructures.LaboratoryPlateBeam>();
            Main.wallHouse[Type] = true;

            AddMapEntry(new Color(48, 45, 46));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
