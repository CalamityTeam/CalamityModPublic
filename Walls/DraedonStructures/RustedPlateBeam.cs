using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls.DraedonStructures
{
    public class RustedPlateBeam : ModWall
    {

        public override void SetStaticDefaults()
        {
            DustType = 32;
            ItemDrop = ModContent.ItemType<Items.Placeables.Walls.DraedonStructures.RustedPlateBeam>();
            Main.wallHouse[Type] = true;

            AddMapEntry(new Color(74, 47, 39));
        }

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
