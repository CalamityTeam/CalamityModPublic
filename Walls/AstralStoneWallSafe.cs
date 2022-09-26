using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class AstralStoneWallSafe : ModWall
    {

        public override void SetStaticDefaults()
        {
            // TODO -- Change this dust to be one more befitting Astral Stone.
            DustType = DustID.Shadowflame;
            ItemDrop = ModContent.ItemType<Items.Placeables.Walls.AstralStoneWall>();
            Main.wallHouse[Type] = true;

            WallID.Sets.Conversion.Stone[Type] = true;

            AddMapEntry(new Color(15, 26, 31));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
