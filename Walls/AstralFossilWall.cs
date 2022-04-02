using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class AstralFossilWall : ModWall
    {

        public override void SetDefaults()
        {
            dustType = ModContent.DustType<Dusts.AstralBasic>();
            drop = ModContent.ItemType<Items.Placeables.Walls.AstralFossilWall>();
            Main.wallHouse[Type] = true;

            AddMapEntry(new Color(29, 38, 49));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
